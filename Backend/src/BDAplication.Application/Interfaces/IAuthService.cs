using BDAplication.Application.DTOs;

namespace BDAplication.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
