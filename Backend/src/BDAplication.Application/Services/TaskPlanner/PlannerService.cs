using BDAplication.Application.DTOs.TaskPlanner;
using BDAplication.Application.Interfaces.TaskPlanner;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;

namespace BDAplication.Application.Services.TaskPlanner;

public class PlannerService : IPlannerService
{
    private readonly IPlannerRepository _plannerRepo;
    private readonly IBacklogRepository _backlogRepo;

    public PlannerService(IPlannerRepository plannerRepo, IBacklogRepository backlogRepo)
    {
        _plannerRepo = plannerRepo;
        _backlogRepo = backlogRepo;
    }

    public async Task<PlannerDto> RegisterAsync(PlannerRegisterRequest request, string user)
    {
        var backlog = await _backlogRepo.GetByIdAsync(request.BacklogId)
            ?? throw new KeyNotFoundException($"Backlog {request.BacklogId} not found");

        if (!backlog.IsActive)
            throw new ArgumentException("Backlog task is not active");

        ValidateDate(request.Day, request.Month, request.Year);

        var planner = new Planner
        {
            BacklogId = request.BacklogId,
            Day = request.Day,
            Month = request.Month,
            Year = request.Year,
            Notes = request.Notes,
            UserCreated = user
        };
        var created = await _plannerRepo.CreateAsync(planner);
        created.Backlog = backlog;
        return ToDto(created);
    }

    public async Task<PlannerDto> InactivateAsync(PlannerInactiveRequest request)
    {
        var planner = await _plannerRepo.GetByIdAsync(request.PlannerId)
            ?? throw new KeyNotFoundException($"Planner task {request.PlannerId} not found");

        planner.IsActive = false;
        var updated = await _plannerRepo.UpdateAsync(planner);
        return ToDto(updated);
    }

    public async Task<IEnumerable<PlannerListByDayDto>> ListByMonthAsync(PlannerListRequest request)
    {
        var planners = await _plannerRepo.GetByMonthYearAsync(request.Month, request.Year);
        return planners
            .GroupBy(p => p.Day)
            .OrderBy(g => g.Key)
            .Select(g => new PlannerListByDayDto(g.Key, g.Select(ToDto)));
    }

    public async Task<PlannerDto> MoveAsync(MovePlannerRequest request)
    {
        var planner = await _plannerRepo.GetByIdAsync(request.PlannerId)
            ?? throw new KeyNotFoundException($"Planner task {request.PlannerId} not found");

        if (!planner.IsActive)
            throw new ArgumentException("Cannot move an inactive planner task");

        ValidateDate(request.NewDay, request.NewMonth, request.NewYear);

        planner.Day = request.NewDay;
        planner.Month = request.NewMonth;
        planner.Year = request.NewYear;

        var updated = await _plannerRepo.UpdateAsync(planner);
        return ToDto(updated);
    }

    private static void ValidateDate(int day, int month, int year)
    {
        if (month < 1 || month > 12)
            throw new ArgumentException("Invalid month");
        if (year < 2000 || year > 2100)
            throw new ArgumentException("Invalid year");
        var daysInMonth = DateTime.DaysInMonth(year, month);
        if (day < 1 || day > daysInMonth)
            throw new ArgumentException($"Day {day} is invalid for month {month}/{year}");
    }

    private static PlannerDto ToDto(Planner p) =>
        new(p.Id, p.BacklogId,
            p.Backlog?.Name ?? "", p.Backlog?.Description ?? "",
            p.Backlog?.Priority ?? Domain.Enums.Priority.Medium,
            (p.Backlog?.Priority ?? Domain.Enums.Priority.Medium).ToString(),
            p.Day, p.Month, p.Year, p.Notes,
            p.IsActive, p.UserCreated, p.DateCreated);
}
