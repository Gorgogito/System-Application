namespace BDAplication.Application.DTOs.Finance;

public record MovementDto(
    int Id,
    string Code,
    int AccountId,
    string AccountCode,
    string AccountName,
    string Concept,
    int? TypeConceptId,
    string TypeConceptCode,
    string TypeConceptDescription,
    DateTime Date,
    decimal Amount,
    string Type,           // I / O
    string TypeLabel,      // Ingreso / Salida
    decimal Balance,
    bool IsTransfer,
    string TransferSourceDestiny,
    DateTime CreatedDate,
    string CreatedBy);

public record CreateMovementRequest(
    int AccountId,
    string Concept,
    int? TypeConceptId,
    DateTime Date,
    decimal Amount,
    string Type);          // I / O

public record UpdateMovementRequest(
    int Id,
    string Concept,
    int? TypeConceptId,
    DateTime Date,
    decimal Amount);

public record CreateTransferRequest(
    int SourceAccountId,
    int DestinyAccountId,
    DateTime Date,
    decimal Amount,
    string Concept);

public record ListMovementsByAccountRequest(int AccountId);
