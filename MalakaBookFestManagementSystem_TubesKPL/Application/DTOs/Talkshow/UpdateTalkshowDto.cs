namespace MalakaBookFest.Application.DTOs.Talkshow;

using System.ComponentModel.DataAnnotations;

public class UpdateTalkshowDto
{
    [StringLength(250, MinimumLength = 2)]
    public string? Title { get; set; }

    [StringLength(150, MinimumLength = 2)]
    public string? SpeakerName { get; set; }

    [StringLength(2000)]
    public string? SpeakerBio { get; set; }

    [StringLength(250)]
    public string? Venue { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    [Range(1, 100000)]
    public int? MaxCapacity { get; set; }
}
