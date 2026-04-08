using Domain.Enums;
using DTOs.Common;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Service responsible for computing aggregated lending system statistics.
/// </summary>
public sealed class StatisticsService(
    ApplicationDbContext context,
    ILogger<StatisticsService> logger)
    : IStatisticsService
{
    /// <inheritdoc />
    public async Task<StatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Computing system statistics");

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
            var asset = await context.Assets.FindAsync([mostBorrowedAsset.AssetId], cancellationToken);
            if (asset is not null)
            {
                mostBorrowedAssetName = asset.Name;
            }
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
            var user = await context.Users.FindAsync([mostActiveUser.UserId], cancellationToken);
            if (user is not null)
            {
                mostActiveUserName = user.FullName;
            }
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
