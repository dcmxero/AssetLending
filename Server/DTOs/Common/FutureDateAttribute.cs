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
    /// <remarks>
    /// The current date comes from the <see cref="TimeProvider"/> registered for the request,
    /// so validation can be exercised at a chosen instant rather than only at the real one.
    /// </remarks>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var timeProvider = validationContext.GetService(typeof(TimeProvider)) as TimeProvider ?? TimeProvider.System;

        if (value is DateTime dateTime && dateTime.Date < timeProvider.GetUtcNow().UtcDateTime.Date)
        {
            return new ValidationResult(
                ErrorMessage ?? $"{validationContext.DisplayName} must be today or a future date.");
        }

        return ValidationResult.Success;
    }
}
