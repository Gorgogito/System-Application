using System.Security.Claims;
using BDAplication.Application.DTOs;
using BDAplication.Application.DTOs.Finance;
using BDAplication.Application.Interfaces.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDAplication.API.Controllers.Finance;

[ApiController]
[Route("api/finance/reprocess")]
[Authorize(Roles = "Admin")]
public class ReprocesarSaldosController : ControllerBase
{
    private readonly IReprocesarSaldosService _service;
    public ReprocesarSaldosController(IReprocesarSaldosService service) => _service = service;

    private string CurrentUser => User.FindFirstValue(ClaimTypes.Name) ?? "system";

    [HttpPost("execute")]
    public async Task<IActionResult> Execute([FromBody] ReprocesarRequest request)
    {
        var result = await _service.ExecuteAsync(request, CurrentUser);
        var msg = request.IsSimulation ? "Simulación completada" : "Reproceso completado";
        return Ok(ApiResponse<ReprocesarResultDto>.Ok(result, msg));
    }

    [HttpGet("history")]
    public async Task<IActionResult> History([FromQuery] int limit = 20)
    {
        var result = await _service.GetHistoryAsync(limit);
        return Ok(ApiResponse<IEnumerable<ReprocesarLogDto>>.Ok(result));
    }
}
