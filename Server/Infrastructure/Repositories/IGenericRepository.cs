namespace Infrastructure.Repositories;

/// <summary>
/// Generic repository interface providing basic CRUD operations for any entity type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Returns a queryable collection of all entities. Supports further query composition.
    /// </summary>
    /// <returns>An <see cref="IQueryable{TEntity}"/> for building queries.</returns>
    IQueryable<TEntity> GetAll();

    /// <summary>
    /// Retrieves an entity by its primary key.
    /// </summary>
    /// <param name="id">The primary key of the entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity to the context. Changes are not persisted until <see cref="UnitOfWork.IUnitOfWork.CompleteAsync"/> is called.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
}
