using BDAplication.Application.DTOs.Bitacora;

namespace BDAplication.Application.Interfaces.Bitacora;

/// <summary>
/// La Bitácora Diaria es un dato privado por usuario: toda operación valida que el recurso
/// (Bitacora / Actividad / Evidencia) pertenezca al usuario autenticado (userId), sin excepción
/// de rol — a diferencia del resto del sistema, aquí ni siquiera Admin ve datos de otro usuario.
/// </summary>
public interface IBitacoraService
{
    Task<BitacoraDto> GetByFechaAsync(DateTime fecha, int userId, string user);
    Task<BitacoraDto> UpdateObservacionAsync(int id, UpdateBitacoraRequest request, int userId, string user);
    Task<IEnumerable<BitacoraResumenDto>> GetResumenAsync(GetResumenRequest request, int userId);

    Task<BitacoraActividadDto> CreateActividadAsync(CreateActividadRequest request, int userId, string user);
    Task<BitacoraActividadDto> UpdateActividadAsync(UpdateActividadRequest request, int userId, string user);
    Task DeleteActividadAsync(int id, int userId);

    Task<BitacoraEvidenciaDto> UploadEvidenciaAsync(
        int actividadId, Stream fileStream, string fileName, string contentType, long fileSize,
        int userId, string user);

    Task<SasUploadResponse> GetSasUploadUrlAsync(int actividadId, SasUploadRequest request, int userId);
    Task<BitacoraEvidenciaDto> ConfirmEvidenciaAsync(ConfirmEvidenciaRequest request, int userId, string user);

    Task<string> GetEvidenciaUrlAsync(int evidenciaId, int userId);
    Task DeleteEvidenciaAsync(int evidenciaId, int userId);
}
