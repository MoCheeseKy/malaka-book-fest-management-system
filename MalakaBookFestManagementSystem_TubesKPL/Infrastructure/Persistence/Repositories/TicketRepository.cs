using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Enums;
using MalakaBookFest.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MalakaBookFest.Infrastructure.Persistence.Repositories;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Ticket>> GetByUserIdAsync(Guid userId) =>
        await _dbSet.Where(t => t.UserId == userId).ToListAsync();

    public async Task<Ticket?> GetByQrCodeAsync(string qrCode) =>
        await _dbSet.FirstOrDefaultAsync(t => t.QrCode == qrCode);

    public async Task<int> CountActiveByUserIdAsync(Guid userId) =>
        await _dbSet.CountAsync(t => t.UserId == userId && t.Status == TicketStatus.Active);

    public async Task<bool> UserHasTicketTypeAsync(Guid userId, TicketType type) =>
        await _dbSet.AnyAsync(t =>
            t.UserId == userId &&
            t.Type == type &&
            t.Status == TicketStatus.Active);
}
