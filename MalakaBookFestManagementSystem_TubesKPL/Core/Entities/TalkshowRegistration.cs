namespace MalakaBookFest.Core.Entities;

public class TalkshowRegistration
{
    public Guid RegistrationId { get; set; }
    public Guid UserId { get; set; }
    public Guid TalkshowId { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public string? SeatCode { get; set; }

    public User User { get; set; } = null!;
    public Talkshow Talkshow { get; set; } = null!;
}