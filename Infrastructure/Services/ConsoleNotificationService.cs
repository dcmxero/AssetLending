using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Console-based implementation of <see cref="INotificationService"/> that logs notifications.
/// </summary>
public sealed class ConsoleNotificationService(ILogger<ConsoleNotificationService> logger)
    : INotificationService
{
    /// <inheritdoc />
    public Task NotifyLoanDueSoonAsync(int loanId, string assetName, string userName, DateTime dueDate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "NOTIFICATION: Loan {LoanId} for asset '{AssetName}' borrowed by '{UserName}' is due on {DueDate:yyyy-MM-dd}",
            loanId, assetName, userName, dueDate);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task NotifyReservationExpiringSoonAsync(int reservationId, string assetName, string userName, DateTime reservedUntil, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "NOTIFICATION: Reservation {ReservationId} for asset '{AssetName}' by '{UserName}' expires on {ReservedUntil:yyyy-MM-dd}",
            reservationId, assetName, userName, reservedUntil);
        return Task.CompletedTask;
    }
}
