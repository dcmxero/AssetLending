using Domain.Enums;
using Domain.Models.AssetManagement;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository for loan-specific data access operations.
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

    public async Task<List<Loan>> GetActiveLoansAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Loans
            .Include(l => l.Asset)
            .Include(l => l.BorrowedBy)
            .Where(l => l.Status == LoanStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<Loan?> GetActiveLoanByAssetIdAsync(int assetId, CancellationToken cancellationToken = default)
    {
        return await Context.Loans
            .FirstOrDefaultAsync(l => l.AssetId == assetId && l.Status == LoanStatus.Active, cancellationToken);
    }

    public async Task<(List<Loan> Items, int TotalCount)> GetAllLoansAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = Context.Loans
            .Include(l => l.Asset)
            .Include(l => l.BorrowedBy);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.BorrowedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return ([.. items], totalCount);
    }

    public async Task<List<Loan>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var query = Context.Loans
            .Include(l => l.Asset)
            .Include(l => l.BorrowedBy)
            .Where(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow);

        return [.. await query.ToListAsync(cancellationToken)];
    }

    public async Task<(List<Loan> Items, int TotalCount)> GetLoansByAssetIdAsync(int assetId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = Context.Loans
            .Include(l => l.Asset)
            .Include(l => l.BorrowedBy)
            .Where(l => l.AssetId == assetId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.BorrowedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return ([.. items], totalCount);
    }
}
