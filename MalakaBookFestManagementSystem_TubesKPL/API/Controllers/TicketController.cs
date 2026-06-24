using MalakaBookFest.Application.Common;
using MalakaBookFest.Application.DTOs.Ticket;
using MalakaBookFest.Application.Tables;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MalakaBookFest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly TicketPriceTable _priceTable;

    public TicketController(ITicketService ticketService, TicketPriceTable priceTable)
    {
        _ticketService = ticketService;
        _priceTable    = priceTable;
    }

    [HttpGet("prices")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<object>> GetPrices()
    {
        var prices = _priceTable.GetAllPrices()
            .ToDictionary(kv => kv.Key.ToString(), kv => kv.Value);

        return Ok(ApiResponse<object>.Ok(prices));
    }

    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TicketDto>>>> GetMyTickets()
    {
        var userId  = GetRequesterId();
        var tickets = await _ticketService.GetTicketsByUserAsync(userId);
        return Ok(ApiResponse<IEnumerable<TicketDto>>.Ok(tickets.Select(MapToDto)));
    }

    [HttpPost("purchase")]
    // [Authorize(Roles = "Attendee,Organizer,Admin")]
    public async Task<ActionResult<ApiResponse<TicketDto>>> Purchase([FromBody] PurchaseTicketDto dto)
    {
        var userId = GetRequesterId();
        var ticket = await _ticketService.PurchaseTicketAsync(userId, dto.Type, dto.ValidDate);
        return Ok(ApiResponse<TicketDto>.Ok(MapToDto(ticket), "Ticket purchased successfully."));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<TicketDto>>> Cancel(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.Fail("Ticket id is required."));
        }

        var requesterId = GetRequesterId();
        var ticket      = await _ticketService.CancelTicketAsync(id, requesterId);
        return Ok(ApiResponse<TicketDto>.Ok(MapToDto(ticket), "Ticket cancelled successfully."));
    }

    [HttpPost("scan")]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<TicketDto>>> Scan([FromQuery] string qrCode)
    {
        if (string.IsNullOrWhiteSpace(qrCode))
        {
            return BadRequest(ApiResponse<object>.Fail("qrCode is required."));
        }

        var ticket = await _ticketService.ScanTicketAsync(qrCode);
        return Ok(ApiResponse<TicketDto>.Ok(MapToDto(ticket), "Ticket scanned successfully."));
    }

    private Guid GetRequesterId() 
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(sub))
        {
            var claims = string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"));
            var authHeader = Request.Headers["Authorization"].ToString();
            throw new Exception($"DEBUG INFO - IsAuth: {User.Identity?.IsAuthenticated}, Claims: [{claims}], AuthHeader: {(string.IsNullOrEmpty(authHeader) ? "EMPTY" : "PRESENT")}");
        }
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }

    private static TicketDto MapToDto(Ticket t) => new()
    {
        TicketId    = t.TicketId,
        UserId      = t.UserId,
        Type        = t.Type,
        Status      = t.Status,
        QrCode      = t.QrCode,
        PricePaid   = t.PricePaid,
        PurchasedAt = t.PurchasedAt,
        ValidDate   = t.ValidDate,
    };
}
