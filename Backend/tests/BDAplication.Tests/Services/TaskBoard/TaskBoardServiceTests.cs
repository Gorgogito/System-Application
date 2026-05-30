using BDAplication.Application.DTOs.TaskBoard;
using BDAplication.Application.Services.TaskBoard;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Enums;
using BDAplication.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BDAplication.Tests.Services.TaskBoard;

public class TaskBoardServiceTests
{
    private readonly Mock<ITaskBoardRepository> _repoMock = new();
    private readonly TaskBoardService _service;

    public TaskBoardServiceTests() => _service = new TaskBoardService(_repoMock.Object);

    [Fact]
    public async Task CreateAsync_ValidRequest_CreatesTaskWithPendingStatus()
    {
        var request = new CreateTaskBoardRequest("Pay mortgage", "Monthly payment", Priority.High);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<BoardTask>()))
            .ReturnsAsync(new BoardTask
            {
                Id = 1, Title = "Pay mortgage", Description = "Monthly payment",
                Priority = Priority.High, Status = TaskBoardStatus.Pending, UserCreated = "admin"
            });

        var result = await _service.CreateAsync(request, "admin");

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Pay mortgage");
        result.Status.Should().Be(TaskBoardStatus.Pending);
        result.Priority.Should().Be(Priority.High);
        result.UserCreated.Should().Be("admin");
    }

    [Fact]
    public async Task GetAllAsync_NoFilters_ReturnsAllActiveTasks()
    {
        var tasks = new List<BoardTask>
        {
            new() { Id = 1, Title = "Task A", Status = TaskBoardStatus.Pending, IsActive = true },
            new() { Id = 2, Title = "Task B", Status = TaskBoardStatus.InProgress, IsActive = true },
            new() { Id = 3, Title = "Task C", Status = TaskBoardStatus.Completed, IsActive = true }
        };
        _repoMock.Setup(r => r.GetAllActiveAsync(null, null, null)).ReturnsAsync(tasks);

        var result = await _service.GetAllAsync(new GetTaskBoardRequest(null, null, null));

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsTask()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new BoardTask { Id = 1, Title = "Task A", Status = TaskBoardStatus.Pending });

        var result = await _service.GetByIdAsync(1);

        result.Id.Should().Be(1);
        result.Title.Should().Be("Task A");
        result.StatusLabel.Should().Be("Pending");
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((BoardTask?)null);

        Func<Task> act = async () => await _service.GetByIdAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_UpdatesTask()
    {
        var existing = new BoardTask { Id = 1, Title = "Old Title", Priority = Priority.Low };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<BoardTask>()))
            .ReturnsAsync(new BoardTask { Id = 1, Title = "New Title", Priority = Priority.High });

        var result = await _service.UpdateAsync(new UpdateTaskBoardRequest(1, "New Title", "New desc", Priority.High));

        result.Title.Should().Be("New Title");
        result.Priority.Should().Be(Priority.High);
    }

    [Fact]
    public async Task MoveAsync_ChangesStatusCorrectly()
    {
        _repoMock.Setup(r => r.MoveAsync(1, TaskBoardStatus.InProgress))
            .ReturnsAsync(new BoardTask { Id = 1, Title = "Task A", Status = TaskBoardStatus.InProgress });

        var result = await _service.MoveAsync(new MoveTaskBoardRequest(1, TaskBoardStatus.InProgress));

        result.Status.Should().Be(TaskBoardStatus.InProgress);
        result.StatusLabel.Should().Be("InProgress");
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesTask()
    {
        _repoMock.Setup(r => r.SoftDeleteAsync(1))
            .ReturnsAsync(new BoardTask { Id = 1, Title = "Task A", IsActive = false });

        var result = await _service.DeleteAsync(1);

        result.IsActive.Should().BeFalse();
    }
}
