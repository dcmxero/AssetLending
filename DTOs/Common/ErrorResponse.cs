namespace DTOs.Common;

/// <summary>
/// Standardized error response returned by the API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error message describing what went wrong.
    /// </summary>
    public required string Error { get; init; }

    /// <summary>
    /// Optional additional details about the error.
    /// </summary>
    public string? Details { get; init; }
}
