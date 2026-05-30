using BDAplication.Domain.Entities;

namespace BDAplication.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllWithRolesAsync();
    Task<User?> GetByIdWithRoleAsync(int id);
}
