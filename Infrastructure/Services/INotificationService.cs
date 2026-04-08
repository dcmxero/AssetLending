namespace Infrastructure.Services;

/// <summary>
/// Service interface for sending notifications about loan and reservation events.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Notifies a user that their loan is due soon.
    /// </summary>
    /// <param name="loanId">The identifier of the loan.</param>
    /// <param name="assetName">The name of the borrowed asset.</param>
    /// <param name="userName">The name of the borrower.</param>
    /// <param name="dueDate">The due date of the loan.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task NotifyLoanDueSoonAsync(int loanId, string assetName, string userName, DateTime dueDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Notifies a user that their reservation is expiring soon.
    /// </summary>
    /// <param name="reservationId">The identifier of the reservation.</param>
    /// <param name="assetName">The name of the reserved asset.</param>
    /// <param name="userName">The name of the user who reserved.</param>
    /// <param name="reservedUntil">The expiration date of the reservation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task NotifyReservationExpiringSoonAsync(int reservationId, string assetName, string userName, DateTime reservedUntil, CancellationToken cancellationToken = default);
}
