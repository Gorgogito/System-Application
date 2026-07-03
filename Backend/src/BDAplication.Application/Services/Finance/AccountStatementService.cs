using BDAplication.Application.DTOs.Finance;
using BDAplication.Application.Interfaces.Finance;
using BDAplication.Domain.Interfaces.Finance;

namespace BDAplication.Application.Services.Finance;

public class AccountStatementService : IAccountStatementService
{
    private readonly IAccountStatementRepository _repo;

    public AccountStatementService(IAccountStatementRepository repo) => _repo = repo;

    public async Task<AccountStatementDto> GetStatementAsync(AccountStatementRequest request)
    {
        if (request.EndDate < request.StartDate)
            throw new ArgumentException("La fecha final no puede ser anterior a la fecha inicial.");

        var account = await _repo.GetAccountAsync(request.AccountId)
            ?? throw new KeyNotFoundException($"Cuenta {request.AccountId} no encontrada.");

        var startDay = request.StartDate.Date;
        var endDay   = request.EndDate.Date;

        var previousBalance = await _repo.GetPreviousBalanceAsync(request.AccountId, startDay);
        var movements       = await _repo.GetMovementsInRangeAsync(request.AccountId, startDay, endDay);

        decimal totalDebit  = 0m;
        decimal totalCredit = 0m;
        var lines = new List<AccountStatementLineDto>();

        foreach (var m in movements)
        {
            decimal debit  = m.Type == "O" ? m.Amount : 0m;
            decimal credit = m.Type == "I" ? m.Amount : 0m;
            totalDebit  += debit;
            totalCredit += credit;

            string docType = m.IsTransfer
                ? m.TransferSourceDestiny == "O" ? "Transferencia Salida"
                : m.TransferSourceDestiny == "D" ? "Transferencia Entrada"
                : "Transferencia"
                : "Movimiento";

            lines.Add(new AccountStatementLineDto(
                Date:         m.Date,
                DocumentCode: m.Code,
                DocumentType: docType,
                Concept:      m.Concept,
                TypeConcept:  m.TypeConcept?.Description ?? string.Empty,
                Debit:        debit,
                Credit:       credit,
                Balance:      m.Balance,
                IsTransfer:   m.IsTransfer
            ));
        }

        decimal finalBalance = lines.Count > 0
            ? lines[^1].Balance
            : previousBalance;

        return new AccountStatementDto(
            Account: new AccountStatementAccountInfo(
                account.Id, account.Code, account.Name, account.Description ?? string.Empty),
            PreviousBalance: previousBalance,
            Lines:           lines,
            TotalDebit:      totalDebit,
            TotalCredit:     totalCredit,
            FinalBalance:    finalBalance
        );
    }
}
