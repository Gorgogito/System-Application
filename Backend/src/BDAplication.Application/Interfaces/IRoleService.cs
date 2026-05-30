using BDAplication.Application.DTOs;

namespace BDAplication.Application.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync();
    Task<RoleDto?> GetByIdAsync(int id);
    Task<RoleDto> CreateAsync(CreateRoleRequest request);
    Task<RoleDto> UpdateAsync(UpdateRoleRequest request);
    Task<bool> DeleteAsync(int id);
}
