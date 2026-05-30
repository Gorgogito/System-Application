using BDAplication.Application.DTOs.TaskPlanner;
using BDAplication.Application.Services.TaskPlanner;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Enums;
using BDAplication.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BDAplication.Tests.Services.TaskPlanner;

public class PlannerServiceTests
{
    private readonly Mock<IPlannerRepository> _plannerMock = new();
    private readonly Mock<IBacklogRepository> _backlogMock = new();
    private readonly PlannerService _service;

    public PlannerServiceTests() =>
        _service = new PlannerService(_plannerMock.Object, _backlogMock.Object);

    private static Backlog ActiveBacklog(int id = 1) =>
        new() { Id = id, Name = "Task", Description = "Desc", Priority = Priority.Medium, IsActive = true };

    [Fact]
    public async Task RegisterAsync_ValidRequest_CreatesPlannerTask()
    {
        var backlog = ActiveBacklog();
        _backlogMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(backlog);
        _plannerMock.Setup(r => r.CreateAsync(It.IsAny<Planner>()))
            .ReturnsAsync(new Planner { Id = 1, BacklogId = 1, Day = 15, Month = 6, Year = 2026, Backlog = backlog });

        var result = await _service.RegisterAsync(
            new PlannerRegisterRequest(1, 15, 6, 2026, "Review PR"), "admin");

        result.Should().NotBeNull();
        result.Day.Should().Be(15);
        result.Month.Should().Be(6);
        result.BacklogName.Should().Be("Task");
    }

    [Fact]
    public async Task RegisterAsync_BacklogNotFound_ThrowsKeyNotFoundException()
    {
        _backlogMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Backlog?)null);

        var act = () => _service.RegisterAsync(new PlannerRegisterRequest(99, 1, 6, 2026, ""), "admin");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task RegisterAsync_InactiveBacklog_ThrowsArgumentException()
    {
        _backlogMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Backlog { Id = 1, IsActive = false });

        var act = () => _service.RegisterAsync(new PlannerRegisterRequest(1, 1, 6, 2026, ""), "admin");

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*not active*");
    }

    [Fact]
    public async Task RegisterAsync_InvalidDay_ThrowsArgumentException()
    {
        _backlogMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ActiveBacklog());

        var act = () => _service.RegisterAsync(new PlannerRegisterRequest(1, 32, 6, 2026, ""), "admin");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task InactivateAsync_ExistingPlanner_SetsIsActiveFalse()
    {
        var backlog = ActiveBacklog();
        var planner = new Planner { Id = 5, IsActive = true, Backlog = backlog };
        _plannerMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(planner);
        _plannerMock.Setup(r => r.UpdateAsync(It.IsAny<Planner>()))
            .ReturnsAsync((Planner p) => p);

        var result = await _service.InactivateAsync(new PlannerInactiveRequest(5));

        result.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task InactivateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        _plannerMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Planner?)null);

        var act = () => _service.InactivateAsync(new PlannerInactiveRequest(99));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ListByMonthAsync_ReturnsGroupedByDay()
    {
        var backlog = ActiveBacklog();
        var planners = new List<Planner>
        {
            new() { Id = 1, Day = 1, Month = 6, Year = 2026, Backlog = backlog },
            new() { Id = 2, Day = 1, Month = 6, Year = 2026, Backlog = backlog },
            new() { Id = 3, Day = 15, Month = 6, Year = 2026, Backlog = backlog }
        };
        _plannerMock.Setup(r => r.GetByMonthYearAsync(6, 2026)).ReturnsAsync(planners);

        var result = (await _service.ListByMonthAsync(new PlannerListRequest(6, 2026))).ToList();

        result.Should().HaveCount(2);
        result.First(d => d.Day == 1).Tasks.Should().HaveCount(2);
        result.First(d => d.Day == 15).Tasks.Should().HaveCount(1);
    }

    [Fact]
    public async Task MoveAsync_ValidRequest_UpdatesDayMonthYear()
    {
        var backlog = ActiveBacklog();
        var planner = new Planner { Id = 1, Day = 5, Month = 6, Year = 2026, IsActive = true, Backlog = backlog };
        _plannerMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(planner);
        _plannerMock.Setup(r => r.UpdateAsync(It.IsAny<Planner>()))
            .ReturnsAsync((Planner p) => p);

        var result = await _service.MoveAsync(new MovePlannerRequest(1, 20, 7, 2026));

        result.Day.Should().Be(20);
        result.Month.Should().Be(7);
        result.Year.Should().Be(2026);
    }

    [Fact]
    public async Task MoveAsync_InactivePlanner_ThrowsArgumentException()
    {
        var backlog = ActiveBacklog();
        _plannerMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Planner { Id = 1, IsActive = false, Backlog = backlog });

        var act = () => _service.MoveAsync(new MovePlannerRequest(1, 20, 7, 2026));

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*inactive*");
    }
}
