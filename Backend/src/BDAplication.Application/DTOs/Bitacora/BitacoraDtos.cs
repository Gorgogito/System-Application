namespace BDAplication.Application.DTOs.Bitacora;

public record BitacoraEvidenciaDto(
    int Id,
    int BitacoraActividadId,
    string NombreOriginal,
    string ContentType,
    string Extension,
    long TamanoBytes,
    string Tipo,
    string UserCreated,
    DateTime DateCreated);

public record BitacoraActividadDto(
    int Id,
    int BitacoraId,
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    string Descripcion,
    string UserCreated,
    DateTime DateCreated,
    string? UserModified,
    DateTime? DateModified,
    IEnumerable<BitacoraEvidenciaDto> Evidencias);

public record BitacoraDto(
    int Id,
    int UserId,
    DateTime Fecha,
    string Observacion,
    string UserCreated,
    DateTime DateCreated,
    string? UserModified,
    DateTime? DateModified,
    IEnumerable<BitacoraActividadDto> Actividades);

public record BitacoraResumenDto(
    DateTime Fecha,
    int TotalActividades,
    int TotalImagenes,
    int TotalVideos);

public record UpdateBitacoraRequest(string? Observacion);

public record CreateActividadRequest(
    int BitacoraId,
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    string Descripcion);

public record UpdateActividadRequest(
    int Id,
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    string Descripcion);

public record GetResumenRequest(DateTime Desde, DateTime Hasta, string? Texto);

public record SasUploadRequest(string FileName, string ContentType, long DeclaredSizeBytes);

public record SasUploadResponse(string UploadUrl, string BlobPath, DateTime ExpiresAt);

public record ConfirmEvidenciaRequest(
    int BitacoraActividadId,
    string BlobPath,
    string NombreOriginal,
    long DeclaredSizeBytes);
