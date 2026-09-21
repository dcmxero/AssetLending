namespace Application.Abstractions.Persistence;

/// <summary>
/// Saves the changes accumulated by the repositories as a single unit.
/// </summary>
/// <remarks>
/// This is the application layer's only way to reach persistence, which is what keeps
/// the layer free of a reference to the data access technology behind it.
/// </remarks>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ConcurrencyConflictException">Another user modified the same data first.</exception>
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
