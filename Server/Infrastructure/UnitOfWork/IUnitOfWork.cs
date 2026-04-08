namespace Infrastructure.UnitOfWork;

/// <summary>
/// Unit of Work interface for persisting all pending changes in a single transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
