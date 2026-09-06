using BDAplication.Application.DTOs.Bitacora;
using BDAplication.Application.Interfaces;
using BDAplication.Application.Interfaces.Bitacora;
using BDAplication.Domain.Entities.Bitacora;
using BDAplication.Domain.Enums;
using BDAplication.Domain.Interfaces.Bitacora;
using Microsoft.Extensions.Configuration;

namespace BDAplication.Application.Services.Bitacora;

public class BitacoraService : IBitacoraService
{
    private readonly IBitacoraRepository _repo;
    private readonly IBlobStorageService _blob;
    private readonly long _maxImageSizeBytes;
    private readonly long _maxVideoSizeBytes;
    private readonly int _maxEvidenciasPorActividad;

    public BitacoraService(IBitacoraRepository repo, IBlobStorageService blob, IConfiguration config)
    {
        _repo = repo;
        _blob = blob;
        _maxImageSizeBytes = config.GetValue<long>("Bitacora:MaxImageSizeBytes", 15_728_640);      // 15 MB
        _maxVideoSizeBytes = config.GetValue<long>("Bitacora:MaxVideoSizeBytes", 314_572_800);      // 300 MB
        _maxEvidenciasPorActividad = config.GetValue<int>("Bitacora:MaxEvidenciasPorActividad", 20);
    }

    // ── Día ──────────────────────────────────────────────────
    public async Task<BitacoraDto> GetByFechaAsync(DateTime fecha, int userId, string user)
    {
        var bitacora = await _repo.GetOrCreateByUserAndDateAsync(userId, fecha, user);
        return ToDto(bitacora);
    }

    public async Task<BitacoraDto> UpdateObservacionAsync(int id, UpdateBitacoraRequest request, int userId, string user)
    {
        var bitacora = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Bitácora {id} no encontrada");
        EnsureOwnership(bitacora.UserId, userId);

        bitacora.Observacion = request.Observacion ?? string.Empty;
        bitacora.UserModified = user;
        bitacora.DateModified = DateTime.UtcNow;

        var updated = await _repo.UpdateAsync(bitacora);
        return ToDto(updated);
    }

    public async Task<IEnumerable<BitacoraResumenDto>> GetResumenAsync(GetResumenRequest request, int userId)
    {
        var dias = await _repo.GetRangeAsync(userId, request.Desde, request.Hasta, request.Texto);
        return dias.Select(b =>
        {
            var evidencias = b.Actividades.Where(a => a.IsActive).SelectMany(a => a.Evidencias).ToList();
            return new BitacoraResumenDto(
                b.Fecha,
                b.Actividades.Count(a => a.IsActive),
                evidencias.Count(e => e.Tipo == TipoEvidencia.Imagen),
                evidencias.Count(e => e.Tipo == TipoEvidencia.Video));
        });
    }

    // ── Actividad ────────────────────────────────────────────
    public async Task<BitacoraActividadDto> CreateActividadAsync(CreateActividadRequest request, int userId, string user)
    {
        var bitacora = await _repo.GetByIdAsync(request.BitacoraId)
            ?? throw new KeyNotFoundException($"Bitácora {request.BitacoraId} no encontrada");
        EnsureOwnership(bitacora.UserId, userId);

        EnsureValidRange(request.HoraInicio, request.HoraFin);
        await EnsureNoOverlapAsync(request.BitacoraId, request.HoraInicio, request.HoraFin, excludeId: null);

        var entity = new BitacoraActividad
        {
            BitacoraId = request.BitacoraId,
            HoraInicio = request.HoraInicio,
            HoraFin = request.HoraFin,
            Descripcion = request.Descripcion,
            UserCreated = user
        };

        var created = await _repo.CreateActividadAsync(entity);
        return ToDto(created);
    }

    public async Task<BitacoraActividadDto> UpdateActividadAsync(UpdateActividadRequest request, int userId, string user)
    {
        var actividad = await _repo.GetActividadByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Actividad {request.Id} no encontrada");
        EnsureOwnership(actividad.Bitacora.UserId, userId);

        EnsureValidRange(request.HoraInicio, request.HoraFin);
        await EnsureNoOverlapAsync(actividad.BitacoraId, request.HoraInicio, request.HoraFin, excludeId: request.Id);

        actividad.HoraInicio = request.HoraInicio;
        actividad.HoraFin = request.HoraFin;
        actividad.Descripcion = request.Descripcion;
        actividad.UserModified = user;
        actividad.DateModified = DateTime.UtcNow;

        var updated = await _repo.UpdateActividadAsync(actividad);
        return ToDto(updated);
    }

    public async Task DeleteActividadAsync(int id, int userId)
    {
        var actividad = await _repo.GetActividadByIdAsync(id)
            ?? throw new KeyNotFoundException($"Actividad {id} no encontrada");
        EnsureOwnership(actividad.Bitacora.UserId, userId);

        foreach (var evidencia in actividad.Evidencias.ToList())
        {
            await _blob.DeleteAsync(evidencia.BlobPath);
            await _repo.DeleteEvidenciaAsync(evidencia);
        }

        await _repo.DeleteActividadAsync(actividad);
    }

