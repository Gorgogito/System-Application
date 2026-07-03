using BDAplication.Application.DTOs;
using BDAplication.Application.Interfaces;
using BDAplication.Application.Services;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;
using Moq;

namespace BDAplication.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _hasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed_password");
        _service = new UserService(_repoMock.Object, _hasherMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        var role = new Role { Id = 1, Name = "Admin" };
        _repoMock.Setup(r => r.GetAllWithRolesAsync())
            .ReturnsAsync(new List<User>
            {
                new() { Id = 1, Username = "admin", FullName = "Admin User", Email = "admin@test.com", Role = role, RoleId = 1 }
            });

        var result = await _service.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("admin", result.First().Username);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_HashesPassword()
    {
        var request = new CreateUserRequest("newuser", "password123", "New User", "new@test.com", 1);
        var role = new Role { Id = 1, Name = "User" };
        var created = new User
        {
            Id = 5, Username = "newuser", PasswordHash = "hashed_password",
            FullName = "New User", Email = "new@test.com", RoleId = 1, Role = role
        };

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(created);
        _repoMock.Setup(r => r.GetByIdWithRoleAsync(5)).ReturnsAsync(created);

        var result = await _service.CreateAsync(request);

        Assert.Equal("newuser", result.Username);
        _hasherMock.Verify(h => h.Hash("password123"), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingUser_ReturnsFalse()
    {
        _repoMock.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

        var result = await _service.DeleteAsync(999);

        Assert.False(result);
    }
}
