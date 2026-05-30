using System.Security.Claims;
using BDAplication.Application.DTOs;
using BDAplication.Application.DTOs.TaskPlanner;
using BDAplication.Application.Interfaces.TaskPlanner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDAplication.API.Controllers;

[ApiController]
[Route("api/process")]
[Authorize]
public class ProcessController : ControllerBase
{
    private readonly IBacklogService _backlog;
    private readonly IPlannerService _planner;

    public ProcessController(IBacklogService backlog, IPlannerService planner)
    {
        _backlog = backlog;
        _planner = planner;
    }

    private string CurrentUser =>
        User.FindFirstValue(ClaimTypes.Name) ?? "system";

    // ── Backlog ────────────────────────────────────────────────────

    /// <summary>Registrar tarea en backlog</summary>
    [HttpPost("backlog/BacklogRegister")]
    public async Task<IActionResult> BacklogRegister([FromBody] BacklogRegisterRequest request)
    {
        var result = await _backlog.RegisterAsync(request, CurrentUser);
        return Ok(ApiResponse<BacklogDto>.Ok(result, "Backlog task registered successfully"));
    }

    /// <summary>Listar tareas activas del backlog</summary>
    [HttpGet("backlog/BacklogList")]
    public async Task<IActionResult> BacklogList(
        [FromQuery] string? search,
        [FromQuery] Domain.Enums.Priority? priority,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _backlog.ListAsync(new BacklogListRequest(search, priority, page, pageSize));
        return Ok(ApiResponse<BacklogListResponse>.Ok(result));
    }

    // ── Planner ────────────────────────────────────────────────────

    /// <summary>Registrar tarea planificada en un día</summary>
    [HttpPost("planner/PlannerRegister")]
    public async Task<IActionResult> PlannerRegister([FromBody] PlannerRegisterRequest request)
    {
        var result = await _planner.RegisterAsync(request, CurrentUser);
        return Ok(ApiResponse<PlannerDto>.Ok(result, "Planner task registered successfully"));
    }

    /// <summary>Inactivar tarea planificada (soft delete)</summary>
    [HttpPut("planner/PlannerInactive")]
    public async Task<IActionResult> PlannerInactive([FromBody] PlannerInactiveRequest request)
    {
        var result = await _planner.InactivateAsync(request);
        return Ok(ApiResponse<PlannerDto>.Ok(result, "Planner task inactivated"));
    }

    /// <summary>Listar tareas planificadas de un mes agrupadas por día</summary>
    [HttpPatch("planner/PlannerList")]
    public async Task<IActionResult> PlannerList([FromBody] PlannerListRequest request)
    {
        var result = await _planner.ListByMonthAsync(request);
        return Ok(ApiResponse<IEnumerable<PlannerListByDayDto>>.Ok(result));
    }

    /// <summary>Mover tarea planificada a otro día</summary>
    [HttpPatch("planner/MovePlanner")]
    public async Task<IActionResult> MovePlanner([FromBody] MovePlannerRequest request)
    {
        var result = await _planner.MoveAsync(request);
        return Ok(ApiResponse<PlannerDto>.Ok(result, "Planner task moved successfully"));
    }
}
