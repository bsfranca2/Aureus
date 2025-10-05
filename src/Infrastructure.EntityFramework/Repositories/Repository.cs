using Bsfranca2.Core;

using Microsoft.EntityFrameworkCore;

namespace Aureus.Infrastructure.EntityFramework.Repositories;

public class Repository<T, TId, TDb> : IRepository<T, TId>
    where T : class, IEntity<TId>
    where TId : IEquatable<TId>
    where TDb : DbContext
{
    private bool _disposed = false;
    
    protected readonly TDb DbContext;
    protected readonly DbSet<T> DbSet;

    protected Repository(TDb db)
    {
        DbContext = db;
        DbSet = db.Set<T>();
    }

    public async Task<T> CreateAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        await DbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<T?> GetByIdAsync(TId id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        var original = await DbSet.FindAsync(entity.Id);
        if (original != null)
        {
            DbContext.Entry(original).CurrentValues.SetValues(entity);
            await DbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(T entity)
    {
        DbContext.Remove(entity);
        await DbContext.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                DbContext?.Dispose();
            }
            
            _disposed = true;
        }
    }
}