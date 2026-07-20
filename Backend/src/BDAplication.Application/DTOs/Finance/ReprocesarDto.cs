namespace BDAplication.Application.DTOs.Finance;

public record ReprocesarRequest(
    List<int>? AccountIds,
    bool IsSimulation);

public record MovementDiffDto(
    int MovementId,
    string MovementCode,
    DateTime Date,
    string Concept,
    decimal CurrentBalance,
    decimal ExpectedBalance,
    decimal Difference);

public record AccountReprocesarResultDto(
    int AccountId,
    string AccountCode,
    string AccountName,
    bool HasInconsistency,
    int MovementsChecked,
    int DiffsCount,
    List<MovementDiffDto> Diffs);

public record ReprocesarResultDto(
    int LogId,
    int AccountsProcessed,
    int InconsistenciesFound,
    int MovementsRecalculated,
    bool IsSimulation,
    DateTime ExecutedAt,
    List<AccountReprocesarResultDto> AccountResults);

public record ReprocesarLogDto(
    int Id,
    string ExecutedBy,
    DateTime ExecutedAt,
    bool IsSimulation,
    int AccountsProcessed,
    int MovementsRecalculated,
    int InconsistenciesFound);
