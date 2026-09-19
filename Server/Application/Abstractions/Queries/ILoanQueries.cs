using DTOs.Asset;
using DTOs.Common;

namespace Application.Abstractions.Queries;

/// <summary>
/// Read-side contract for loan data.
/// </summary>
public interface ILoanQueries
{
    /// <summary>
    /// Retrieves all loans that are currently active.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of active loans.</returns>
    Task<List<LoanDto>> GetActiveLoansAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all active loans that are past their due date.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of overdue loans.</returns>
    Task<List<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated page of all loans, newest first.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A page of loans.</returns>
    Task<PaginatedList<LoanDto>> GetAllLoansAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated page of loans for a specific asset, newest first.
    /// </summary>
    /// <param name="assetId">The identifier of the asset.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A page of loans.</returns>
    Task<PaginatedList<LoanDto>> GetLoansByAssetIdAsync(int assetId, int page, int pageSize, CancellationToken cancellationToken = default);
}
