namespace MalakaBookFest.Application.DTOs.Talkshow;

public class CreateTalkshowDto
{
    public string Title { get; set; } = string.Empty;
    public string SpeakerName { get; set; } = string.Empty;
    public string? SpeakerBio { get; set; }
    public string Venue { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxCapacity { get; set; }
}
