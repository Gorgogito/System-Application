using BDAplication.Domain.Entities;
using BDAplication.Domain.Entities.Finance;
using BDAplication.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Backlog> Backlogs => Set<Backlog>();
    public DbSet<Planner> Planners => Set<Planner>();
    public DbSet<BoardTask> BoardTasks => Set<BoardTask>();
    public DbSet<SubTask> SubTasks => Set<SubTask>();

    // Finance
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<TypeConcept> TypeConcepts => Set<TypeConcept>();
    public DbSet<Movement> Movements => Set<Movement>();
    public DbSet<Transfer> Transfers => Set<Transfer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.Property(x => x.Description).HasMaxLength(200);
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).IsRequired().HasMaxLength(50);
            e.Property(x => x.Email).IsRequired().HasMaxLength(150);
            e.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            e.Property(x => x.PasswordHash).IsRequired();
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
            e.HasOne(x => x.Role)
             .WithMany(r => r.Users)
             .HasForeignKey(x => x.RoleId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Backlog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Priority).HasConversion<int>();
            e.Property(x => x.UserCreated).HasMaxLength(50);
        });

        modelBuilder.Entity<Planner>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Notes).HasMaxLength(500);
            e.Property(x => x.UserCreated).HasMaxLength(50);
            e.HasOne(x => x.Backlog)
             .WithMany(b => b.Planners)
             .HasForeignKey(x => x.BacklogId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.Month, x.Year });
        });

        modelBuilder.Entity<BoardTask>(e =>
        {
            e.HasKey(x => x.Id);
            e.ToTable("TaskBoard");
            e.Property(x => x.Title).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.Priority).HasConversion<int>();
            e.Property(x => x.Status).HasConversion<int>();
            e.Property(x => x.UserCreated).HasMaxLength(50);
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<SubTask>(e =>
        {
            e.HasKey(x => x.Id);
            e.ToTable("SubTask");
            e.Property(x => x.Title).IsRequired().HasMaxLength(300);
            e.Property(x => x.UserCreated).HasMaxLength(50);
            e.HasOne(x => x.BoardTask)
             .WithMany(t => t.SubTasks)
             .HasForeignKey(x => x.BoardTaskId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Finance ──────────────────────────────────────────────
        modelBuilder.Entity<Account>(e =>
        {
            e.HasKey(x => x.Id);
            e.ToTable("account");
            e.Property(x => x.Code).IsRequired().HasMaxLength(12);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.Balance).HasColumnType("decimal(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(50);
            e.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<TypeConcept>(e =>
        {
            e.HasKey(x => x.Id);
            e.ToTable("typeconcept");
            e.Property(x => x.Code).IsRequired().HasMaxLength(12);
            e.Property(x => x.Description).IsRequired().HasMaxLength(100);
            e.Property(x => x.CreatedBy).HasMaxLength(50);
            e.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<Movement>(e =>
        {
            e.HasKey(x => x.Id);
            e.ToTable("movement");
            e.Property(x => x.Code).IsRequired().HasMaxLength(12);
            e.Property(x => x.Concept).IsRequired().HasMaxLength(300);
            e.Property(x => x.Type).IsRequired().HasMaxLength(1);
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.Property(x => x.Balance).HasColumnType("decimal(18,2)");
            e.Property(x => x.TransferSourceDestiny).HasMaxLength(1);
            e.Property(x => x.CreatedBy).HasMaxLength(50);
            e.HasIndex(x => x.Code).IsUnique();
            e.HasOne(x => x.Account)
             .WithMany(a => a.Movements)
             .HasForeignKey(x => x.AccountId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.TypeConcept)
             .WithMany()
             .HasForeignKey(x => x.TypeConceptId)
             .OnDelete(DeleteBehavior.SetNull)
             .IsRequired(false);
        });

        modelBuilder.Entity<Transfer>(e =>
        {
            e.HasKey(x => x.Id);
            e.ToTable("transfer");
            e.Property(x => x.CreatedBy).HasMaxLength(50);
            e.HasOne(x => x.SourceAccount).WithMany().HasForeignKey(x => x.SourceAccountId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.DestinyAccount).WithMany().HasForeignKey(x => x.DestinyAccountId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.SourceMovement).WithMany().HasForeignKey(x => x.SourceMovementId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.DestinyMovement).WithMany().HasForeignKey(x => x.DestinyMovementId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
