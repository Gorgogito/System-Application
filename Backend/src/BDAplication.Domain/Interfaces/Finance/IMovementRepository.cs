using BDAplication.Domain.Entities.Finance;

namespace BDAplication.Domain.Interfaces.Finance;

public interface IMovementRepository
{
    Task<IEnumerable<Movement>> GetByAccountIdAsync(int accountId);
    Task<Movement?> GetByCodeAsync(string code);
    Task<Movement?> GetByIdAsync(int id);
    Task<Movement> CreateAsync(Movement movement);
    Task<Movement> UpdateAsync(Movement movement);
    Task DeleteAsync(int id);
    Task CreateTransferAsync(Movement source, Movement destiny, Transfer transfer);
    Task RecalculateAccountAsync(int accountId);
    Task<string> GenerateCodeAsync();
}
