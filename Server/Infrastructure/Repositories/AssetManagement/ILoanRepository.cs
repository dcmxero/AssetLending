using Domain.Models.AssetManagement;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository interface for loan write operations.
/// </summary>
public interface ILoanRepository
{
    /// <summary>
    /// Retrieves a loan by its identifier, together with the asset and borrower its domain methods act on.
    /// </summary>
    /// <param name="id">The identifier of the loan.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The loan if found; otherwise, null.</returns>
    Task<Loan?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new loan to the context. Changes are not persisted until <see cref="UnitOfWork.IUnitOfWork.CompleteAsync"/> is called.
    /// </summary>
    /// <param name="loan">The loan to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Loan loan, CancellationToken cancellationToken = default);
}
