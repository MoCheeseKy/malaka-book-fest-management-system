using MalakaBookFest.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MalakaBookFest.Application.DTOs.Ticket;

public class PurchaseTicketDto : IValidatableObject
{
    [Required]
    [EnumDataType(typeof(TicketType))]
    public TicketType Type { get; set; }

    [Required]
    public DateOnly ValidDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (ValidDate < today)
        {
            yield return new ValidationResult("ValidDate cannot be in the past.", new[] { nameof(ValidDate) });
        }
    }
}
