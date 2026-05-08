using MalakaBookFest.Core.Entities;

namespace MalakaBookFest.Core.Interfaces.Services;

public interface IBookService
{
    Task<IEnumerable<Book>> GetBooksByBoothAsync(Guid boothId);
    Task<Book> GetBookByIdAsync(Guid bookId);
    Task<Book> AddBookAsync(Book book, Guid requesterId);
    Task<Book> UpdateBookAsync(Guid bookId, Book updated, Guid requesterId);
    Task DeleteBookAsync(Guid bookId, Guid requesterId);
}
