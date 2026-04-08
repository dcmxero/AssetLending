using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Generic repository providing basic CRUD operations for entity types.
/// </summary>
public class GenericRepository<TEntity>(ApplicationDbContext context)
    : IGenericRepository<TEntity>
    where TEntity : class
{
    protected readonly ApplicationDbContext Context = context;

    public IQueryable<TEntity> GetAll()
    {
        return Context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }
}
