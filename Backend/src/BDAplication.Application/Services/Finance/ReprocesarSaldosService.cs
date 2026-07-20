using System.Text.Json;
using BDAplication.Application.DTOs.Finance;
using BDAplication.Application.Interfaces.Finance;
using BDAplication.Domain.Entities.Finance;
using BDAplication.Domain.Interfaces.Finance;

namespace BDAplication.Application.Services.Finance;

public class ReprocesarSaldosService : IReprocesarSaldosService
{
    private readonly IAccountRepository _accountRepo;
    private readonly IMovementRepository _movementRepo;
    private readonly IReprocessLogRepository _logRepo;

    public ReprocesarSaldosService(
        IAccountRepository accountRepo,
        IMovementRepository movementRepo,
        IReprocessLogRepository logRepo)
    {
        _accountRepo = accountRepo;
        _movementRepo = movementRepo;
        _logRepo = logRepo;
    }

    public async Task<ReprocesarResultDto> ExecuteAsync(ReprocesarRequest request, string user)
    {
        var allAccounts = (await _accountRepo.GetAllAsync()).ToList();
        var accounts = request.AccountIds?.Any() == true
            ? allAccounts.Where(a => request.AccountIds.Contains(a.Id)).ToList()
            : allAccounts;

        var accountResults = new List<AccountReprocesarResultDto>();
        int totalMovementsRecalculated = 0;
        int totalInconsistencies = 0;

        foreach (var account in accounts)
        {
            var movements = (await _movementRepo.GetByAccountIdAsync(account.Id)).ToList();

            decimal running = 0;
            var diffs = new List<MovementDiffDto>();

            foreach (var m in movements)
            {
                running = m.Type == "I" ? running + m.Amount : running - m.Amount;
                if (m.Balance != running)
                    diffs.Add(new MovementDiffDto(m.Id, m.Code, m.Date, m.Concept, m.Balance, running, running - m.Balance));
            }

            bool hasInconsistency = diffs.Any();
            if (hasInconsistency) totalInconsistencies++;

            if (!request.IsSimulation && hasInconsistency)
            {
                await _movementRepo.RecalculateAccountAsync(account.Id);
                totalMovementsRecalculated += diffs.Count;
            }

            accountResults.Add(new AccountReprocesarResultDto(
                account.Id, account.Code, account.Name,
                hasInconsistency, movements.Count, diffs.Count, diffs));
        }

        var logDetails = accountResults
            .Select(r => new { r.AccountId, r.AccountCode, r.DiffsCount, r.HasInconsistency });

        var log = new ReprocessLog
        {
            ExecutedBy = user,
            ExecutedAt = DateTime.UtcNow,
            IsSimulation = request.IsSimulation,
            AccountsProcessed = accounts.Count,
            MovementsRecalculated = totalMovementsRecalculated,
            InconsistenciesFound = totalInconsistencies,
            AccountIdsJson = JsonSerializer.Serialize(accounts.Select(a => a.Id)),
            LogDetailsJson = JsonSerializer.Serialize(logDetails)
        };

        var savedLog = await _logRepo.CreateAsync(log);

        return new ReprocesarResultDto(
            savedLog.Id,
            accounts.Count,
            totalInconsistencies,
            totalMovementsRecalculated,
            request.IsSimulation,
            savedLog.ExecutedAt,
            accountResults);
    }

    public async Task<IEnumerable<ReprocesarLogDto>> GetHistoryAsync(int limit = 20)
    {
        var logs = await _logRepo.GetRecentAsync(limit);
        return logs.Select(l => new ReprocesarLogDto(
            l.Id, l.ExecutedBy, l.ExecutedAt, l.IsSimulation,
            l.AccountsProcessed, l.MovementsRecalculated, l.InconsistenciesFound));
    }
}
