using Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitOfWork;

/// <summary>
/// Unit of work implementation that coordinates persistence of changes across repositories.
/// </summary>
public sealed class UnitOfWork(ApplicationDbContext context)
    : IUnitOfWork
{
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyConflictException(exception);
        }
    }
}
