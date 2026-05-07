using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Core.Entities;

public class Talkshow
{
    public Guid TalkshowId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SpeakerName { get; set; } = string.Empty;
    public string? SpeakerBio { get; set; }
    public string Venue { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxCapacity { get; set; }
    public TalkshowStatus Status { get; set; } = TalkshowStatus.Scheduled;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TalkshowRegistration> Registrations { get; set; } = [];
}