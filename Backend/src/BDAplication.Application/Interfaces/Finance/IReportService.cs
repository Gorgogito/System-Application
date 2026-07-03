using BDAplication.Application.DTOs.Finance;

namespace BDAplication.Application.Interfaces.Finance;

public interface IExcelReportService
{
    byte[] GenerateAccountStatement(AccountStatementDto statement, DateTime startDate, DateTime endDate);
}

public interface IPdfReportService
{
    byte[] GenerateAccountStatement(AccountStatementDto statement, DateTime startDate, DateTime endDate);
}
