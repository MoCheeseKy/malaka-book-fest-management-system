using MalakaBookFest.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MalakaBookFest.Infrastructure.Persistence.Repositories;

/// <summary>
/// Generic repository implementation backed by Entity Framework Core.
/// Provides base CRUD for any entity registered in AppDbContext.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id) =>
        await _dbSet.FindAsync(id);

    public virtual async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.ToListAsync();

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Entity with id {id} not found.");
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}