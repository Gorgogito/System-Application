using BDAplication.Domain.Entities.Finance;

namespace BDAplication.Domain.Interfaces.Finance;

public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetAllAsync();
    Task<Account?> GetByCodeAsync(string code);
    Task<Account?> GetByIdAsync(int id);
    Task<Account> CreateAsync(Account account);
    Task<Account> UpdateAsync(Account account);
    Task DeleteAsync(int id);
    Task<string> GenerateCodeAsync();
    Task<bool> HasMovementsAsync(int id);
}
