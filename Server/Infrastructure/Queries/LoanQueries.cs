using Application.Abstractions.Queries;
using Domain.Enums;
using Domain.Models.AssetManagement;
using DTOs.Asset;
using DTOs.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Queries;

/// <summary>
/// Reads loan data and projects it into DTOs within the database query.
/// </summary>
public sealed class LoanQueries(ApplicationDbContext context, TimeProvider timeProvider)
    : ILoanQueries
{
    private static readonly Expression<Func<Loan, LoanDto>> ToDto = loan => new LoanDto
    {
        Id = loan.Id,
        AssetId = loan.AssetId,
        AssetName = loan.Asset.Name,
        BorrowedById = loan.BorrowedById,
        BorrowedByName = loan.BorrowedBy.FirstName + " " + loan.BorrowedBy.LastName,
        BorrowedAt = loan.BorrowedAt,
        DueDate = loan.DueDate,
        ReturnedAt = loan.ReturnedAt,
        Status = loan.Status.ToString()
    };

    public async Task<List<LoanDto>> GetActiveLoansAsync(CancellationToken cancellationToken = default)
    {
        return await context.Loans
            .Where(l => l.Status == LoanStatus.Active)
            .Select(ToDto)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        return await context.Loans
            .Where(l => l.Status == LoanStatus.Active && l.DueDate < now)
            .Select(ToDto)
            .ToListAsync(cancellationToken);
    }

    public Task<PaginatedList<LoanDto>> GetAllLoansAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return GetPageAsync(context.Loans, page, pageSize, cancellationToken);
    }

    public Task<PaginatedList<LoanDto>> GetLoansByAssetIdAsync(int assetId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return GetPageAsync(context.Loans.Where(l => l.AssetId == assetId), page, pageSize, cancellationToken);
    }

    private static async Task<PaginatedList<LoanDto>> GetPageAsync(IQueryable<Loan> query, int page, int pageSize, CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.BorrowedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToDto)
            .ToListAsync(cancellationToken);

        return new PaginatedList<LoanDto>
        {
            Data = items,
            TotalCount = totalCount,
            PageIndex = page,
            PageSize = pageSize
        };
    }
}
