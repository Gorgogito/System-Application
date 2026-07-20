using BDAplication.Domain.Entities.Finance;
using BDAplication.Domain.Interfaces.Finance;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories.Finance;

public class ReprocessLogRepository : IReprocessLogRepository
{
    private readonly ApplicationDbContext _db;
    public ReprocessLogRepository(ApplicationDbContext db) => _db = db;

    public async Task<ReprocessLog> CreateAsync(ReprocessLog log)
    {
        _db.ReprocessLogs.Add(log);
        await _db.SaveChangesAsync();
        return log;
    }

    public async Task<IEnumerable<ReprocessLog>> GetRecentAsync(int limit = 20) =>
        await _db.ReprocessLogs
            .OrderByDescending(l => l.ExecutedAt)
            .Take(limit)
            .ToListAsync();
}
