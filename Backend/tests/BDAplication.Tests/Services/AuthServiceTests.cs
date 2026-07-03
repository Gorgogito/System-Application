using BDAplication.Application.DTOs;
using BDAplication.Application.Interfaces;
using BDAplication.Application.Services;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;
using Moq;

namespace BDAplication.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();
    private readonly Mock<IJwtService> _jwtMock = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(_repoMock.Object, _hasherMock.Object, _jwtMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        var role = new Role { Id = 1, Name = "Admin", Description = "Administrator" };
        var user = new User
        {
            Id = 1, Username = "admin", PasswordHash = "hashed",
            FullName = "Admin", Email = "admin@test.com", RoleId = 1,
            Role = role, IsActive = true
        };

        _repoMock.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync(user);
        _hasherMock.Setup(h => h.Verify("password", "hashed")).Returns(true);
        _jwtMock.Setup(j => j.GenerateToken(user))
            .Returns(("jwt_token_here", DateTime.UtcNow.AddHours(8)));

        var result = await _service.LoginAsync(new LoginRequest("admin", "password"));

        Assert.NotNull(result);
        Assert.Equal("jwt_token_here", result.Token);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        var user = new User
        {
            Id = 1, Username = "admin", PasswordHash = "hashed",
            IsActive = true, Role = new Role { Name = "Admin" }
        };

        _repoMock.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync(user);
        _hasherMock.Setup(h => h.Verify("wrongpassword", "hashed")).Returns(false);

        var result = await _service.LoginAsync(new LoginRequest("admin", "wrongpassword"));

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_NonExistingUser_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByUsernameAsync("unknown")).ReturnsAsync((User?)null);

        var result = await _service.LoginAsync(new LoginRequest("unknown", "pass"));

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_InactiveUser_ReturnsNull()
    {
        var user = new User
        {
            Id = 2, Username = "inactive", PasswordHash = "hashed",
            IsActive = false, Role = new Role { Name = "User" }
        };

        _repoMock.Setup(r => r.GetByUsernameAsync("inactive")).ReturnsAsync(user);
        _hasherMock.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        var result = await _service.LoginAsync(new LoginRequest("inactive", "pass"));

        Assert.Null(result);
    }
}
