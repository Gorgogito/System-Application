using BDAplication.Domain.Entities;

namespace BDAplication.Domain.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
}
