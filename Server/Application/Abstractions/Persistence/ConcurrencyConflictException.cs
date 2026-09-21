namespace Application.Abstractions.Persistence;

/// <summary>
/// Thrown when pending changes could not be saved because another user modified the same data first.
/// </summary>
public sealed class ConcurrencyConflictException(Exception innerException)
    : Exception("The data was modified by another user.", innerException);
