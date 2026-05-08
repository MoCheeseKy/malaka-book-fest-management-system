using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MalakaBookFest.Infrastructure.Persistence.Repositories;

public class TalkshowRegistrationRepository : Repository<TalkshowRegistration>, ITalkshowRegistrationRepository
{
    public TalkshowRegistrationRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<TalkshowRegistration>> GetByUserIdAsync(Guid userId) =>
        await _dbSet.Where(r => r.UserId == userId).Include(r => r.Talkshow).ToListAsync();

    public async Task<bool> IsAlreadyRegisteredAsync(Guid userId, Guid talkshowId) =>
        await _dbSet.AnyAsync(r => r.UserId == userId && r.TalkshowId == talkshowId);
}
