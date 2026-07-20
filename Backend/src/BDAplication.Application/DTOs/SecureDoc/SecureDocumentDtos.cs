namespace BDAplication.Application.DTOs.SecureDoc;

// ── Listas y cabeceras ─────────────────────────────────────────────
public record SecureDocumentListItemDto(
    int Id,
    string Title,
    int VersionCount,
    DateTime LastModified,
    string CreatedBy,
    bool IsActive);

// ── Versión (contenido siempre cifrado) ───────────────────────────
public record SecureDocumentVersionDto(
    int Id,
    int VersionNumber,
    string EncryptedContent,
    string Observation,
    DateTime DocumentDate,
    DateTime CreatedAt,
    string CreatedBy);

// ── Documento completo ────────────────────────────────────────────
public record SecureDocumentDto(
    int Id,
    string Title,
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy,
    List<SecureDocumentVersionDto> Versions);

// ── Descifrado (contenido desencriptado, solo en memoria) ─────────
public record DecryptedContentDto(
    int DocumentId,
    int VersionId,
    int VersionNumber,
    string HtmlContent,
    string Observation,
    DateTime DocumentDate,
    string DecryptedBy);

// ── Requests ──────────────────────────────────────────────────────
public record CreateSecureDocumentRequest(
    string Title,
    string HtmlContent,
    string Observation,
    DateTime DocumentDate);

public record AddVersionRequest(
    string HtmlContent,
    string Observation,
    DateTime DocumentDate);

public record UpdateTitleRequest(string Title);

public record DecryptRequest(
    string Username,
    string Password,
    int? VersionId = null);
