using System.Security.Claims;
using BDAplication.Application.DTOs.Finance;
using BDAplication.Application.Interfaces.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BDAplication.API.Controllers.Finance;

[ApiController]
[Route("api/finance/statement")]
[Authorize]
public class AccountStatementController : ControllerBase
{
    private readonly IAccountStatementService _statementService;
    private readonly IExcelReportService      _excelService;
    private readonly IPdfReportService        _pdfService;

    public AccountStatementController(
        IAccountStatementService statementService,
        IExcelReportService      excelService,
        IPdfReportService        pdfService)
    {
        _statementService = statementService;
        _excelService     = excelService;
        _pdfService       = pdfService;
    }

    [HttpGet("excel")]
    public async Task<IActionResult> GetExcel(
        [FromQuery] int    accountId,
        [FromQuery] string startDate,
        [FromQuery] string endDate)
    {
        if (!DateTime.TryParse(startDate, out var start) ||
            !DateTime.TryParse(endDate,   out var end))
            return BadRequest("Formato de fecha inválido. Use yyyy-MM-dd.");

        if (end < start)
            return BadRequest("La fecha final no puede ser anterior a la fecha inicial.");

        var stmt  = await _statementService.GetStatementAsync(
                        new AccountStatementRequest(accountId, start, end));
        var bytes = _excelService.GenerateAccountStatement(stmt, start, end);

        string fileName = $"EstadoCuenta_{stmt.Account.Code}_{start:yyyyMMdd}_{end:yyyyMMdd}.xlsx";
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> GetPdf(
        [FromQuery] int    accountId,
        [FromQuery] string startDate,
        [FromQuery] string endDate)
    {
        if (!DateTime.TryParse(startDate, out var start) ||
            !DateTime.TryParse(endDate,   out var end))
            return BadRequest("Formato de fecha inválido. Use yyyy-MM-dd.");

        if (end < start)
            return BadRequest("La fecha final no puede ser anterior a la fecha inicial.");

        var stmt  = await _statementService.GetStatementAsync(
                        new AccountStatementRequest(accountId, start, end));
        var bytes = _pdfService.GenerateAccountStatement(stmt, start, end);

        string fileName = $"EstadoCuenta_{stmt.Account.Code}_{start:yyyyMMdd}_{end:yyyyMMdd}.pdf";
        return File(bytes, "application/pdf", fileName);
    }
}
