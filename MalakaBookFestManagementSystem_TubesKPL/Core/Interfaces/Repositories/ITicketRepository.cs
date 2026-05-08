using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Core.Interfaces.Repositories;

public interface ITicketRepository : IRepository<Ticket>
{
    Task<IEnumerable<Ticket>> GetByUserIdAsync(Guid userId);
    Task<Ticket?> GetByQrCodeAsync(string qrCode);
    Task<int> CountActiveByUserIdAsync(Guid userId);
    Task<bool> UserHasTicketTypeAsync(Guid userId, TicketType type);
}
