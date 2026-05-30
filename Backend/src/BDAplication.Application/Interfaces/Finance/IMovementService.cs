using BDAplication.Application.DTOs.Finance;

namespace BDAplication.Application.Interfaces.Finance;

public interface IMovementService
{
    Task<IEnumerable<MovementDto>> GetByAccountAsync(int accountId);
    Task<MovementDto> GetByCodeAsync(string code);
    Task<MovementDto> CreateAsync(CreateMovementRequest request, string user);
    Task<MovementDto> UpdateAsync(UpdateMovementRequest request, string user);
    Task DeleteAsync(int id);
    Task<(MovementDto Source, MovementDto Destiny)> CreateTransferAsync(CreateTransferRequest request, string user);
}
