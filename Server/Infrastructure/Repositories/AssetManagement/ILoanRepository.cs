using Domain.Models.AssetManagement;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository interface for loan write operations.
/// </summary>
public interface ILoanRepository
    : IGenericRepository<Loan>
{
    /// <summary>
    /// Retrieves the active loan for a specific asset, if one exists.
    /// </summary>
    /// <param name="assetId">The identifier of the asset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The active loan if found; otherwise, null.</returns>
    Task<Loan?> GetActiveLoanByAssetIdAsync(int assetId, CancellationToken cancellationToken = default);
}
