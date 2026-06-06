using MalakaBookFest.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace MalakaBookFest.Application.DTOs.Booth;

public class CreateBoothDto
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string BoothName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    public string BoothNumber { get; set; } = string.Empty;

    public BoothCategory Category { get; set; } = BoothCategory.Other;
}
