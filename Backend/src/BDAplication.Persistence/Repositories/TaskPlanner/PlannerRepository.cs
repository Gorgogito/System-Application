using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories.TaskPlanner;

public class PlannerRepository : IPlannerRepository
{
    private readonly ApplicationDbContext _db;
    public PlannerRepository(ApplicationDbContext db) => _db = db;

    public async Task<IEnumerable<Planner>> GetByMonthYearAsync(int month, int year) =>
        await _db.Planners
            .Include(p => p.Backlog)
            .Where(p => p.Month == month && p.Year == year && p.IsActive)
            .OrderBy(p => p.Day)
            .ThenBy(p => p.DateCreated)
            .ToListAsync();

    public async Task<Planner?> GetByIdAsync(int id) =>
        await _db.Planners
            .Include(p => p.Backlog)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Planner> CreateAsync(Planner planner)
    {
        _db.Planners.Add(planner);
        await _db.SaveChangesAsync();
        return planner;
    }

    public async Task<Planner> UpdateAsync(Planner planner)
    {
        _db.Planners.Update(planner);
        await _db.SaveChangesAsync();
        return planner;
    }
}
