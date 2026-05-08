using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Core.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Guest;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Ticket> Tickets { get; set; } = [];
    public ICollection<TalkshowRegistration> TalkshowRegistrations { get; set; } = [];
    public ICollection<Booth> ManagedBooths { get; set; } = [];
}
