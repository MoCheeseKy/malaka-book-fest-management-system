namespace MalakaBookFest.Application.DTOs.Book;

public class UpdateBookDto
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Isbn { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    public string? CoverUrl { get; set; }
}
