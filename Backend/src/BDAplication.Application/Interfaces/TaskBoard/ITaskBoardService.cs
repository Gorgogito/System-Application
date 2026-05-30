using BDAplication.Application.DTOs.TaskBoard;

namespace BDAplication.Application.Interfaces.TaskBoard;

public interface ITaskBoardService
{
    Task<TaskBoardDto> CreateAsync(CreateTaskBoardRequest request, string user);
    Task<IEnumerable<TaskBoardDto>> GetAllAsync(GetTaskBoardRequest request);
    Task<TaskBoardDto> GetByIdAsync(int id);
    Task<TaskBoardDto> UpdateAsync(UpdateTaskBoardRequest request);
    Task<TaskBoardDto> MoveAsync(MoveTaskBoardRequest request);
    Task<TaskBoardDto> DeleteAsync(int id);

    // Subtareas
    Task<SubTaskDto> CreateSubTaskAsync(CreateSubTaskRequest request, string user);
    Task<SubTaskDto> ToggleSubTaskAsync(int subTaskId);
    Task DeleteSubTaskAsync(int subTaskId);
}
