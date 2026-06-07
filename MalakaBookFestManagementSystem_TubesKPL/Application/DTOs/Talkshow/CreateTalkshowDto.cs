namespace MalakaBookFest.Application.DTOs.Talkshow;

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class CreateTalkshowDto : IValidatableObject
{
    [Required]
    [StringLength(250, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string SpeakerName { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? SpeakerBio { get; set; }

    [Required]
    [StringLength(250)]
    public string Venue { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Range(1, 100000)]
    public int MaxCapacity { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult("EndTime must be after StartTime.", new[] { nameof(EndTime), nameof(StartTime) });
        }

        if (StartTime == default || EndTime == default)
        {
            yield break;
        }
    }
}
