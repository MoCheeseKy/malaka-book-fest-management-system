using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Application.DTOs.Talkshow;

public class TalkshowDto
{
    public Guid TalkshowId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SpeakerName { get; set; } = string.Empty;
    public string? SpeakerBio { get; set; }
    public string Venue { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxCapacity { get; set; }
    public int RegisteredCount { get; set; }
    public TalkshowStatus Status { get; set; }
}
