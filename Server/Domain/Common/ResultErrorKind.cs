namespace Domain.Common;

/// <summary>
/// Classifies why an operation failed, so that callers can react to the category
/// of failure rather than to the wording of its message.
/// </summary>
public enum ResultErrorKind
{
    /// <summary>
    /// The operation was rejected because it would violate a business rule.
    /// </summary>
    RuleViolation,

    /// <summary>
    /// The operation targeted something that does not exist.
    /// </summary>
    NotFound,

    /// <summary>
    /// The operation lost a race with another user who changed the same data first.
    /// </summary>
    ConcurrencyConflict
}
