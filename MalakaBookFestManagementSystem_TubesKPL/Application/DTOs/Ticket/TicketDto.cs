using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Application.DTOs.Ticket;

public class TicketDto
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public TicketType Type { get; set; }
    public TicketStatus Status { get; set; }
    public string? QrCode { get; set; }
    public decimal PricePaid { get; set; }
    public DateTime PurchasedAt { get; set; }
    public DateOnly ValidDate { get; set; }
}