    private static void EnsureValidRange(TimeOnly inicio, TimeOnly fin)
    {
        if (fin <= inicio)
            throw new ArgumentException("La hora de fin debe ser posterior a la hora de inicio");
    }

    private async Task EnsureNoOverlapAsync(int bitacoraId, TimeOnly inicio, TimeOnly fin, int? excludeId)
    {
        var existentes = await _repo.GetActividadesActivasByBitacoraIdAsync(bitacoraId, excludeId);
        var solapa = existentes.Any(a => inicio < a.HoraFin && a.HoraInicio < fin);
        if (solapa)
            throw new ArgumentException("El horario se solapa con otra actividad ya registrada ese día");
    }

    // ── Evidencia — subida directa (imágenes, ≤ límite configurado) ──
    public async Task<BitacoraEvidenciaDto> UploadEvidenciaAsync(
        int actividadId, Stream fileStream, string fileName, string contentType, long fileSize,
        int userId, string user)
    {
        var actividad = await _repo.GetActividadByIdAsync(actividadId)
            ?? throw new KeyNotFoundException($"Actividad {actividadId} no encontrada");
        EnsureOwnership(actividad.Bitacora.UserId, userId);

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var tipo = EvidenciaSignatureValidator.TipoFromExtension(ext)
            ?? throw new ArgumentException($"Extensión no permitida: {ext}");

        if (tipo == TipoEvidencia.Video)
            throw new ArgumentException("Los videos deben subirse mediante el flujo de carga directa (sas-upload-url)");

        if (fileSize > _maxImageSizeBytes)
            throw new ArgumentException($"La imagen supera el tamaño máximo permitido ({_maxImageSizeBytes / 1024 / 1024} MB)");

        await EnsureEvidenceQuotaAsync(actividadId);

        using var buffer = new MemoryStream();
        await fileStream.CopyToAsync(buffer);
        var bytes = buffer.ToArray();

        var header = bytes.Length > 64 ? bytes[..64] : bytes;
        if (!EvidenciaSignatureValidator.MatchesSignature(ext, header))
            throw new ArgumentException("El archivo no corresponde al tipo declarado (firma binaria inválida)");

        var storedName = $"{Guid.NewGuid()}{ext}";
        var blobPath = $"bitacora/{userId}/{actividad.BitacoraId}/{actividadId}/{storedName}";
        var expectedContentType = EvidenciaSignatureValidator.AllowedContentTypes[ext];

        await using var uploadStream = new MemoryStream(bytes);
        await _blob.UploadAsync(uploadStream, blobPath, expectedContentType);

        BitacoraEvidencia evidencia;
        try
        {
            evidencia = await _repo.CreateEvidenciaAsync(new BitacoraEvidencia
            {
                BitacoraActividadId = actividadId,
                NombreOriginal = Path.GetFileName(fileName),
                NombreAlmacenado = storedName,
                BlobPath = blobPath,
                Tipo = tipo,
                ContentType = expectedContentType,
                Extension = ext,
                TamanoBytes = bytes.Length,
                UserCreated = user
            });
        }
        catch
        {
            await _blob.DeleteAsync(blobPath);
            throw;
        }

        return ToDto(evidencia);
    }

    // ── Evidencia — subida directa a Blob vía SAS (video / archivos grandes) ──
    public async Task<SasUploadResponse> GetSasUploadUrlAsync(int actividadId, SasUploadRequest request, int userId)
    {
        var actividad = await _repo.GetActividadByIdAsync(actividadId)
            ?? throw new KeyNotFoundException($"Actividad {actividadId} no encontrada");
        EnsureOwnership(actividad.Bitacora.UserId, userId);

        var ext = Path.GetExtension(request.FileName).ToLowerInvariant();
        var tipo = EvidenciaSignatureValidator.TipoFromExtension(ext)
            ?? throw new ArgumentException($"Extensión no permitida: {ext}");

        var maxSize = tipo == TipoEvidencia.Video ? _maxVideoSizeBytes : _maxImageSizeBytes;
        if (request.DeclaredSizeBytes > maxSize)
            throw new ArgumentException($"El archivo supera el tamaño máximo permitido ({maxSize / 1024 / 1024} MB)");

        await EnsureEvidenceQuotaAsync(actividadId);

        var storedName = $"{Guid.NewGuid()}{ext}";
        var blobPath = $"bitacora/{userId}/{actividad.BitacoraId}/{actividadId}/{storedName}";
        var expiry = TimeSpan.FromMinutes(30);

        var url = await _blob.GetSasUploadUrlAsync(blobPath, expiry);
        return new SasUploadResponse(url, blobPath, DateTime.UtcNow.Add(expiry));
    }

