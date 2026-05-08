namespace MalakaBookFest.Core.Interfaces.Repositories;

/// <summary>
/// Generic repository interface providing base CRUD operations for any entity type.
/// </summary>
/// <typeparam name="T">The entity type. Must be a reference type.</typeparam>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
