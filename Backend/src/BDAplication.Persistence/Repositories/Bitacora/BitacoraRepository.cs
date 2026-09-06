using BitacoraEntity = BDAplication.Domain.Entities.Bitacora.Bitacora;
using BDAplication.Domain.Entities.Bitacora;
using BDAplication.Domain.Interfaces.Bitacora;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories.Bitacora;

public class BitacoraRepository : IBitacoraRepository
{
    private readonly ApplicationDbContext _db;
    public BitacoraRepository(ApplicationDbContext db) => _db = db;

    private IQueryable<BitacoraEntity> WithGraph() =>
        _db.Bitacoras
            .Include(b => b.Actividades.Where(a => a.IsActive))
                .ThenInclude(a => a.Evidencias);

    public async Task<BitacoraEntity> GetOrCreateByUserAndDateAsync(int userId, DateTime fecha, string user)
    {
        var existing = await WithGraph()
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Fecha.Date == fecha.Date && b.IsActive);
        if (existing != null) return existing;

        var nuevo = new BitacoraEntity
        {
            UserId = userId,
            Fecha = fecha.Date,
            UserCreated = user
        };
        _db.Bitacoras.Add(nuevo);
        await _db.SaveChangesAsync();
        return nuevo;
    }

    public async Task<BitacoraEntity?> GetByIdAsync(int id) =>
        await WithGraph().FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IEnumerable<BitacoraEntity>> GetRangeAsync(int userId, DateTime desde, DateTime hasta, string? texto)
    {
        var q = WithGraph().Where(b => b.UserId == userId && b.IsActive &&
            b.Fecha.Date >= desde.Date && b.Fecha.Date <= hasta.Date);

        if (!string.IsNullOrWhiteSpace(texto))
            q = q.Where(b => b.Observacion.Contains(texto) ||
                             b.Actividades.Any(a => a.IsActive && a.Descripcion.Contains(texto)));

        return await q.OrderBy(b => b.Fecha).ToListAsync();
    }

    public async Task<BitacoraEntity> UpdateAsync(BitacoraEntity bitacora)
    {
        _db.Bitacoras.Update(bitacora);
        await _db.SaveChangesAsync();
        return bitacora;
    }

    // ── Actividad ────────────────────────────────────────────
    public async Task<BitacoraActividad?> GetActividadByIdAsync(int id) =>
        await _db.BitacoraActividades
            .Include(a => a.Bitacora)
            .Include(a => a.Evidencias)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<BitacoraActividad>> GetActividadesActivasByBitacoraIdAsync(int bitacoraId, int? excludeId = null)
    {
        var q = _db.BitacoraActividades.Where(a => a.BitacoraId == bitacoraId && a.IsActive);
        if (excludeId.HasValue) q = q.Where(a => a.Id != excludeId.Value);
        return await q.ToListAsync();
    }

    public async Task<BitacoraActividad> CreateActividadAsync(BitacoraActividad actividad)
    {
        _db.BitacoraActividades.Add(actividad);
        await _db.SaveChangesAsync();
        return actividad;
    }

    public async Task<BitacoraActividad> UpdateActividadAsync(BitacoraActividad actividad)
    {
        _db.BitacoraActividades.Update(actividad);
        await _db.SaveChangesAsync();
        return actividad;
    }

    public async Task DeleteActividadAsync(BitacoraActividad actividad)
    {
        actividad.IsActive = false;
        actividad.DateModified = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    // ── Evidencia ────────────────────────────────────────────
    public async Task<BitacoraEvidencia?> GetEvidenciaByIdAsync(int id) =>
        await _db.BitacoraEvidencias
            .Include(e => e.Actividad).ThenInclude(a => a.Bitacora)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<int> CountEvidenciasByActividadAsync(int actividadId) =>
        await _db.BitacoraEvidencias.CountAsync(e => e.BitacoraActividadId == actividadId);

    public async Task<BitacoraEvidencia> CreateEvidenciaAsync(BitacoraEvidencia evidencia)
    {
        _db.BitacoraEvidencias.Add(evidencia);
        await _db.SaveChangesAsync();
        return evidencia;
    }

    public async Task DeleteEvidenciaAsync(BitacoraEvidencia evidencia)
    {
        _db.BitacoraEvidencias.Remove(evidencia);
        await _db.SaveChangesAsync();
    }
}
