using BDAplication.Application.DTOs;
using BDAplication.Application.Interfaces;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;

namespace BDAplication.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher _hasher;

    public UserService(IUserRepository repo, IPasswordHasher hasher)
    {
        _repo = repo;
        _hasher = hasher;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _repo.GetAllWithRolesAsync();
        return users.Select(ToDto);
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _repo.GetByIdWithRoleAsync(id);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            PasswordHash = _hasher.Hash(request.Password),
            FullName = request.FullName,
            Email = request.Email,
            RoleId = request.RoleId
        };
        var created = await _repo.CreateAsync(user);
        var withRole = await _repo.GetByIdWithRoleAsync(created.Id)
            ?? throw new InvalidOperationException("User created but not retrievable");
        return ToDto(withRole);
    }

    public async Task<UserDto> UpdateAsync(UpdateUserRequest request)
    {
        var user = await _repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"User {request.Id} not found");

        user.Username = request.Username;
        user.FullName = request.FullName;
        user.Email = request.Email;
        user.RoleId = request.RoleId;
        user.IsActive = request.IsActive;

        if (!string.IsNullOrWhiteSpace(request.Password))
            user.PasswordHash = _hasher.Hash(request.Password);

        await _repo.UpdateAsync(user);
        var withRole = await _repo.GetByIdWithRoleAsync(user.Id)
            ?? throw new InvalidOperationException("User updated but not retrievable");
        return ToDto(withRole);
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    private static UserDto ToDto(User u) =>
        new(u.Id, u.Username, u.FullName, u.Email, u.RoleId, u.Role?.Name ?? "", u.IsActive, u.CreatedAt);
}
