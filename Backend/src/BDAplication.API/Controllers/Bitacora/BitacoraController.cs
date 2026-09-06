using System.Globalization;
using System.Security.Claims;
using BDAplication.Application.DTOs;
using BDAplication.Application.DTOs.Bitacora;
using BDAplication.Application.Interfaces.Bitacora;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDAplication.API.Controllers.Bitacora;

[ApiController]
[Route("api/bitacora")]
[Authorize]
public class BitacoraController : ControllerBase
{
    private readonly IBitacoraService _service;

    public BitacoraController(IBitacoraService service) => _service = service;

    private string CurrentUser => User.FindFirstValue(ClaimTypes.Name) ?? "system";

    private int CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new UnauthorizedAccessException("Token inválido");

    // ── Día ──────────────────────────────────────────────────

    /// <summary>Obtiene (o crea) el día de la bitácora del usuario autenticado.</summary>
    [HttpGet("{fecha}")]
    public async Task<IActionResult> GetByFecha(string fecha)
    {
        if (!DateTime.TryParse(fecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            return BadRequest(ApiResponse<string>.Fail("Fecha inválida"));

        var result = await _service.GetByFechaAsync(parsed, CurrentUserId, CurrentUser);
        return Ok(ApiResponse<BitacoraDto>.Ok(result));
    }

    /// <summary>Resumen por rango de fechas (pantalla de consulta), con filtro de texto opcional.</summary>
    [HttpGet]
    public async Task<IActionResult> GetResumen([FromQuery] DateTime desde, [FromQuery] DateTime hasta, [FromQuery] string? texto)
    {
        var result = await _service.GetResumenAsync(new GetResumenRequest(desde, hasta, texto), CurrentUserId);
        return Ok(ApiResponse<IEnumerable<BitacoraResumenDto>>.Ok(result));
    }

    /// <summary>Actualiza la observación general del día.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateObservacion(int id, [FromBody] UpdateBitacoraRequest request)
    {
        var result = await _service.UpdateObservacionAsync(id, request, CurrentUserId, CurrentUser);
        return Ok(ApiResponse<BitacoraDto>.Ok(result, "Bitácora actualizada"));
    }

    // ── Actividad ────────────────────────────────────────────

    [HttpPost("actividad")]
    public async Task<IActionResult> CreateActividad([FromBody] CreateActividadRequest request)
    {
        var result = await _service.CreateActividadAsync(request, CurrentUserId, CurrentUser);
        return Ok(ApiResponse<BitacoraActividadDto>.Ok(result, "Actividad creada"));
    }

    [HttpPut("actividad/{id:int}")]
    public async Task<IActionResult> UpdateActividad(int id, [FromBody] UpdateActividadRequest request)
    {
        if (id != request.Id)
            return BadRequest(ApiResponse<string>.Fail("El Id de la ruta no coincide con el de la solicitud"));

        var result = await _service.UpdateActividadAsync(request, CurrentUserId, CurrentUser);
        return Ok(ApiResponse<BitacoraActividadDto>.Ok(result, "Actividad actualizada"));
    }

    [HttpDelete("actividad/{id:int}")]
    public async Task<IActionResult> DeleteActividad(int id)
    {
        await _service.DeleteActividadAsync(id, CurrentUserId);
        return Ok(ApiResponse<string>.Ok("Eliminada", "Actividad eliminada correctamente"));
    }

    // ── Evidencia — imágenes (multipart directo) ──────────────

    [HttpPost("actividad/{id:int}/evidencia/upload")]
    [RequestSizeLimit(20_971_520)] // 20 MB (holgura sobre el límite de negocio de 15 MB para imágenes)
    public async Task<IActionResult> UploadEvidencia(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("No se proporcionó ningún archivo"));

        await using var stream = file.OpenReadStream();
        var result = await _service.UploadEvidenciaAsync(
            id, stream, file.FileName, file.ContentType, file.Length, CurrentUserId, CurrentUser);

        return Ok(ApiResponse<BitacoraEvidenciaDto>.Ok(result, "Evidencia subida correctamente"));
    }

    // ── Evidencia — video / archivos grandes (subida directa a Blob) ──

    [HttpPost("actividad/{id:int}/evidencia/sas-upload-url")]
    public async Task<IActionResult> GetSasUploadUrl(int id, [FromBody] SasUploadRequest request)
    {
        var result = await _service.GetSasUploadUrlAsync(id, request, CurrentUserId);
        return Ok(ApiResponse<SasUploadResponse>.Ok(result));
    }

    [HttpPost("actividad/{id:int}/evidencia/confirm")]
    public async Task<IActionResult> ConfirmEvidencia(int id, [FromBody] ConfirmEvidenciaRequest request)
    {
        if (id != request.BitacoraActividadId)
            return BadRequest(ApiResponse<string>.Fail("El Id de la ruta no coincide con el de la solicitud"));

        var result = await _service.ConfirmEvidenciaAsync(request, CurrentUserId, CurrentUser);
        return Ok(ApiResponse<BitacoraEvidenciaDto>.Ok(result, "Evidencia confirmada correctamente"));
    }

    [HttpGet("evidencia/{id:int}/url")]
    public async Task<IActionResult> GetEvidenciaUrl(int id)
    {
        var url = await _service.GetEvidenciaUrlAsync(id, CurrentUserId);
        return Ok(ApiResponse<string>.Ok(url));
    }

    [HttpDelete("evidencia/{id:int}")]
    public async Task<IActionResult> DeleteEvidencia(int id)
    {
        await _service.DeleteEvidenciaAsync(id, CurrentUserId);
        return Ok(ApiResponse<string>.Ok("Eliminada", "Evidencia eliminada correctamente"));
    }
}
