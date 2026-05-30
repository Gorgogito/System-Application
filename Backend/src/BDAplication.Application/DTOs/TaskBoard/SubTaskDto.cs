namespace BDAplication.Application.DTOs.TaskBoard;

public record SubTaskDto(
    int Id,
    int BoardTaskId,
    string Title,
    bool IsCompleted,
    DateTime DateCreated);

public record CreateSubTaskRequest(int BoardTaskId, string Title);
public record ToggleSubTaskRequest(int SubTaskId);
