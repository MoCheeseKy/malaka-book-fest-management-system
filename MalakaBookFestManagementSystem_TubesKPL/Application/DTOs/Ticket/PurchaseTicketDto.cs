using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Application.DTOs.Ticket;

public class PurchaseTicketDto
{
    public TicketType Type { get; set; }
    public DateOnly ValidDate { get; set; }
}
