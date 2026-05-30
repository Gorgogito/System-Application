using System.Security.Claims;
using BDAplication.Application.DTOs;
using BDAplication.Application.DTOs.Finance;
using BDAplication.Application.Interfaces.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDAplication.API.Controllers.Finance;

[ApiController]
[Route("api/finance/movement")]
[Authorize]
public class MovementController : ControllerBase
{
    private readonly IMovementService _service;
    public MovementController(IMovementService service) => _service = service;

    private string CurrentUser => User.FindFirstValue(ClaimTypes.Name) ?? "system";

    [HttpGet("listbyaccount")]
    public async Task<IActionResult> ListByAccount([FromQuery] int accountId)
    {
        var result = await _service.GetByAccountAsync(accountId);
        return Ok(ApiResponse<IEnumerable<MovementDto>>.Ok(result));
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] string code)
    {
        var result = await _service.GetByCodeAsync(code);
        return Ok(ApiResponse<MovementDto>.Ok(result));
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateMovementRequest request)
    {
        var result = await _service.CreateAsync(request, CurrentUser);
        return Ok(ApiResponse<MovementDto>.Ok(result, "Movimiento registrado"));
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateMovementRequest request)
    {
        var result = await _service.UpdateAsync(request, CurrentUser);
        return Ok(ApiResponse<MovementDto>.Ok(result, "Movimiento actualizado"));
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        await _service.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(null!, "Movimiento eliminado"));
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] CreateTransferRequest request)
    {
        var (source, destiny) = await _service.CreateTransferAsync(request, CurrentUser);
        var result = new { Source = source, Destiny = destiny };
        return Ok(ApiResponse<object>.Ok(result, "Transferencia registrada correctamente"));
    }
}
