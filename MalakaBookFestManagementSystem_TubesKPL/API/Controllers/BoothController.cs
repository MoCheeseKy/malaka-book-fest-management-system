using MalakaBookFest.Application.Common;
using MalakaBookFest.Application.DTOs.Booth;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MalakaBookFest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoothController : ControllerBase
{
    private readonly IBoothService _boothService;

    public BoothController(IBoothService boothService)
    {
        _boothService = boothService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<BoothDto>>>> GetAll()
    {
        var booths = await _boothService.GetAllBoothsAsync();
        var dtos   = booths.Select(MapToDto);
        return Ok(ApiResponse<IEnumerable<BoothDto>>.Ok(dtos));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<BoothDto>>> GetById(Guid id)
    {
        var booth = await _boothService.GetBoothByIdAsync(id);
        return Ok(ApiResponse<BoothDto>.Ok(MapToDto(booth)));
    }

    [HttpPost]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<ActionResult<ApiResponse<BoothDto>>> Create([FromBody] CreateBoothDto dto)
    {
        var requesterId = GetRequesterId();

        var booth = new Booth
        {
            OrganizerId = requesterId,
            BoothName   = dto.BoothName,
            Description = dto.Description,
            BoothNumber = dto.BoothNumber,
            Category    = dto.Category,
        };

        var created = await _boothService.CreateBoothAsync(booth);
        return CreatedAtAction(nameof(GetById), new { id = created.BoothId },
            ApiResponse<BoothDto>.Ok(MapToDto(created), "Booth created successfully."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<ActionResult<ApiResponse<BoothDto>>> Update(
        Guid id, [FromBody] UpdateBoothDto dto)
    {
        var requesterId = GetRequesterId();
        var existing    = await _boothService.GetBoothByIdAsync(id);

        existing.BoothName   = dto.BoothName   ?? existing.BoothName;
        existing.Description = dto.Description ?? existing.Description;
        existing.Category    = dto.Category    ?? existing.Category;
        existing.IsActive    = dto.IsActive    ?? existing.IsActive;

        var updated = await _boothService.UpdateBoothAsync(id, existing, requesterId);
        return Ok(ApiResponse<BoothDto>.Ok(MapToDto(updated), "Booth updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var requesterId = GetRequesterId();
        await _boothService.DeleteBoothAsync(id, requesterId);
        return Ok(ApiResponse<object>.Ok(null!, "Booth deleted successfully."));
    }

    private Guid GetRequesterId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User identity not found."));

    private static BoothDto MapToDto(Booth b) => new()
    {
        BoothId       = b.BoothId,
        OrganizerId   = b.OrganizerId,
        OrganizerName = b.Organizer?.FullName ?? string.Empty,
        BoothName     = b.BoothName,
        Description   = b.Description,
        BoothNumber   = b.BoothNumber,
        Category      = b.Category,
        IsActive      = b.IsActive,
    };
}
