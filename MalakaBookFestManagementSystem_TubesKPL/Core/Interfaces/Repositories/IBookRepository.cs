using MalakaBookFest.Core.Entities;

namespace MalakaBookFest.Core.Interfaces.Repositories;

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetByBoothIdAsync(Guid boothId);
}
