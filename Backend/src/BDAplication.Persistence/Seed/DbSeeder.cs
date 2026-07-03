using BDAplication.Domain.Entities;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.Roles.AnyAsync())
        {
            var adminRole = new Role { Name = "Admin", Description = "Administrator", IsActive = true };
            var userRole = new Role { Name = "User", Description = "Standard User", IsActive = true };
            db.Roles.AddRange(adminRole, userRole);
            await db.SaveChangesAsync();

            // Seed admin user — password: Admin123!
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FullName = "System Administrator",
                Email = "admin@bdaplication.com",
                RoleId = adminRole.Id,
                IsActive = true
            };
            db.Users.Add(adminUser);
            await db.SaveChangesAsync();
        }
    }
}
