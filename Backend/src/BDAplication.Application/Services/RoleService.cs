using BDAplication.Application.DTOs;
using BDAplication.Application.Interfaces;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;

namespace BDAplication.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repo;

    public RoleService(IRoleRepository repo) => _repo = repo;

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var roles = await _repo.GetAllAsync();
        return roles.Select(ToDto);
    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        var role = await _repo.GetByIdAsync(id);
        return role is null ? null : ToDto(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleRequest request)
    {
        var role = new Role { Name = request.Name, Description = request.Description };
        var created = await _repo.CreateAsync(role);
        return ToDto(created);
    }

    public async Task<RoleDto> UpdateAsync(UpdateRoleRequest request)
    {
        var role = await _repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Role {request.Id} not found");

        role.Name = request.Name;
        role.Description = request.Description;
        role.IsActive = request.IsActive;

        var updated = await _repo.UpdateAsync(role);
        return ToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    private static RoleDto ToDto(Role r) =>
        new(r.Id, r.Name, r.Description, r.IsActive, r.CreatedAt);
}
