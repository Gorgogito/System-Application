using System.Security.Claims;
using BDAplication.Application.DTOs;
using BDAplication.Application.DTOs.SecureDoc;
using BDAplication.Application.Interfaces.SecureDoc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDAplication.API.Controllers.SecureDoc;

[ApiController]
[Route("api/secure-docs")]
[Authorize]
public class SecureDocumentController : ControllerBase
{
    private readonly ISecureDocumentService _service;

    public SecureDocumentController(ISecureDocumentService service) => _service = service;

    private string CurrentUser => User.FindFirstValue(ClaimTypes.Name) ?? "system";

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync(CurrentUser);
        return Ok(ApiResponse<IEnumerable<SecureDocumentListItemDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null) return NotFound(ApiResponse<object>.Fail("Documento no encontrado"));
        return Ok(ApiResponse<SecureDocumentDto>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSecureDocumentRequest request)
    {
        var result = await _service.CreateAsync(request, CurrentUser);
        return Ok(ApiResponse<SecureDocumentDto>.Ok(result, "Documento creado y cifrado correctamente"));
    }

    [HttpPost("{id:int}/versions")]
    public async Task<IActionResult> AddVersion(int id, [FromBody] AddVersionRequest request)
    {
        var result = await _service.AddVersionAsync(id, request, CurrentUser);
        if (result is null) return NotFound(ApiResponse<object>.Fail("Documento no encontrado"));
        return Ok(ApiResponse<SecureDocumentDto>.Ok(result, "Nueva versión guardada y cifrada"));
    }

    [HttpPost("{id:int}/decrypt")]
    public async Task<IActionResult> Decrypt(int id, [FromBody] DecryptRequest request)
    {
        var result = await _service.DecryptAsync(id, request, CurrentUser);
        if (result is null)
            return Unauthorized(ApiResponse<object>.Fail("Credenciales incorrectas o documento no disponible"));
        return Ok(ApiResponse<DecryptedContentDto>.Ok(result, "Documento descifrado correctamente"));
    }

    [HttpPost("{id:int}/delete")]
    public async Task<IActionResult> Delete(int id, [FromBody] DecryptRequest request)
    {
        var result = await _service.SoftDeleteAsync(id, request, CurrentUser);
        return result switch
        {
            null  => Unauthorized(ApiResponse<object>.Fail("Credenciales incorrectas")),
            false => NotFound(ApiResponse<object>.Fail("Documento no encontrado")),
            _     => Ok(ApiResponse<bool>.Ok(true, "Documento eliminado"))
        };
    }
}
