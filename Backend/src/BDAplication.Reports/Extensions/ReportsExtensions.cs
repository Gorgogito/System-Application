using BDAplication.Application.Interfaces.Finance;
using BDAplication.Reports.Excel;
using BDAplication.Reports.Pdf;
using Microsoft.Extensions.DependencyInjection;

namespace BDAplication.Reports.Extensions;

public static class ReportsExtensions
{
    public static IServiceCollection AddReports(this IServiceCollection services)
    {
        services.AddScoped<IExcelReportService, AccountStatementExcelService>();
        services.AddScoped<IPdfReportService,   AccountStatementPdfService>();
        return services;
    }
}
