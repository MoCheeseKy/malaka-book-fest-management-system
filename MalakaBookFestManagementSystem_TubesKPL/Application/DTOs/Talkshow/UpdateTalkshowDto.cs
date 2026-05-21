namespace MalakaBookFest.Application.DTOs.Talkshow;

public class UpdateTalkshowDto
{
    public string? Title { get; set; }
    public string? SpeakerName { get; set; }
    public string? SpeakerBio { get; set; }
    public string? Venue { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? MaxCapacity { get; set; }
}
