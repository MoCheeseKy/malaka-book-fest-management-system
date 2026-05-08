using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Core.Entities;

public class Booth
{
    public Guid BoothId { get; set; }
    public Guid OrganizerId { get; set; }
    public string BoothName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string BoothNumber { get; set; } = string.Empty;
    public BoothCategory Category { get; set; } = BoothCategory.Other;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User Organizer { get; set; } = null!;
    public ICollection<Book> Books { get; set; } = [];
}