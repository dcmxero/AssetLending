using Domain.Models.AssetManagement;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository interface for loan-specific data access operations.
/// </summary>
public interface ILoanRepository
    : IGenericRepository<Loan>
{
    /// <summary>
    /// Retrieves all loans with <see cref="Domain.Enums.LoanStatus.Active"/> status, including related asset and user data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of active loans.</returns>
    Task<List<Loan>> GetActiveLoansAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the active loan for a specific asset, if one exists.
    /// </summary>
    /// <param name="assetId">The identifier of the asset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The active loan if found; otherwise, null.</returns>
    Task<Loan?> GetActiveLoanByAssetIdAsync(int assetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of all loans, including related asset and user data.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the list of loans and total count.</returns>
    Task<(List<Loan> Items, int TotalCount)> GetAllLoansAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all overdue loans (active loans past their due date), including related asset and user data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of overdue loans.</returns>
    Task<List<Loan>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of loans for a specific asset, including related asset and user data.
    /// </summary>
    /// <param name="assetId">The identifier of the asset.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the list of loans and total count.</returns>
    Task<(List<Loan> Items, int TotalCount)> GetLoansByAssetIdAsync(int assetId, int page, int pageSize, CancellationToken cancellationToken = default);
}
