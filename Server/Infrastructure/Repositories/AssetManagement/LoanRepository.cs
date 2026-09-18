using Domain.Enums;
using Domain.Models.AssetManagement;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository for loan write operations.
/// </summary>
public sealed class LoanRepository(ApplicationDbContext context)
    : GenericRepository<Loan>(context), ILoanRepository
{
    public override async Task<Loan?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Loans
            .Include(l => l.Asset)
            .Include(l => l.BorrowedBy)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<Loan?> GetActiveLoanByAssetIdAsync(int assetId, CancellationToken cancellationToken = default)
    {
        return await Context.Loans
            .FirstOrDefaultAsync(l => l.AssetId == assetId && l.Status == LoanStatus.Active, cancellationToken);
    }
}
