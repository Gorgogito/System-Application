using BDAplication.Domain.Entities;
using BDAplication.Domain.Enums;
using BDAplication.Domain.Interfaces;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories.TaskBoard;

public class TaskBoardRepository : ITaskBoardRepository
{
    private readonly ApplicationDbContext _db;
    public TaskBoardRepository(ApplicationDbContext db) => _db = db;

    private IQueryable<BoardTask> WithSubTasks() =>
        _db.BoardTasks.Include(t => t.SubTasks.OrderBy(s => s.DateCreated));

    public async Task<IEnumerable<BoardTask>> GetAllActiveAsync(
        TaskBoardStatus? status = null, Priority? priority = null, string? search = null)
    {
        var query = WithSubTasks().Where(t => t.IsActive);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);
        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Title.Contains(search) || t.Description.Contains(search));

        return await query
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DateCreated)
            .ToListAsync();
    }

    public async Task<BoardTask?> GetByIdAsync(int id) =>
        await WithSubTasks().FirstOrDefaultAsync(t => t.Id == id);

    public async Task<BoardTask> CreateAsync(BoardTask task)
    {
        _db.BoardTasks.Add(task);
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task<BoardTask> UpdateAsync(BoardTask task)
    {
        _db.BoardTasks.Update(task);
        await _db.SaveChangesAsync();
        return await GetByIdAsync(task.Id) ?? task;
    }

    public async Task<BoardTask> MoveAsync(int id, TaskBoardStatus newStatus)
    {
        var task = await WithSubTasks().FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new KeyNotFoundException($"Task {id} not found");
        task.Status = newStatus;
        task.DateUpdated = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task<BoardTask> SoftDeleteAsync(int id)
    {
        var task = await WithSubTasks().FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new KeyNotFoundException($"Task {id} not found");
        task.IsActive = false;
        task.DateUpdated = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return task;
    }

    // ── Subtareas ────────────────────────────────────────────
    public async Task<SubTask> CreateSubTaskAsync(SubTask subTask)
    {
        _db.SubTasks.Add(subTask);
        await _db.SaveChangesAsync();
        return subTask;
    }

    public async Task<SubTask?> GetSubTaskByIdAsync(int id) =>
        await _db.SubTasks.FindAsync(id);

    public async Task<SubTask> ToggleSubTaskAsync(int id)
    {
        var sub = await _db.SubTasks.FindAsync(id)
            ?? throw new KeyNotFoundException($"SubTask {id} not found");
        sub.IsCompleted = !sub.IsCompleted;
        await _db.SaveChangesAsync();
        return sub;
    }

    public async System.Threading.Tasks.Task DeleteSubTaskAsync(int id)
    {
        var sub = await _db.SubTasks.FindAsync(id)
            ?? throw new KeyNotFoundException($"SubTask {id} not found");
        _db.SubTasks.Remove(sub);
        await _db.SaveChangesAsync();
    }
}
