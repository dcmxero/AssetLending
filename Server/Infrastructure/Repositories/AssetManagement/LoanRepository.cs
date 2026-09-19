using Domain.Models.AssetManagement;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.AssetManagement;

/// <summary>
/// Repository for loan write operations.
/// </summary>
public sealed class LoanRepository(ApplicationDbContext context)
    : ILoanRepository
{
    public async Task<Loan?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Loans
            .Include(l => l.Asset)
            .Include(l => l.BorrowedBy)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task AddAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        await context.Loans.AddAsync(loan, cancellationToken);
    }
}
