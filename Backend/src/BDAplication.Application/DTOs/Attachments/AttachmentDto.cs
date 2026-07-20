namespace BDAplication.Application.DTOs.Attachments;

public record AttachmentDto(
    int Id,
    string EntityType,
    int EntityId,
    string OriginalName,
    string BlobPath,
    string ContentType,
    string Extension,
    long FileSizeBytes,
    string Description,
    string Comment,
    DateTime DocumentDate,
    int? DocumentConceptId,
    string? DocumentConceptName,
    decimal? Amount,
    string UserCreated,
    DateTime DateCreated,
    string? UserModified,
    DateTime? DateModified
);
