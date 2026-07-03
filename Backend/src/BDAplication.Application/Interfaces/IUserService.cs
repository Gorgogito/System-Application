using BDAplication.Application.DTOs;

namespace BDAplication.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<UserDto> UpdateAsync(UpdateUserRequest request);
    Task<bool> DeleteAsync(int id);
}
