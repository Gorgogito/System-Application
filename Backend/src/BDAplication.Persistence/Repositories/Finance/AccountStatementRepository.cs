using BDAplication.Domain.Entities.Finance;
using BDAplication.Domain.Interfaces.Finance;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories.Finance;

public class AccountStatementRepository : IAccountStatementRepository
{
    private readonly ApplicationDbContext _db;
    public AccountStatementRepository(ApplicationDbContext db) => _db = db;

    public async Task<Account?> GetAccountAsync(int accountId) =>
        await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == accountId);

    public async Task<decimal> GetPreviousBalanceAsync(int accountId, DateTime startDate)
    {
        // El campo Balance de cada movimiento contiene el saldo acumulado después de ese movimiento.
        // El "saldo anterior" es el Balance del último movimiento antes de startDate.
        var balance = await _db.Movements
            .AsNoTracking()
            .Where(m => m.AccountId == accountId && m.Date < startDate)
            .OrderByDescending(m => m.Date)
            .ThenByDescending(m => m.Id)
            .Select(m => (decimal?)m.Balance)
            .FirstOrDefaultAsync();

        return balance ?? 0m;
    }

    public async Task<IEnumerable<Movement>> GetMovementsInRangeAsync(
        int accountId, DateTime startDate, DateTime endDate)
    {
        var endInclusive = endDate.Date.AddDays(1);

        return await _db.Movements
            .AsNoTracking()
            .Include(m => m.TypeConcept)
            .Where(m => m.AccountId == accountId &&
                        m.Date >= startDate &&
                        m.Date < endInclusive)
            .OrderBy(m => m.Date)
            .ThenBy(m => m.Id)
            .ToListAsync();
    }
}
