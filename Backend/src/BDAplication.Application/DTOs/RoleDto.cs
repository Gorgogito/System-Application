namespace BDAplication.Application.DTOs;

public record RoleDto(int Id, string Name, string Description, bool IsActive, DateTime CreatedAt);

public record CreateRoleRequest(string Name, string Description);

public record UpdateRoleRequest(int Id, string Name, string Description, bool IsActive);
