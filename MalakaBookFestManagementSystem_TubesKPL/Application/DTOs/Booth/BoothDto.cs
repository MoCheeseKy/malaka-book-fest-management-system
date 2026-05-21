using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Application.DTOs.Booth;

public class BoothDto
{
    public Guid BoothId { get; set; }
    public Guid OrganizerId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public string BoothName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string BoothNumber { get; set; } = string.Empty;
    public BoothCategory Category { get; set; }
    public bool IsActive { get; set; }
}
