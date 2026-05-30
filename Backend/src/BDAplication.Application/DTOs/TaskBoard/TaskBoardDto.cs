using BDAplication.Domain.Enums;

namespace BDAplication.Application.DTOs.TaskBoard;

public record TaskBoardDto(
    int Id,
    string Title,
    string Description,
    Priority Priority,
    string PriorityLabel,
    TaskBoardStatus Status,
    string StatusLabel,
    string UserCreated,
    DateTime DateCreated,
    DateTime DateUpdated,
    bool IsActive,
    IEnumerable<SubTaskDto> SubTasks);

public record CreateTaskBoardRequest(
    string Title,
    string Description,
    Priority Priority);

public record UpdateTaskBoardRequest(
    int Id,
    string Title,
    string Description,
    Priority Priority);

public record MoveTaskBoardRequest(
    int TaskId,
    TaskBoardStatus Status);

public record GetTaskBoardRequest(
    TaskBoardStatus? Status,
    Priority? Priority,
    string? Search);
