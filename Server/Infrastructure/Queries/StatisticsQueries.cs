using Domain.Enums;
using DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries;

/// <summary>
/// Computes aggregated lending system statistics directly against the database.
/// </summary>
public sealed class StatisticsQueries(ApplicationDbContext context)
    : IStatisticsQueries
{
    public async Task<StatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var totalAssets = await context.Assets.CountAsync(a => a.IsActive, cancellationToken);
        var totalUsers = await context.Users.CountAsync(cancellationToken);
        var activeLoans = await context.Loans.CountAsync(l => l.Status == LoanStatus.Active, cancellationToken);
        var overdueLoans = await context.Loans.CountAsync(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow, cancellationToken);
        var activeReservations = await context.Reservations.CountAsync(r => !r.IsCancelled && r.ReservedUntil >= DateTime.UtcNow, cancellationToken);

        var mostBorrowedAsset = await context.Loans
            .GroupBy(l => l.AssetId)
            .Select(g => new { AssetId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefaultAsync(cancellationToken);

        string? mostBorrowedAssetName = null;
        var mostBorrowedAssetCount = 0;
        if (mostBorrowedAsset is not null)
        {
            mostBorrowedAssetCount = mostBorrowedAsset.Count;
            mostBorrowedAssetName = await context.Assets
                .Where(a => a.Id == mostBorrowedAsset.AssetId)
                .Select(a => a.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var mostActiveUser = await context.Loans
            .GroupBy(l => l.BorrowedById)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefaultAsync(cancellationToken);

        string? mostActiveUserName = null;
        var mostActiveUserLoanCount = 0;
        if (mostActiveUser is not null)
        {
            mostActiveUserLoanCount = mostActiveUser.Count;
            mostActiveUserName = await context.Users
                .Where(u => u.Id == mostActiveUser.UserId)
                .Select(u => u.FirstName + " " + u.LastName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new StatisticsDto
        {
            TotalAssets = totalAssets,
            TotalUsers = totalUsers,
            ActiveLoans = activeLoans,
            OverdueLoans = overdueLoans,
            ActiveReservations = activeReservations,
            MostBorrowedAssetName = mostBorrowedAssetName,
            MostBorrowedAssetCount = mostBorrowedAssetCount,
            MostActiveUserName = mostActiveUserName,
            MostActiveUserLoanCount = mostActiveUserLoanCount
        };
    }
}
