using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Application.DTOs.Booth;

public class UpdateBoothDto
{
    public string? BoothName { get; set; }
    public string? Description { get; set; }
    public BoothCategory? Category { get; set; }
    public bool? IsActive { get; set; }
}
