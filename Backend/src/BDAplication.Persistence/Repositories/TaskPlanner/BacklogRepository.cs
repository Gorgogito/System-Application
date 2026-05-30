using BDAplication.Domain.Entities;
using BDAplication.Domain.Enums;
using BDAplication.Domain.Interfaces;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories.TaskPlanner;

public class BacklogRepository : IBacklogRepository
{
    private readonly ApplicationDbContext _db;
    public BacklogRepository(ApplicationDbContext db) => _db = db;

    public async Task<IEnumerable<Backlog>> GetAllActiveAsync(
        string? search = null, Priority? priority = null, int page = 1, int pageSize = 50)
    {
        var query = _db.Backlogs.Where(b => b.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(b => b.Name.Contains(search) || b.Description.Contains(search));

        if (priority.HasValue)
            query = query.Where(b => b.Priority == priority.Value);

        return await query
            .OrderByDescending(b => b.Priority)
            .ThenBy(b => b.DateCreated)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Backlog?> GetByIdAsync(int id) =>
        await _db.Backlogs.FindAsync(id);

    public async Task<Backlog> CreateAsync(Backlog backlog)
    {
        _db.Backlogs.Add(backlog);
        await _db.SaveChangesAsync();
        return backlog;
    }

    public async Task<Backlog> UpdateAsync(Backlog backlog)
    {
        _db.Backlogs.Update(backlog);
        await _db.SaveChangesAsync();
        return backlog;
    }

    public async Task<int> CountActiveAsync() =>
        await _db.Backlogs.CountAsync(b => b.IsActive);
}
