using BDAplication.Application.DTOs.Finance;

namespace BDAplication.Application.Interfaces.Finance;

public interface IAccountStatementService
{
    Task<AccountStatementDto> GetStatementAsync(AccountStatementRequest request);
}
