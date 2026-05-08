using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MalakaBookFest.Infrastructure.Persistence.Repositories;

public class BoothRepository : Repository<Booth>, IBoothRepository
{
    public BoothRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Booth>> GetByOrganizerIdAsync(Guid organizerId) =>
        await _dbSet.Where(b => b.OrganizerId == organizerId).ToListAsync();

    public async Task<bool> BoothNumberExistsAsync(string boothNumber) =>
        await _dbSet.AnyAsync(b => b.BoothNumber == boothNumber);
}
