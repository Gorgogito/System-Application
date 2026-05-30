using BDAplication.Domain.Enums;

namespace BDAplication.Application.DTOs.TaskPlanner;

public record PlannerDto(
    int Id,
    int BacklogId,
    string BacklogName,
    string BacklogDescription,
    Priority Priority,
    string PriorityLabel,
    int Day,
    int Month,
    int Year,
    string Notes,
    bool IsActive,
    string UserCreated,
    DateTime DateCreated);

public record PlannerRegisterRequest(
    int BacklogId,
    int Day,
    int Month,
    int Year,
    string Notes);

public record PlannerInactiveRequest(int PlannerId);

public record PlannerListRequest(int Month, int Year);

public record PlannerListByDayDto(int Day, IEnumerable<PlannerDto> Tasks);

public record MovePlannerRequest(
    int PlannerId,
    int NewDay,
    int NewMonth,
    int NewYear);
