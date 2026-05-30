using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext db) : base(db) { }

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _db.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username);

    public async Task<IEnumerable<User>> GetAllWithRolesAsync() =>
        await _db.Users.Include(u => u.Role).ToListAsync();

    public async Task<User?> GetByIdWithRoleAsync(int id) =>
        await _db.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
}
