using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MalakaBookFest.Infrastructure.Persistence.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Book>> GetByBoothIdAsync(Guid boothId) =>
        await _dbSet.Where(b => b.BoothId == boothId).ToListAsync();
}
