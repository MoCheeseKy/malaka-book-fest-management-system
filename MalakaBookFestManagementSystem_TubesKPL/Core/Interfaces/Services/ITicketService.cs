using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Core.Interfaces.Services;

public interface ITicketService
{
    Task<Ticket> PurchaseTicketAsync(Guid userId, TicketType type, DateOnly validDate);
    Task<IEnumerable<Ticket>> GetTicketsByUserAsync(Guid userId);
    Task<Ticket> ScanTicketAsync(string qrCode);
    Task<Ticket> CancelTicketAsync(Guid ticketId, Guid requesterId);
    Task ExpireTicketsAsync();
}
