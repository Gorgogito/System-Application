using BDAplication.Domain.Entities;
using BDAplication.Domain.Enums;

namespace BDAplication.Domain.Interfaces;

public interface ITaskBoardRepository
{
    Task<IEnumerable<BoardTask>> GetAllActiveAsync(TaskBoardStatus? status = null, Priority? priority = null, string? search = null);
    Task<BoardTask?> GetByIdAsync(int id);
    Task<BoardTask> CreateAsync(BoardTask task);
    Task<BoardTask> UpdateAsync(BoardTask task);
    Task<BoardTask> MoveAsync(int id, TaskBoardStatus newStatus);
    Task<BoardTask> SoftDeleteAsync(int id);

    // Subtareas
    Task<SubTask> CreateSubTaskAsync(SubTask subTask);
    Task<SubTask?> GetSubTaskByIdAsync(int id);
    Task<SubTask> ToggleSubTaskAsync(int id);
    Task DeleteSubTaskAsync(int id);
}
