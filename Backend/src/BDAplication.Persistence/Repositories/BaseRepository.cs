using BDAplication.Domain.Common;
using BDAplication.Domain.Interfaces;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories;

public class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _db;

    public BaseRepository(ApplicationDbContext db) => _db = db;

    public virtual async Task<IEnumerable<T>> GetAllAsync() =>
        await _db.Set<T>().ToListAsync();

    public virtual async Task<T?> GetByIdAsync(int id) =>
        await _db.Set<T>().FindAsync(id);

    public virtual async Task<T> CreateAsync(T entity)
    {
        _db.Set<T>().Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _db.Set<T>().Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is null) return false;
        _db.Set<T>().Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}
