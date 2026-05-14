using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Enums;
using MalakaBookFest.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MalakaBookFest.Infrastructure.Persistence.Repositories;

public class TalkshowRepository : Repository<Talkshow>, ITalkshowRepository
{
    public TalkshowRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Talkshow>> GetUpcomingAsync() =>
        await _dbSet
            .Where(t => t.Status == TalkshowStatus.Scheduled && t.StartTime > DateTime.UtcNow)
            .OrderBy(t => t.StartTime)
            .ToListAsync();

    public async Task<int> GetRegistrationCountAsync(Guid talkshowId) =>
        await _context.TalkshowRegistrations.CountAsync(r => r.TalkshowId == talkshowId);
}
