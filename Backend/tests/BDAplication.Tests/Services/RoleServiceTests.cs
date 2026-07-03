using BDAplication.Application.DTOs;
using BDAplication.Application.Services;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;
using Moq;

namespace BDAplication.Tests.Services;

public class RoleServiceTests
{
    private readonly Mock<IRoleRepository> _repoMock = new();
    private readonly RoleService _service;

    public RoleServiceTests() => _service = new RoleService(_repoMock.Object);

    [Fact]
    public async Task GetAllAsync_ReturnsAllRoles()
    {
        _repoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Role>
            {
                new() { Id = 1, Name = "Admin", Description = "Administrator", IsActive = true },
                new() { Id = 2, Name = "User", Description = "Standard", IsActive = true }
            });

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsRole()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Role { Id = 1, Name = "Admin", Description = "Administrator" });

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Admin", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Role?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsCreatedRole()
    {
        var request = new CreateRoleRequest("Supervisor", "Supervisor role");
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Role>()))
            .ReturnsAsync(new Role { Id = 3, Name = "Supervisor", Description = "Supervisor role" });

        var result = await _service.CreateAsync(request);

        Assert.Equal("Supervisor", result.Name);
        Assert.Equal(3, result.Id);
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_ReturnsTrue()
    {
        _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }
}
