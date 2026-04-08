using Domain.Common;
using DTOs.Asset;
using DTOs.Common;

namespace Application.Services;

/// <summary>
/// Service interface for loan (checkout/return) operations.
/// </summary>
public interface ILoanService
{
    /// <summary>
    /// Retrieves all currently active loans.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of active loans with asset and user details.</returns>
    Task<List<LoanDto>> GetActiveLoansAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of all loans.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of loans.</returns>
    Task<PaginatedList<LoanDto>> GetAllLoansAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all overdue loans (active loans past their due date).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of overdue loans.</returns>
    Task<List<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new loan (checks out an asset to a user).
    /// </summary>
    /// <param name="dto">The loan creation data containing asset and user identifiers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success with created loan; or failure if asset/user not found or asset not available.</returns>
    Task<Result<LoanDto>> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a loaned asset, marking the loan as returned and the asset as available.
    /// </summary>
    /// <param name="loanId">The identifier of the loan to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success with updated loan; or failure if loan not found or already returned.</returns>
    Task<Result<LoanDto>> ReturnAssetAsync(int loanId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of loans for a specific asset.
    /// </summary>
    /// <param name="assetId">The identifier of the asset.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of loans for the specified asset.</returns>
    Task<PaginatedList<LoanDto>> GetLoansByAssetIdAsync(int assetId, int page, int pageSize, CancellationToken cancellationToken = default);
}
