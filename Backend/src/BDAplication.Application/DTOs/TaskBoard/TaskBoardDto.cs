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
    DateTime? StartDate,
    DateTime? DueDate,
    int? AlertHoursBeforeDue,
    DueStatus DueStatus,
    string DueStatusLabel,
    IEnumerable<SubTaskDto> SubTasks);

public record CreateTaskBoardRequest(
    string Title,
    string Description,
    Priority Priority,
    DateTime? StartDate = null,
    DateTime? DueDate = null,
    int? AlertHoursBeforeDue = null);

public record UpdateTaskBoardRequest(
    int Id,
    string Title,
    string Description,
    Priority Priority,
    DateTime? StartDate = null,
    DateTime? DueDate = null,
    int? AlertHoursBeforeDue = null);

public record MoveTaskBoardRequest(
    int TaskId,
    TaskBoardStatus Status);

public record GetTaskBoardRequest(
    TaskBoardStatus? Status,
    Priority? Priority,
    string? Search);
