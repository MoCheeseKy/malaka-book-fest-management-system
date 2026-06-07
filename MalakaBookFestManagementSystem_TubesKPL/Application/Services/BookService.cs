using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Enums;
using MalakaBookFest.Core.Interfaces.Repositories;
using MalakaBookFest.Core.Interfaces.Services;
namespace MalakaBookFest.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBoothRepository _boothRepository;
    private readonly IUserRepository _userRepository;
    public BookService(
        IBookRepository bookRepository,
        IBoothRepository boothRepository,
        IUserRepository userRepository)
    {
        _bookRepository = bookRepository;
        _boothRepository = boothRepository;
        _userRepository = userRepository;
    }
    public async Task<IEnumerable<Book>> GetBooksByBoothAsync(Guid boothId) =>
        await _bookRepository.GetByBoothIdAsync(boothId);
    public async Task<Book> GetBookByIdAsync(Guid bookId) =>
        await _bookRepository.GetByIdAsync(bookId)
            ?? throw new KeyNotFoundException($"Book {bookId} not found.");
    public async Task<Book> AddBookAsync(Book book, Guid requesterId)
    {
        var booth = await _boothRepository.GetByIdAsync(book.BoothId)
            ?? throw new KeyNotFoundException("Booth not found.");
        var requester = await _userRepository.GetByIdAsync(requesterId)
            ?? throw new KeyNotFoundException("Requester not found.");
        if (requester.Role != UserRole.Admin && booth.OrganizerId != requesterId)
            throw new UnauthorizedAccessException("You can only add books to your own booth.");
        book.BookId = Guid.NewGuid();
        book.CreatedAt = DateTime.UtcNow;
        book.UpdatedAt = DateTime.UtcNow;
        return await _bookRepository.CreateAsync(book);
    }
    public async Task<Book> UpdateBookAsync(Guid bookId, Book updated, Guid requesterId)
    {
        var book = await _bookRepository.GetByIdAsync(bookId)
            ?? throw new KeyNotFoundException($"Book {bookId} not found.");
        var booth = await _boothRepository.GetByIdAsync(book.BoothId)
            ?? throw new KeyNotFoundException("Booth not found.");
        var requester = await _userRepository.GetByIdAsync(requesterId)
            ?? throw new KeyNotFoundException("Requester not found.");
        if (requester.Role != UserRole.Admin && booth.OrganizerId != requesterId)
            throw new UnauthorizedAccessException("You can only update books in your own booth.");
        book.Title = updated.Title;
        book.Author = updated.Author;
        book.Isbn = updated.Isbn;
        book.Price = updated.Price;
        book.Stock = updated.Stock;
        book.CoverUrl = updated.CoverUrl;
        book.UpdatedAt = DateTime.UtcNow;
        return await _bookRepository.UpdateAsync(book);
    }
    public async Task DeleteBookAsync(Guid bookId, Guid requesterId)
    {
        var book = await _bookRepository.GetByIdAsync(bookId)
            ?? throw new KeyNotFoundException($"Book {bookId} not found.");
        var booth = await _boothRepository.GetByIdAsync(book.BoothId)
            ?? throw new KeyNotFoundException("Booth not found.");
        var requester = await _userRepository.GetByIdAsync(requesterId)
            ?? throw new KeyNotFoundException("Requester not found.");
        if (requester.Role != UserRole.Admin && booth.OrganizerId != requesterId)
            throw new UnauthorizedAccessException("You can only delete books from your own booth.");
        await _bookRepository.DeleteAsync(bookId);
    }
}