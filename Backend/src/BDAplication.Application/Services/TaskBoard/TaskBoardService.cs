using BDAplication.Application.DTOs.TaskBoard;
using BDAplication.Application.Interfaces.TaskBoard;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;

namespace BDAplication.Application.Services.TaskBoard;

public class TaskBoardService : ITaskBoardService
{
    private readonly ITaskBoardRepository _repo;

    public TaskBoardService(ITaskBoardRepository repo) => _repo = repo;

    public async Task<TaskBoardDto> CreateAsync(CreateTaskBoardRequest request, string user)
    {
        var entity = new BoardTask
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            UserCreated = user
        };
        return ToDto(await _repo.CreateAsync(entity));
    }

    public async Task<IEnumerable<TaskBoardDto>> GetAllAsync(GetTaskBoardRequest request)
    {
        var items = await _repo.GetAllActiveAsync(request.Status, request.Priority, request.Search);
        return items.Select(ToDto);
    }

    public async Task<TaskBoardDto> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Task {id} not found");
        return ToDto(entity);
    }

    public async Task<TaskBoardDto> UpdateAsync(UpdateTaskBoardRequest request)
    {
        var entity = await _repo.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException($"Task {request.Id} not found");
        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.Priority = request.Priority;
        entity.DateUpdated = DateTime.UtcNow;
        return ToDto(await _repo.UpdateAsync(entity));
    }

    public async Task<TaskBoardDto> MoveAsync(MoveTaskBoardRequest request) =>
        ToDto(await _repo.MoveAsync(request.TaskId, request.Status));

    public async Task<TaskBoardDto> DeleteAsync(int id) =>
        ToDto(await _repo.SoftDeleteAsync(id));

    // ── Subtareas ───────────────────────────────────────────
    public async Task<SubTaskDto> CreateSubTaskAsync(CreateSubTaskRequest request, string user)
    {
        var entity = new SubTask
        {
            BoardTaskId = request.BoardTaskId,
            Title = request.Title,
            UserCreated = user
        };
        return SubToDto(await _repo.CreateSubTaskAsync(entity));
    }

    public async Task<SubTaskDto> ToggleSubTaskAsync(int subTaskId) =>
        SubToDto(await _repo.ToggleSubTaskAsync(subTaskId));

    public async System.Threading.Tasks.Task DeleteSubTaskAsync(int subTaskId) =>
        await _repo.DeleteSubTaskAsync(subTaskId);

    // ── Mapping ─────────────────────────────────────────────
    private static TaskBoardDto ToDto(BoardTask t) =>
        new(t.Id, t.Title, t.Description, t.Priority, t.Priority.ToString(),
            t.Status, t.Status.ToString(), t.UserCreated, t.DateCreated, t.DateUpdated,
            t.IsActive, t.SubTasks.Select(SubToDto));

    private static SubTaskDto SubToDto(SubTask s) =>
        new(s.Id, s.BoardTaskId, s.Title, s.IsCompleted, s.DateCreated);
}
