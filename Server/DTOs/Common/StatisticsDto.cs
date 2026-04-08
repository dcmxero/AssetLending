namespace DTOs.Common;

/// <summary>
/// Aggregated statistics about the lending system.
/// </summary>
public class StatisticsDto
{
    /// <summary>
    /// Total number of active assets.
    /// </summary>
    public int TotalAssets { get; init; }

    /// <summary>
    /// Total number of users.
    /// </summary>
    public int TotalUsers { get; init; }

    /// <summary>
    /// Number of currently active loans.
    /// </summary>
    public int ActiveLoans { get; init; }

    /// <summary>
    /// Number of overdue loans.
    /// </summary>
    public int OverdueLoans { get; init; }

    /// <summary>
    /// Number of active (non-cancelled, non-expired) reservations.
    /// </summary>
    public int ActiveReservations { get; init; }

    /// <summary>
    /// Name of the most borrowed asset.
    /// </summary>
    public string? MostBorrowedAssetName { get; init; }

    /// <summary>
    /// Number of times the most borrowed asset was borrowed.
    /// </summary>
    public int MostBorrowedAssetCount { get; init; }

    /// <summary>
    /// Name of the most active user (most loans).
    /// </summary>
    public string? MostActiveUserName { get; init; }

    /// <summary>
    /// Number of loans by the most active user.
    /// </summary>
    public int MostActiveUserLoanCount { get; init; }
}
