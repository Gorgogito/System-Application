using BDAplication.Domain.Enums;

namespace BDAplication.Application.DTOs.TaskPlanner;

public record BacklogDto(
    int Id,
    string Name,
    string Description,
    Priority Priority,
    string PriorityLabel,
    bool IsActive,
    string UserCreated,
    DateTime DateCreated);

public record BacklogRegisterRequest(
    string Name,
    string Description,
    Priority Priority);

public record BacklogListRequest(
    string? Search,
    Priority? Priority,
    int Page,
    int PageSize);

public record BacklogListResponse(
    IEnumerable<BacklogDto> Items,
    int Total,
    int Page,
    int PageSize);
