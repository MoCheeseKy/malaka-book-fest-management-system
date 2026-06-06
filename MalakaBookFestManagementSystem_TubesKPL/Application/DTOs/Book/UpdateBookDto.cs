namespace MalakaBookFest.Application.DTOs.Book;

using System.ComponentModel.DataAnnotations;

public class UpdateBookDto
{
    [StringLength(200, MinimumLength = 2)]
    public string? Title { get; set; }

    [StringLength(120, MinimumLength = 2)]
    public string? Author { get; set; }

    [StringLength(25)]
    [RegularExpression(@"^(97(8|9))?\d{9}(\d|X)$", ErrorMessage = "ISBN format is invalid.")]
    public string? Isbn { get; set; }

    [Range(0.01, 1000000000)]
    public decimal? Price { get; set; }

    [Range(0, 1000000)]
    public int? Stock { get; set; }

    [Url]
    [StringLength(500)]
    public string? CoverUrl { get; set; }
}
