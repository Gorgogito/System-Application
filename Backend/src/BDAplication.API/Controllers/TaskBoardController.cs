using System.Security.Claims;
using BDAplication.Application.DTOs;
using BDAplication.Application.DTOs.TaskBoard;
using BDAplication.Application.Interfaces.TaskBoard;
using BDAplication.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDAplication.API.Controllers;

[ApiController]
[Route("api/process/taskboard")]
[Authorize]
public class TaskBoardController : ControllerBase
{
    private readonly ITaskBoardService _service;

    public TaskBoardController(ITaskBoardService service) => _service = service;

    private string CurrentUser => User.FindFirstValue(ClaimTypes.Name) ?? "system";

    /// <summary>Crear una tarea en el tablero</summary>
    [HttpPost("CreateTask")]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskBoardRequest request)
    {
        var result = await _service.CreateAsync(request, CurrentUser);
        return Ok(ApiResponse<TaskBoardDto>.Ok(result, "Task created successfully"));
    }

    /// <summary>Obtener todas las tareas activas</summary>
    [HttpGet("GetTasks")]
    public async Task<IActionResult> GetTasks(
        [FromQuery] TaskBoardStatus? status,
        [FromQuery] Priority? priority,
        [FromQuery] string? search)
    {
        var result = await _service.GetAllAsync(new GetTaskBoardRequest(status, priority, search));
        return Ok(ApiResponse<IEnumerable<TaskBoardDto>>.Ok(result));
    }

    /// <summary>Obtener tarea por Id</summary>
    [HttpGet("GetTaskById")]
    public async Task<IActionResult> GetTaskById([FromQuery] int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(ApiResponse<TaskBoardDto>.Ok(result));
    }

    /// <summary>Actualizar una tarea</summary>
    [HttpPut("UpdateTask")]
    public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskBoardRequest request)
    {
        var result = await _service.UpdateAsync(request);
        return Ok(ApiResponse<TaskBoardDto>.Ok(result, "Task updated successfully"));
    }

    /// <summary>Mover tarea a otro estado (columna)</summary>
    [HttpPatch("MoveTask")]
    public async Task<IActionResult> MoveTask([FromBody] MoveTaskBoardRequest request)
    {
        var result = await _service.MoveAsync(request);
        return Ok(ApiResponse<TaskBoardDto>.Ok(result, "Task moved successfully"));
    }

    /// <summary>Soft delete de una tarea</summary>
    [HttpDelete("DeleteTask")]
    public async Task<IActionResult> DeleteTask([FromQuery] int id)
    {
        var result = await _service.DeleteAsync(id);
        return Ok(ApiResponse<TaskBoardDto>.Ok(result, "Task deleted successfully"));
    }

    // ── Subtareas ────────────────────────────────────────────

    /// <summary>Agregar subtarea a una tarea</summary>
    [HttpPost("subtask/Create")]
    public async Task<IActionResult> CreateSubTask([FromBody] CreateSubTaskRequest request)
    {
        var result = await _service.CreateSubTaskAsync(request, CurrentUser);
        return Ok(ApiResponse<SubTaskDto>.Ok(result, "Sub-task created"));
    }

    /// <summary>Marcar/desmarcar subtarea como completada</summary>
    [HttpPatch("subtask/Toggle")]
    public async Task<IActionResult> ToggleSubTask([FromBody] ToggleSubTaskRequest request)
    {
        var result = await _service.ToggleSubTaskAsync(request.SubTaskId);
        return Ok(ApiResponse<SubTaskDto>.Ok(result, "Sub-task toggled"));
    }

    /// <summary>Eliminar subtarea</summary>
    [HttpDelete("subtask/Delete")]
    public async Task<IActionResult> DeleteSubTask([FromQuery] int id)
    {
        await _service.DeleteSubTaskAsync(id);
        return Ok(ApiResponse<object>.Ok(null, "Sub-task deleted"));
    }
}
