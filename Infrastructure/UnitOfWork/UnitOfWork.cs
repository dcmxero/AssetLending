namespace Infrastructure.UnitOfWork;

/// <summary>
/// Unit of work implementation that coordinates persistence of changes across repositories.
/// </summary>
public sealed class UnitOfWork(ApplicationDbContext context)
    : IUnitOfWork
{
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
