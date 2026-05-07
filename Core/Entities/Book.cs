namespace MalakaBookFest.Core.Entities;

public class Book
{
    public Guid BookId { get; set; }
    public Guid BoothId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? Isbn { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? CoverUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Booth Booth { get; set; } = null!;
}