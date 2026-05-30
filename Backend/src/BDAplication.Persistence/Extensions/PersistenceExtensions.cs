using BDAplication.Application.Interfaces;
using BDAplication.Application.Interfaces.Finance;
using BDAplication.Application.Interfaces.TaskBoard;
using BDAplication.Application.Interfaces.TaskPlanner;
using BDAplication.Application.Services;
using BDAplication.Application.Services.Finance;
using BDAplication.Application.Services.TaskBoard;
using BDAplication.Application.Services.TaskPlanner;
using BDAplication.Domain.Interfaces;
using BDAplication.Domain.Interfaces.Finance;
using BDAplication.Persistence.Context;
using BDAplication.Persistence.Repositories;
using BDAplication.Persistence.Repositories.Finance;
using BDAplication.Persistence.Repositories.TaskBoard;
using BDAplication.Persistence.Repositories.TaskPlanner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BDAplication.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        // User Management
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IAuthService, AuthService>();

        // TaskPlanner
        services.AddScoped<IBacklogRepository, BacklogRepository>();
        services.AddScoped<IPlannerRepository, PlannerRepository>();
        services.AddScoped<IBacklogService, BacklogService>();
        services.AddScoped<IPlannerService, PlannerService>();

        // TaskBoard
        services.AddScoped<ITaskBoardRepository, TaskBoardRepository>();
        services.AddScoped<ITaskBoardService, TaskBoardService>();

        // Finance
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITypeConceptRepository, TypeConceptRepository>();
        services.AddScoped<IMovementRepository, MovementRepository>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITypeConceptService, TypeConceptService>();
        services.AddScoped<IMovementService, MovementService>();

        return services;
    }
}
