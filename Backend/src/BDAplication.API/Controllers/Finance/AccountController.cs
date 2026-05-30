using System.Security.Claims;
using BDAplication.Application.DTOs;
using BDAplication.Application.DTOs.Finance;
using BDAplication.Application.Interfaces.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDAplication.API.Controllers.Finance;

[ApiController]
[Route("api/finance/account")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IAccountService _service;
    public AccountController(IAccountService service) => _service = service;

    private string CurrentUser => User.FindFirstValue(ClaimTypes.Name) ?? "system";

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<AccountDto>>.Ok(result));
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] string code)
    {
        var result = await _service.GetByCodeAsync(code);
        return Ok(ApiResponse<AccountDto>.Ok(result));
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
    {
        var result = await _service.CreateAsync(request, CurrentUser);
        return Ok(ApiResponse<AccountDto>.Ok(result, "Cuenta creada correctamente"));
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateAccountRequest request)
    {
        var result = await _service.UpdateAsync(request);
        return Ok(ApiResponse<AccountDto>.Ok(result, "Cuenta actualizada"));
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        await _service.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(null!, "Cuenta eliminada"));
    }
}
