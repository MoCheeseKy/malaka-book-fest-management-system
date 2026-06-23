using MalakaBookFest.Application.Common;
using MalakaBookFest.Application.DTOs.Book;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MalakaBookFest.API.Controllers;

[ApiController]
[Route("api/booths/{boothId:guid}/books")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<BookDto>>>> GetByBooth(Guid boothId)
    {
        if (boothId == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.Fail("Booth id is required."));
        }

        var books = await _bookService.GetBooksByBoothAsync(boothId);
        return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(books.Select(MapToDto)));
    }

    [HttpPost]
    // [Authorize(Roles = "Organizer,Admin")]
    public async Task<ActionResult<ApiResponse<BookDto>>> AddBook(
        Guid boothId, [FromBody] CreateBookDto dto)
    {
        if (boothId == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.Fail("Booth id is required."));
        }

        var requesterId = GetRequesterId();

        var book = new Book
        {
            BoothId  = boothId,
            Title    = dto.Title,
            Author   = dto.Author,
            Isbn     = dto.Isbn,
            Price    = dto.Price,
            Stock    = dto.Stock,
            CoverUrl = dto.CoverUrl,
        };

        var created = await _bookService.AddBookAsync(book, requesterId);
        return Ok(ApiResponse<BookDto>.Ok(MapToDto(created), "Book added successfully."));
    }

    [HttpPut("{bookId:guid}")]
    // [Authorize(Roles = "Organizer,Admin")]
    public async Task<ActionResult<ApiResponse<BookDto>>> UpdateBook(
        Guid boothId, Guid bookId, [FromBody] UpdateBookDto dto)
    {
        if (boothId == Guid.Empty || bookId == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.Fail("Booth id and book id are required."));
        }

        var requesterId = GetRequesterId();
        var existing    = await _bookService.GetBookByIdAsync(bookId);

        existing.Title    = dto.Title    ?? existing.Title;
        existing.Author   = dto.Author   ?? existing.Author;
        existing.Isbn     = dto.Isbn     ?? existing.Isbn;
        existing.Price    = dto.Price    ?? existing.Price;
        existing.Stock    = dto.Stock    ?? existing.Stock;
        existing.CoverUrl = dto.CoverUrl ?? existing.CoverUrl;

        var updated = await _bookService.UpdateBookAsync(bookId, existing, requesterId);
        return Ok(ApiResponse<BookDto>.Ok(MapToDto(updated), "Book updated successfully."));
    }

    [HttpDelete("{bookId:guid}")]
    // [Authorize(Roles = "Organizer,Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteBook(Guid boothId, Guid bookId)
    {
        if (boothId == Guid.Empty || bookId == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.Fail("Booth id and book id are required."));
        }

        var requesterId = GetRequesterId();
        await _bookService.DeleteBookAsync(bookId, requesterId);
        return Ok(ApiResponse<object>.Ok(null!, "Book deleted successfully."));
    }

    private Guid GetRequesterId() => Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static BookDto MapToDto(Book b) => new()
    {
        BookId   = b.BookId,
        BoothId  = b.BoothId,
        Title    = b.Title,
        Author   = b.Author,
        Isbn     = b.Isbn,
        Price    = b.Price,
        Stock    = b.Stock,
        CoverUrl = b.CoverUrl,
    };
}
