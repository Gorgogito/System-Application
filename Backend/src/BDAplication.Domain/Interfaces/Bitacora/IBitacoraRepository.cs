using BitacoraEntity = BDAplication.Domain.Entities.Bitacora.Bitacora;
using BDAplication.Domain.Entities.Bitacora;

namespace BDAplication.Domain.Interfaces.Bitacora;

public interface IBitacoraRepository
{
    // Día
    Task<BitacoraEntity> GetOrCreateByUserAndDateAsync(int userId, DateTime fecha, string user);
    Task<BitacoraEntity?> GetByIdAsync(int id);
    Task<IEnumerable<BitacoraEntity>> GetRangeAsync(int userId, DateTime desde, DateTime hasta, string? texto);
    Task<BitacoraEntity> UpdateAsync(BitacoraEntity bitacora);

    // Actividad
    Task<BitacoraActividad?> GetActividadByIdAsync(int id);
    Task<IEnumerable<BitacoraActividad>> GetActividadesActivasByBitacoraIdAsync(int bitacoraId, int? excludeId = null);
    Task<BitacoraActividad> CreateActividadAsync(BitacoraActividad actividad);
    Task<BitacoraActividad> UpdateActividadAsync(BitacoraActividad actividad);
    Task DeleteActividadAsync(BitacoraActividad actividad);

    // Evidencia
    Task<BitacoraEvidencia?> GetEvidenciaByIdAsync(int id);
    Task<int> CountEvidenciasByActividadAsync(int actividadId);
    Task<BitacoraEvidencia> CreateEvidenciaAsync(BitacoraEvidencia evidencia);
    Task DeleteEvidenciaAsync(BitacoraEvidencia evidencia);
}
