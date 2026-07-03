using System.Security.Claims;
using BDAplication.Application.DTOs;
using BDAplication.Application.DTOs.Attachments;
using BDAplication.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDAplication.API.Controllers;

[ApiController]
[Route("api/documentconcepts")]
[Authorize]
public class DocumentConceptController : ControllerBase
{
    private readonly IDocumentConceptService _service;
    public DocumentConceptController(IDocumentConceptService service) => _service = service;

    private string CurrentUser => User.FindFirstValue(ClaimTypes.Name) ?? "system";

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllActiveAsync();
        return Ok(ApiResponse<IEnumerable<DocumentConceptDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentConceptRequest request)
    {
        var result = await _service.CreateAsync(request, CurrentUser);
        return Ok(ApiResponse<DocumentConceptDto>.Ok(result, "Concepto creado correctamente"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentConceptRequest request)
    {
        request.Id = id;
        var result = await _service.UpdateAsync(request);
        return Ok(ApiResponse<DocumentConceptDto>.Ok(result, "Concepto actualizado"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(ApiResponse<string>.Ok("Eliminado", "Concepto eliminado"));
    }
}
