using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Application.DTOs.Booth;

public class CreateBoothDto
{
    public string BoothName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string BoothNumber { get; set; } = string.Empty;
    public BoothCategory Category { get; set; } = BoothCategory.Other;
}
