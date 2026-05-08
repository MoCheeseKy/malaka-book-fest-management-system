using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Core.Entities;

public class Ticket
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public TicketType Type { get; set; } = TicketType.SingleDay;
    public TicketStatus Status { get; set; } = TicketStatus.Active;
    public string? QrCode { get; set; }
    public decimal PricePaid { get; set; }
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    public DateOnly ValidDate { get; set; }

    public User User { get; set; } = null!;
}