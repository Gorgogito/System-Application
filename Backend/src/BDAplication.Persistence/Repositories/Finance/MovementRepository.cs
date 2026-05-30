using BDAplication.Domain.Entities.Finance;
using BDAplication.Domain.Interfaces.Finance;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories.Finance;

public class MovementRepository : IMovementRepository
{
    private readonly ApplicationDbContext _db;
    public MovementRepository(ApplicationDbContext db) => _db = db;

    private IQueryable<Movement> WithNav() =>
        _db.Movements
           .Include(m => m.Account)
           .Include(m => m.TypeConcept);

    public async Task<IEnumerable<Movement>> GetByAccountIdAsync(int accountId) =>
        await WithNav()
            .Where(m => m.AccountId == accountId)
            .OrderBy(m => m.Date).ThenBy(m => m.Id)
            .ToListAsync();

    public async Task<Movement?> GetByCodeAsync(string code) =>
        await WithNav().FirstOrDefaultAsync(m => m.Code == code);

    public async Task<Movement?> GetByIdAsync(int id) =>
        await WithNav().FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Movement> CreateAsync(Movement movement)
    {
        _db.Movements.Add(movement);
        await _db.SaveChangesAsync();
        return movement;
    }

    public async Task<Movement> UpdateAsync(Movement movement)
    {
        _db.Movements.Update(movement);
        await _db.SaveChangesAsync();
        return movement;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Movements.FindAsync(id) ?? throw new KeyNotFoundException();
        _db.Movements.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task CreateTransferAsync(Movement source, Movement destiny, Transfer transfer)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            _db.Movements.Add(source);
            await _db.SaveChangesAsync();

            _db.Movements.Add(destiny);
            await _db.SaveChangesAsync();

            transfer.SourceMovementId = source.Id;
            transfer.DestinyMovementId = destiny.Id;
            _db.Transfers.Add(transfer);
            await _db.SaveChangesAsync();

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task RecalculateAccountAsync(int accountId)
    {
        var movements = await _db.Movements
            .Where(m => m.AccountId == accountId)
            .OrderBy(m => m.Date).ThenBy(m => m.Id)
            .ToListAsync();

        decimal running = 0;
        foreach (var m in movements)
            running = m.Type == "I" ? running + m.Amount : running - m.Amount;
        // Store balance in each movement
        running = 0;
        foreach (var m in movements)
        {
            running = m.Type == "I" ? running + m.Amount : running - m.Amount;
            m.Balance = running;
        }

        var account = await _db.Accounts.FindAsync(accountId);
        if (account != null)
            account.Balance = running;

        await _db.SaveChangesAsync();
    }

    public async Task<string> GenerateCodeAsync()
    {
        var last = await _db.Movements
            .OrderByDescending(m => m.Code)
            .Select(m => (string?)m.Code)
            .FirstOrDefaultAsync();
        if (last == null) return "MV00000001";
        var num = int.Parse(last[2..]) + 1;
        return $"MV{num:D8}";
    }
}
