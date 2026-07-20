using System.Security.Claims;
using BDAplication.Application.DTOs;
using BDAplication.Application.DTOs.Attachments;
using BDAplication.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDAplication.API.Controllers;

[ApiController]
[Route("api/attachments")]
[Authorize]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _service;
    private readonly IConfiguration _config;

    public AttachmentController(IAttachmentService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    private string CurrentUser => User.FindFirstValue(ClaimTypes.Name) ?? "system";

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        var result = await _service.GetAllAsync(search);
        return Ok(ApiResponse<IEnumerable<AttachmentDto>>.Ok(result));
    }

    [HttpGet("{entityType}/{entityId:int}")]
    public async Task<IActionResult> GetByEntity(string entityType, int entityId)
    {
        var result = await _service.GetByEntityAsync(entityType, entityId);
        return Ok(ApiResponse<IEnumerable<AttachmentDto>>.Ok(result));
    }

    [HttpPost("upload")]
    [RequestSizeLimit(10_485_760)] // 10 MB
    public async Task<IActionResult> Upload(
        [FromForm] string entityType,
        [FromForm] int entityId,
        [FromForm] string description,
        [FromForm] string comment,
        [FromForm] string documentDate,
        [FromForm] int? documentConceptId,
        [FromForm] decimal? amount,
        IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("No se proporcionó ningún archivo"));

        long maxSize = _config.GetValue<long>("AzureBlobStorage:MaxFileSizeBytes", 10_485_760);
        if (file.Length > maxSize)
            return BadRequest(ApiResponse<string>.Fail($"El archivo supera el tamaño máximo permitido ({maxSize / 1024 / 1024} MB)"));

        if (string.IsNullOrEmpty(documentDate) || !DateTime.TryParse(documentDate, out var parsedDocDate))
            return BadRequest(ApiResponse<string>.Fail("La Fecha del Documento es obligatoria"));

        if (amount.HasValue && amount.Value < 0)
            return BadRequest(ApiResponse<string>.Fail("El importe no puede ser negativo"));

        await using var stream = file.OpenReadStream();
        var result = await _service.UploadAsync(
            entityType, entityId, stream,
            file.FileName, file.ContentType, file.Length,
            description, comment, parsedDocDate, documentConceptId, amount, CurrentUser);

        return Ok(ApiResponse<AttachmentDto>.Ok(result, "Archivo subido correctamente"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAttachmentRequest request)
    {
        if (request.DocumentDate == default)
            return BadRequest(ApiResponse<string>.Fail("La Fecha del Documento es obligatoria"));
        request.Id = id;
        var result = await _service.UpdateAsync(request, CurrentUser);
        return Ok(ApiResponse<AttachmentDto>.Ok(result, "Adjunto actualizado"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(ApiResponse<string>.Ok("Eliminado", "Adjunto eliminado correctamente"));
    }

    [HttpGet("{id:int}/url")]
    public async Task<IActionResult> GetUrl(int id)
    {
        var url = await _service.GetSasUrlAsync(id);
        return Ok(ApiResponse<string>.Ok(url));
    }
}
