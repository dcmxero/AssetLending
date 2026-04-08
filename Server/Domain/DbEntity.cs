namespace Domain;

/// <summary>
/// Base class for all database entities, providing a common primary key.
/// </summary>
public abstract class DbEntity
{
    /// <summary>
    /// Unique identifier of the entity.
    /// </summary>
    public int Id { get; set; }
}
