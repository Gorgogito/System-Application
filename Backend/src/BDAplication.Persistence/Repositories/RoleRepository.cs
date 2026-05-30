using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext db) : base(db) { }

    public async Task<Role?> GetByNameAsync(string name) =>
        await _db.Roles.FirstOrDefaultAsync(r => r.Name == name);
}
