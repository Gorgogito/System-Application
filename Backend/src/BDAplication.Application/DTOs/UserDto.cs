namespace BDAplication.Application.DTOs;

public record UserDto(
    int Id,
    string Username,
    string FullName,
    string Email,
    int RoleId,
    string RoleName,
    bool IsActive,
    DateTime CreatedAt);

public record CreateUserRequest(
    string Username,
    string Password,
    string FullName,
    string Email,
    int RoleId);

public record UpdateUserRequest(
    int Id,
    string Username,
    string? Password,
    string FullName,
    string Email,
    int RoleId,
    bool IsActive);
