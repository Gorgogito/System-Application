using BDAplication.Application.DTOs.Finance;
using BDAplication.Application.Interfaces.Finance;
using BDAplication.Domain.Entities.Finance;
using BDAplication.Domain.Interfaces.Finance;

namespace BDAplication.Application.Services.Finance;

public class MovementService : IMovementService
{
    private readonly IMovementRepository _repo;
    private readonly IAccountRepository _accountRepo;

    public MovementService(IMovementRepository repo, IAccountRepository accountRepo)
    {
        _repo = repo;
        _accountRepo = accountRepo;
    }

    public async Task<IEnumerable<MovementDto>> GetByAccountAsync(int accountId) =>
        (await _repo.GetByAccountIdAsync(accountId)).Select(ToDto);

    public async Task<MovementDto> GetByCodeAsync(string code)
    {
        var e = await _repo.GetByCodeAsync(code) ?? throw new KeyNotFoundException($"Movement '{code}' not found");
        return ToDto(e);
    }

    public async Task<MovementDto> CreateAsync(CreateMovementRequest request, string user)
    {
        if (request.Type != "I" && request.Type != "O")
            throw new ArgumentException("El tipo de movimiento debe ser 'I' (Ingreso) u 'O' (Salida).");

        var account = await _accountRepo.GetByIdAsync(request.AccountId)
            ?? throw new KeyNotFoundException($"Account {request.AccountId} not found");

        var code = await _repo.GenerateCodeAsync();
        var entity = new Movement
        {
            Code = code,
            AccountId = request.AccountId,
            Concept = request.Concept,
            TypeConceptId = request.TypeConceptId,
            Date = request.Date.Date,
            Amount = request.Amount,
            Type = request.Type,
            Balance = 0,
            IsTransfer = false,
            TransferSourceDestiny = "X",
            CreatedBy = user
        };

        await _repo.CreateAsync(entity);
        await _repo.RecalculateAccountAsync(account.Id);

        var saved = await _repo.GetByCodeAsync(code) ?? entity;
        return ToDto(saved);
    }

    public async Task<MovementDto> UpdateAsync(UpdateMovementRequest request, string user)
    {
        var entity = await _repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Movement {request.Id} not found");

        if (entity.IsTransfer)
            throw new InvalidOperationException("No se pueden editar movimientos de transferencia.");

        entity.Concept = request.Concept;
        entity.TypeConceptId = request.TypeConceptId;
        entity.Date = request.Date.Date;
        entity.Amount = request.Amount;

        await _repo.UpdateAsync(entity);
        await _repo.RecalculateAccountAsync(entity.AccountId);

        var saved = await _repo.GetByIdAsync(request.Id) ?? entity;
        return ToDto(saved);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Movement {id} not found");

        if (entity.IsTransfer)
            throw new InvalidOperationException("No se pueden eliminar movimientos de transferencia.");

        var accountId = entity.AccountId;
        await _repo.DeleteAsync(id);
        await _repo.RecalculateAccountAsync(accountId);
    }

    public async Task<(MovementDto Source, MovementDto Destiny)> CreateTransferAsync(
        CreateTransferRequest request, string user)
    {
        if (request.SourceAccountId == request.DestinyAccountId)
            throw new ArgumentException("La cuenta origen y destino no pueden ser la misma.");
        if (request.Amount <= 0)
            throw new ArgumentException("El importe de la transferencia debe ser mayor a cero.");

        _ = await _accountRepo.GetByIdAsync(request.SourceAccountId)
            ?? throw new KeyNotFoundException($"Cuenta origen {request.SourceAccountId} not found");
        _ = await _accountRepo.GetByIdAsync(request.DestinyAccountId)
            ?? throw new KeyNotFoundException($"Cuenta destino {request.DestinyAccountId} not found");

        var sourceCode = await _repo.GenerateCodeAsync();
        var source = new Movement
        {
            Code = sourceCode,
            AccountId = request.SourceAccountId,
            Concept = request.Concept,
            Date = request.Date.Date,
            Amount = request.Amount,
            Type = "O",
            Balance = 0,
            IsTransfer = true,
            TransferSourceDestiny = "O",
            CreatedBy = user
        };

        // Reserve next code for destiny
        var destinyCode = $"MV{(int.Parse(sourceCode[2..]) + 1):D8}";
        var destiny = new Movement
        {
            Code = destinyCode,
            AccountId = request.DestinyAccountId,
            Concept = request.Concept,
            Date = request.Date.Date,
            Amount = request.Amount,
            Type = "I",
            Balance = 0,
            IsTransfer = true,
            TransferSourceDestiny = "D",
            CreatedBy = user
        };

        var transfer = new Transfer
        {
            SourceAccountId = request.SourceAccountId,
            DestinyAccountId = request.DestinyAccountId,
            CreatedBy = user
        };

        await _repo.CreateTransferAsync(source, destiny, transfer);
        await _repo.RecalculateAccountAsync(request.SourceAccountId);
        await _repo.RecalculateAccountAsync(request.DestinyAccountId);

        var savedSource = await _repo.GetByCodeAsync(sourceCode) ?? source;
        var savedDestiny = await _repo.GetByCodeAsync(destinyCode) ?? destiny;
        return (ToDto(savedSource), ToDto(savedDestiny));
    }

    private static MovementDto ToDto(Movement m) =>
        new(m.Id, m.Code,
            m.AccountId, m.Account?.Code ?? string.Empty, m.Account?.Name ?? string.Empty,
            m.Concept,
            m.TypeConceptId, m.TypeConcept?.Code ?? string.Empty, m.TypeConcept?.Description ?? string.Empty,
            m.Date, m.Amount,
            m.Type, m.Type == "I" ? "Ingreso" : "Salida",
            m.Balance, m.IsTransfer, m.TransferSourceDestiny,
            m.CreatedDate, m.CreatedBy);
}
