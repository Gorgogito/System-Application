namespace BDAplication.Application.DTOs;

public record LoginRequest(string Username, string Password);

public record LoginResponse(
    string Token,
    DateTime Expiration,
    UserDto User,
    RoleDto Role);
