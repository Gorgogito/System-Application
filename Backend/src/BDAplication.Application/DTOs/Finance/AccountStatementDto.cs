namespace BDAplication.Application.DTOs.Finance;

public record AccountStatementRequest(
    int      AccountId,
    DateTime StartDate,
    DateTime EndDate);

public record AccountStatementDto(
    AccountStatementAccountInfo  Account,
    decimal                      PreviousBalance,
    IEnumerable<AccountStatementLineDto> Lines,
    decimal                      TotalDebit,
    decimal                      TotalCredit,
    decimal                      FinalBalance);

public record AccountStatementAccountInfo(
    int    Id,
    string Code,
    string Name,
    string Description);

public record AccountStatementLineDto(
    DateTime Date,
    string   DocumentCode,
    string   DocumentType,
    string   Concept,
    string   TypeConcept,
    decimal  Debit,
    decimal  Credit,
    decimal  Balance,
    bool     IsTransfer);
