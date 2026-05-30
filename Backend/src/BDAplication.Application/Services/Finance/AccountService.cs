using BDAplication.Application.DTOs.Finance;
using BDAplication.Application.Interfaces.Finance;
using BDAplication.Domain.Entities.Finance;
using BDAplication.Domain.Interfaces.Finance;

namespace BDAplication.Application.Services.Finance;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repo;
    public AccountService(IAccountRepository repo) => _repo = repo;

    public async Task<IEnumerable<AccountDto>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(ToDto);

    public async Task<AccountDto> GetByCodeAsync(string code)
    {
        var e = await _repo.GetByCodeAsync(code) ?? throw new KeyNotFoundException($"Account '{code}' not found");
        return ToDto(e);
    }

    public async Task<AccountDto> CreateAsync(CreateAccountRequest request, string user)
    {
        var code = await _repo.GenerateCodeAsync();
        var entity = new Account
        {
            Code = code,
            Name = request.Name,
            Description = request.Description,
            CreatedBy = user
        };
        return ToDto(await _repo.CreateAsync(entity));
    }

    public async Task<AccountDto> UpdateAsync(UpdateAccountRequest request)
    {
        var entity = await _repo.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException($"Account {request.Id} not found");
        entity.Name = request.Name;
        entity.Description = request.Description;
        return ToDto(await _repo.UpdateAsync(entity));
    }

    public async Task DeleteAsync(int id)
    {
        if (await _repo.HasMovementsAsync(id))
            throw new InvalidOperationException("No se puede eliminar una cuenta con movimientos registrados.");
        await _repo.DeleteAsync(id);
    }

    private static AccountDto ToDto(Account a) =>
        new(a.Id, a.Code, a.Name, a.Description, a.Balance, a.CreatedDate, a.CreatedBy);
}
