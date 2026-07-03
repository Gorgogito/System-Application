using BDAplication.Application.DTOs.Finance;

namespace BDAplication.Application.Interfaces.Finance;

public interface IAccountService
{
    Task<IEnumerable<AccountDto>> GetAllAsync();
    Task<AccountDto> GetByCodeAsync(string code);
    Task<AccountDto> CreateAsync(CreateAccountRequest request, string user);
    Task<AccountDto> UpdateAsync(UpdateAccountRequest request);
    Task DeleteAsync(int id);
}
