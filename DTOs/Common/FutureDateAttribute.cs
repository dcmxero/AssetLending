using System.ComponentModel.DataAnnotations;

namespace DTOs.Common;

/// <summary>
/// Validation attribute that ensures a DateTime value is today or in the future (date-only comparison).
/// </summary>
public class FutureDateAttribute
    : ValidationAttribute
{
    /// <summary>
    /// Validates that the given value is a DateTime that is today or later.
    /// </summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime && dateTime.Date < DateTime.UtcNow.Date)
        {
            return new ValidationResult(
                ErrorMessage ?? $"{validationContext.DisplayName} must be today or a future date.");
        }

        return ValidationResult.Success;
    }
}
