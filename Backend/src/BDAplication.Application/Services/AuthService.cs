using BDAplication.Application.DTOs;
using BDAplication.Application.Interfaces;
using BDAplication.Domain.Interfaces;

namespace BDAplication.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public AuthService(IUserRepository userRepo, IPasswordHasher hasher, IJwtService jwt)
    {
        _userRepo = userRepo;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.GetByUsernameAsync(request.Username);
        if (user is null || !user.IsActive || !_hasher.Verify(request.Password, user.PasswordHash))
            return null;

        var (token, expiration) = _jwt.GenerateToken(user);

        var userDto = new UserDto(
            user.Id, user.Username, user.FullName, user.Email,
            user.RoleId, user.Role?.Name ?? "", user.IsActive, user.CreatedAt);

        var roleDto = new RoleDto(
            user.Role!.Id, user.Role.Name, user.Role.Description,
            user.Role.IsActive, user.Role.CreatedAt);

        return new LoginResponse(token, expiration, userDto, roleDto);
    }
}
