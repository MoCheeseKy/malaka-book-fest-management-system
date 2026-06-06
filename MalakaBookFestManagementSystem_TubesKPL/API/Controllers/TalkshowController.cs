using MalakaBookFest.Application.Common;
using MalakaBookFest.Application.DTOs.Talkshow;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MalakaBookFest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TalkshowController : ControllerBase
{
    private readonly ITalkshowService _talkshowService;

    public TalkshowController(ITalkshowService talkshowService)
    {
        _talkshowService = talkshowService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TalkshowDto>>>> GetAll()
    {
        var talkshows = await _talkshowService.GetAllTalkshowsAsync();
        return Ok(ApiResponse<IEnumerable<TalkshowDto>>.Ok(talkshows.Select(MapToDto)));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TalkshowDto>>> GetById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.Fail("Talkshow id is required."));
        }

        var talkshow = await _talkshowService.GetTalkshowByIdAsync(id);
        return Ok(ApiResponse<TalkshowDto>.Ok(MapToDto(talkshow)));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<TalkshowDto>>> Create([FromBody] CreateTalkshowDto dto)
    {
        var talkshow = new Talkshow
        {
            Title       = dto.Title,
            SpeakerName = dto.SpeakerName,
            SpeakerBio  = dto.SpeakerBio,
            Venue       = dto.Venue,
            StartTime   = dto.StartTime,
            EndTime     = dto.EndTime,
            MaxCapacity = dto.MaxCapacity,
        };

        var created = await _talkshowService.CreateTalkshowAsync(talkshow);
        return CreatedAtAction(nameof(GetById), new { id = created.TalkshowId },
            ApiResponse<TalkshowDto>.Ok(MapToDto(created), "Talkshow created successfully."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<TalkshowDto>>> Update(
        Guid id, [FromBody] UpdateTalkshowDto dto)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.Fail("Talkshow id is required."));
        }

        var existing = await _talkshowService.GetTalkshowByIdAsync(id);

        existing.Title       = dto.Title       ?? existing.Title;
        existing.SpeakerName = dto.SpeakerName ?? existing.SpeakerName;
        existing.SpeakerBio  = dto.SpeakerBio  ?? existing.SpeakerBio;
        existing.Venue       = dto.Venue       ?? existing.Venue;
        existing.StartTime   = dto.StartTime   ?? existing.StartTime;
        existing.EndTime     = dto.EndTime     ?? existing.EndTime;
        existing.MaxCapacity = dto.MaxCapacity ?? existing.MaxCapacity;

        var updated = await _talkshowService.UpdateTalkshowAsync(id, existing);
        return Ok(ApiResponse<TalkshowDto>.Ok(MapToDto(updated), "Talkshow updated successfully."));
    }

    [HttpPost("{id:guid}/register")]
    [Authorize(Roles = "Attendee,Organizer,Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Register(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.Fail("Talkshow id is required."));
        }

        var userId       = GetRequesterId();
        var registration = await _talkshowService.RegisterAttendeeAsync(userId, id);

        return Ok(ApiResponse<object>.Ok(new
        {
            registration.RegistrationId,
            registration.SeatCode,
            registration.RegisteredAt,
        }, "Successfully registered for talkshow."));
    }

    [HttpPost("{id:guid}/advance-status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> AdvanceStatus(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.Fail("Talkshow id is required."));
        }

        await _talkshowService.AdvanceTalkshowStatusAsync(id);
        return Ok(ApiResponse<object>.Ok(null!, "Talkshow status advanced successfully."));
    }

    private Guid GetRequesterId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User identity not found."));

    private static TalkshowDto MapToDto(Talkshow t) => new()
    {
        TalkshowId      = t.TalkshowId,
        Title           = t.Title,
        SpeakerName     = t.SpeakerName,
        SpeakerBio      = t.SpeakerBio,
        Venue           = t.Venue,
        StartTime       = t.StartTime,
        EndTime         = t.EndTime,
        MaxCapacity     = t.MaxCapacity,
        RegisteredCount = t.Registrations?.Count ?? 0,
        Status          = t.Status,
    };
}