    public async Task<BitacoraEvidenciaDto> ConfirmEvidenciaAsync(ConfirmEvidenciaRequest request, int userId, string user)
    {
        var actividad = await _repo.GetActividadByIdAsync(request.BitacoraActividadId)
            ?? throw new KeyNotFoundException($"Actividad {request.BitacoraActividadId} no encontrada");
        EnsureOwnership(actividad.Bitacora.UserId, userId);

        // La ruta del blob debe pertenecer exactamente a esta actividad/usuario — evita confirmar un blob ajeno
        var expectedPrefix = $"bitacora/{userId}/{actividad.BitacoraId}/{request.BitacoraActividadId}/";
        if (!request.BlobPath.StartsWith(expectedPrefix, StringComparison.Ordinal))
            throw new UnauthorizedAccessException("La ruta de archivo no corresponde a esta actividad");

        var ext = Path.GetExtension(request.BlobPath).ToLowerInvariant();
        var tipo = EvidenciaSignatureValidator.TipoFromExtension(ext)
            ?? throw new ArgumentException($"Extensión no permitida: {ext}");

        var (sizeBytes, header) = await _blob.GetBlobHeaderAsync(request.BlobPath);
        if (sizeBytes <= 0)
            throw new ArgumentException("No se encontró el archivo subido en el almacenamiento");

        var maxSize = tipo == TipoEvidencia.Video ? _maxVideoSizeBytes : _maxImageSizeBytes;
        if (sizeBytes > maxSize)
        {
            await _blob.DeleteAsync(request.BlobPath);
            throw new ArgumentException($"El archivo supera el tamaño máximo permitido ({maxSize / 1024 / 1024} MB)");
        }

        if (!EvidenciaSignatureValidator.MatchesSignature(ext, header))
        {
            await _blob.DeleteAsync(request.BlobPath);
            throw new ArgumentException("El archivo subido no corresponde al tipo declarado (firma binaria inválida)");
        }

        await EnsureEvidenceQuotaAsync(request.BitacoraActividadId);

        var evidencia = await _repo.CreateEvidenciaAsync(new BitacoraEvidencia
        {
            BitacoraActividadId = request.BitacoraActividadId,
            NombreOriginal = Path.GetFileName(request.NombreOriginal),
            NombreAlmacenado = Path.GetFileName(request.BlobPath),
            BlobPath = request.BlobPath,
            Tipo = tipo,
            ContentType = EvidenciaSignatureValidator.AllowedContentTypes[ext],
            Extension = ext,
            TamanoBytes = sizeBytes,
            UserCreated = user
        });

        return ToDto(evidencia);
    }

    public async Task<string> GetEvidenciaUrlAsync(int evidenciaId, int userId)
    {
        var evidencia = await _repo.GetEvidenciaByIdAsync(evidenciaId)
            ?? throw new KeyNotFoundException($"Evidencia {evidenciaId} no encontrada");
        EnsureOwnership(evidencia.Actividad.Bitacora.UserId, userId);

        return await _blob.GetSasUrlAsync(evidencia.BlobPath, TimeSpan.FromHours(1));
    }

    public async Task DeleteEvidenciaAsync(int evidenciaId, int userId)
    {
        var evidencia = await _repo.GetEvidenciaByIdAsync(evidenciaId)
            ?? throw new KeyNotFoundException($"Evidencia {evidenciaId} no encontrada");
        EnsureOwnership(evidencia.Actividad.Bitacora.UserId, userId);

        await _repo.DeleteEvidenciaAsync(evidencia);
        await _blob.DeleteAsync(evidencia.BlobPath);
    }

    private async Task EnsureEvidenceQuotaAsync(int actividadId)
    {
        var count = await _repo.CountEvidenciasByActividadAsync(actividadId);
        if (count >= _maxEvidenciasPorActividad)
            throw new ArgumentException($"Se alcanzó el máximo de {_maxEvidenciasPorActividad} evidencias por actividad");
    }

    private static void EnsureOwnership(int resourceUserId, int userId)
    {
        if (resourceUserId != userId)
            throw new UnauthorizedAccessException("No tiene permisos sobre este recurso");
    }

    // ── Mapping ─────────────────────────────────────────────
    private static BitacoraDto ToDto(BDAplication.Domain.Entities.Bitacora.Bitacora b) => new(
        b.Id, b.UserId, b.Fecha, b.Observacion,
        b.UserCreated, b.DateCreated, b.UserModified, b.DateModified,
        b.Actividades.Where(a => a.IsActive).OrderBy(a => a.HoraInicio).Select(ToDto));

    private static BitacoraActividadDto ToDto(BitacoraActividad a) => new(
        a.Id, a.BitacoraId, a.HoraInicio, a.HoraFin, a.Descripcion,
        a.UserCreated, a.DateCreated, a.UserModified, a.DateModified,
        a.Evidencias.OrderBy(e => e.DateCreated).Select(ToDto));

    private static BitacoraEvidenciaDto ToDto(BitacoraEvidencia e) => new(
        e.Id, e.BitacoraActividadId, e.NombreOriginal, e.ContentType, e.Extension,
        e.TamanoBytes, e.Tipo.ToString(), e.UserCreated, e.DateCreated);
}
