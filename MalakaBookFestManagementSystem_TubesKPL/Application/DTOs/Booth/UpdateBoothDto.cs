using MalakaBookFest.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace MalakaBookFest.Application.DTOs.Booth;

public class UpdateBoothDto
{
    [StringLength(150, MinimumLength = 2)]
    public string? BoothName { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public BoothCategory? Category { get; set; }

    public bool? IsActive { get; set; }
}
