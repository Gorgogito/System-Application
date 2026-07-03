using BDAplication.Application.DTOs.TaskPlanner;
using BDAplication.Application.Services.TaskPlanner;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Enums;
using BDAplication.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BDAplication.Tests.Services.TaskPlanner;

public class BacklogServiceTests
{
    private readonly Mock<IBacklogRepository> _repoMock = new();
    private readonly BacklogService _service;

    public BacklogServiceTests() => _service = new BacklogService(_repoMock.Object);

    [Fact]
    public async Task RegisterAsync_ValidRequest_CreatesBacklog()
    {
        var request = new BacklogRegisterRequest("Fix login bug", "Login fails on mobile", Priority.High);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Backlog>()))
            .ReturnsAsync(new Backlog { Id = 1, Name = "Fix login bug", Description = "Login fails on mobile", Priority = Priority.High, UserCreated = "admin" });

        var result = await _service.RegisterAsync(request, "admin");

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Fix login bug");
        result.Priority.Should().Be(Priority.High);
        result.PriorityLabel.Should().Be("High");
    }

    [Fact]
    public async Task ListAsync_ReturnsPagedResults()
    {
        var backlogs = new List<Backlog>
        {
            new() { Id = 1, Name = "Task A", Priority = Priority.Critical, IsActive = true },
            new() { Id = 2, Name = "Task B", Priority = Priority.Low, IsActive = true }
        };
        _repoMock.Setup(r => r.GetAllActiveAsync(null, null, 1, 50)).ReturnsAsync(backlogs);
        _repoMock.Setup(r => r.CountActiveAsync()).ReturnsAsync(2);

        var result = await _service.ListAsync(new BacklogListRequest(null, null, 1, 50));

        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(2);
    }

    [Fact]
    public async Task ListAsync_WithSearchFilter_PassesToRepo()
    {
        _repoMock.Setup(r => r.GetAllActiveAsync("login", null, 1, 50))
            .ReturnsAsync(new List<Backlog> { new() { Id = 1, Name = "Fix login bug" } });
        _repoMock.Setup(r => r.CountActiveAsync()).ReturnsAsync(1);

        var result = await _service.ListAsync(new BacklogListRequest("login", null, 1, 50));

        result.Items.Should().HaveCount(1);
        _repoMock.Verify(r => r.GetAllActiveAsync("login", null, 1, 50), Times.Once);
    }

    [Fact]
    public async Task ListAsync_WithPriorityFilter_PassesToRepo()
    {
        _repoMock.Setup(r => r.GetAllActiveAsync(null, Priority.High, 1, 50))
            .ReturnsAsync(new List<Backlog> { new() { Id = 1, Name = "High task", Priority = Priority.High } });
        _repoMock.Setup(r => r.CountActiveAsync()).ReturnsAsync(1);

        var result = await _service.ListAsync(new BacklogListRequest(null, Priority.High, 1, 50));

        result.Items.Should().OnlyContain(x => x.Priority == Priority.High);
    }
}
