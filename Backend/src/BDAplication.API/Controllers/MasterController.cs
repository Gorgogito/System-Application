using BDAplication.Application.DTOs;
using BDAplication.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDAplication.API.Controllers;

[ApiController]
[Route("api/master")]
public class MasterController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IUserService _users;
    private readonly IRoleService _roles;

    public MasterController(IAuthService auth, IUserService users, IRoleService roles)
    {
        _auth = auth;
        _users = users;
        _roles = roles;
    }

    // ── Authentication ─────────────────────────────────────────────

    /// <summary>Autentica usuario y retorna JWT + datos</summary>
    [HttpPost("authentication")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _auth.LoginAsync(request);
        if (result is null)
            return Unauthorized(ApiResponse<object>.Fail("Invalid username or password"));
        return Ok(ApiResponse<LoginResponse>.Ok(result, "Login successful"));
    }

    // ── Usuarios ───────────────────────────────────────────────────

    /// <summary>Obtiene todos los usuarios</summary>
    [HttpGet("getuser")]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _users.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<UserDto>>.Ok(users));
    }

    /// <summary>Crea un nuevo usuario</summary>
    [HttpPost("createuser")]
    [Authorize]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await _users.CreateAsync(request);
        return Ok(ApiResponse<UserDto>.Ok(user, "User created successfully"));
    }

    /// <summary>Actualiza un usuario existente</summary>
    [HttpPut("updateuser")]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        var user = await _users.UpdateAsync(request);
        return Ok(ApiResponse<UserDto>.Ok(user, "User updated successfully"));
    }

    /// <summary>Elimina un usuario por ID</summary>
    [HttpDelete("deleteuser")]
    [Authorize]
    public async Task<IActionResult> DeleteUser([FromQuery] int id)
    {
        var deleted = await _users.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("User not found"));
        return Ok(ApiResponse<object>.Ok(null!, "User deleted successfully"));
    }

    // ── Roles ──────────────────────────────────────────────────────

    /// <summary>Obtiene todos los roles</summary>
    [HttpGet("getrol")]
    [Authorize]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roles.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<RoleDto>>.Ok(roles));
    }

    /// <summary>Crea un nuevo rol</summary>
    [HttpPost("createrol")]
    [Authorize]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var role = await _roles.CreateAsync(request);
        return Ok(ApiResponse<RoleDto>.Ok(role, "Role created successfully"));
    }

    /// <summary>Actualiza un rol existente</summary>
    [HttpPut("updaterol")]
    [Authorize]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest request)
    {
        var role = await _roles.UpdateAsync(request);
        return Ok(ApiResponse<RoleDto>.Ok(role, "Role updated successfully"));
    }

    /// <summary>Elimina un rol por ID</summary>
    [HttpDelete("deleterol")]
    [Authorize]
    public async Task<IActionResult> DeleteRole([FromQuery] int id)
    {
        var deleted = await _roles.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Role not found"));
        return Ok(ApiResponse<object>.Ok(null!, "Role deleted successfully"));
    }
}
