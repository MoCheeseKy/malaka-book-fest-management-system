namespace MalakaBookFest.Application.DTOs.Book;

public class BookDto
{
    public Guid BookId { get; set; }
    public Guid BoothId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? Isbn { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? CoverUrl { get; set; }
}
