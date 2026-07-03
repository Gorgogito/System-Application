using BDAplication.Domain.Entities.Finance;
using BDAplication.Domain.Interfaces.Finance;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories.Finance;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _db;
    public AccountRepository(ApplicationDbContext db) => _db = db;

    public async Task<IEnumerable<Account>> GetAllAsync() =>
        await _db.Accounts.OrderBy(a => a.Code).ToListAsync();

    public async Task<Account?> GetByCodeAsync(string code) =>
        await _db.Accounts.FirstOrDefaultAsync(a => a.Code == code);

    public async Task<Account?> GetByIdAsync(int id) =>
        await _db.Accounts.FindAsync(id);

    public async Task<Account> CreateAsync(Account account)
    {
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();
        return account;
    }

    public async Task<Account> UpdateAsync(Account account)
    {
        _db.Accounts.Update(account);
        await _db.SaveChangesAsync();
        return account;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Accounts.FindAsync(id) ?? throw new KeyNotFoundException();
        _db.Accounts.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<string> GenerateCodeAsync()
    {
        var last = await _db.Accounts
            .OrderByDescending(a => a.Code)
            .Select(a => (string?)a.Code)
            .FirstOrDefaultAsync();
        if (last == null) return "AC00000001";
        var num = int.Parse(last[2..]) + 1;
        return $"AC{num:D8}";
    }

    public async Task<bool> HasMovementsAsync(int id) =>
        await _db.Movements.AnyAsync(m => m.AccountId == id);
}
