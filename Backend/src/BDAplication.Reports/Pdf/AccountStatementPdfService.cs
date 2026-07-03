using BDAplication.Application.DTOs.Finance;
using BDAplication.Application.Interfaces;
using BDAplication.Application.Interfaces.Finance;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BDAplication.Reports.Pdf;

public class AccountStatementPdfService : IPdfReportService
{
    private readonly IDateTimeService _dateTime;

    public AccountStatementPdfService(IDateTimeService dateTime)
    {
        _dateTime = dateTime;
    }

    // Colores corporativos (hex sin alpha, QuestPDF usa Color)
    private static readonly string NavyHex  = "#003366";
    private static readonly string BlueHex  = "#0070C0";
    private static readonly string LightBg  = "#DCE6F1";
    private static readonly string GrayBg   = "#F5F5F5";
    private static readonly string DebitBg  = "#FFE0E0";
    private static readonly string CreditBg = "#E0F4E8";
    private static readonly string PrevBal  = "#FFF3CD";
    private static readonly string FinalBal = "#D0E8FF";
    private static readonly string TotalBg  = "#E8EEF7";
    private static readonly string BorderC  = "#8EA9C1";

    public byte[] GenerateAccountStatement(AccountStatementDto statement,
                                           DateTime startDate, DateTime endDate)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var generatedAt = _dateTime.ToLimaTime(_dateTime.UtcNow);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.MarginHorizontal(1.5f, Unit.Centimetre);
                page.MarginVertical(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(t => t.FontFamily("Arial").FontSize(9));

                page.Header().Element(header => ComposeHeader(header, statement, startDate, endDate, generatedAt));
                page.Content().Element(content => ComposeContent(content, statement));
                page.Footer().Element(footer => ComposeFooter(footer, generatedAt));
            });
        }).GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, AccountStatementDto stmt,
                                      DateTime startDate, DateTime endDate, DateTime generatedAt)
    {
        container.Column(col =>
        {
            col.Item().Background(NavyHex).Padding(10).AlignCenter()
               .Text("ESTADO DE CUENTA")
               .FontColor(Colors.White).FontSize(18).Bold();

            col.Item().Background(BlueHex).PaddingVertical(4).AlignCenter()
               .Text("BDAplication")
               .FontColor(Colors.White).FontSize(11).Bold();

            col.Item().Background(LightBg).Padding(8).Table(t =>
            {
                t.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(1); c.RelativeColumn(3);
                    c.RelativeColumn(1); c.RelativeColumn(3);
                });

                MetaCell(t, "Cuenta:",    stmt.Account.Code);
                MetaCell(t, "Nombre:",    stmt.Account.Name);
                MetaCell(t, "Período:",   $"{startDate:dd/MM/yyyy}  →  {endDate:dd/MM/yyyy}");
                MetaCell(t, "Generado:",  generatedAt.ToString("dd/MM/yyyy HH:mm:ss"));
            });

            col.Item().Height(4);
        });
    }

    private static void MetaCell(TableDescriptor t, string label, string value)
    {
        t.Cell().PaddingRight(4)
         .Text(label).FontSize(9).Bold().FontColor(NavyHex);
        t.Cell().PaddingRight(12)
         .Text(value).FontSize(9).FontColor("#333333");
    }

    private static void ComposeContent(IContainer container, AccountStatementDto stmt)
    {
        container.Table(table =>
        {
            // Columnas: Fecha | Código | Tipo Doc | Concepto | Tipo Concepto | Debe | Haber | Saldo
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(65);   // Fecha
                c.ConstantColumn(75);   // Código
                c.ConstantColumn(100);  // Tipo Doc
                c.RelativeColumn(3);    // Concepto
                c.RelativeColumn(2);    // Tipo Concepto
                c.ConstantColumn(80);   // Debe
                c.ConstantColumn(80);   // Haber
                c.ConstantColumn(85);   // Saldo
            });

            // Encabezado tabla
            table.Header(h =>
            {
                string[] headers = { "Fecha", "Código", "Tipo Doc.", "Concepto", "Tipo Concepto", "Debe", "Haber", "Saldo" };
                bool[] rightAlign = { false, false, false, false, false, true, true, true };
                for (int i = 0; i < headers.Length; i++)
                {
                    var idx = i;
                    h.Cell().Background(NavyHex).Padding(5)
                     .Element(e => rightAlign[idx] ? e.AlignRight() : e.AlignCenter())
                     .Text(headers[i]).FontColor(Colors.White).FontSize(9).Bold();
                }
            });

            // Fila Saldo Anterior
            AppendSpecialRow(table, "Saldo Anterior", stmt.PreviousBalance, PrevBal, true);

            // Movimientos
            bool alt = false;
            foreach (var line in stmt.Lines)
            {
                string bg = line.Debit > 0 ? DebitBg : line.Credit > 0 ? CreditBg : (alt ? GrayBg : Colors.White);
                alt = !alt;

                DataCell(table, line.Date.ToString("dd/MM/yyyy HH:mm"), bg, center: true);
                DataCell(table, line.DocumentCode,                bg, center: true);
                DataCell(table, line.DocumentType,                bg);
                DataCell(table, line.Concept,                     bg);
                DataCell(table, line.TypeConcept,                 bg);
                MoneyCell(table, line.Debit,   bg);
                MoneyCell(table, line.Credit,  bg);
                MoneyCell(table, line.Balance, bg, bold: true);
            }

            // Fila Saldo Final
            AppendSpecialRow(table, "Saldo Final", stmt.FinalBalance, FinalBal, true);

            // Fila Totales
            AppendTotalsRow(table, stmt.TotalDebit, stmt.TotalCredit);
        });
    }

    private static void AppendSpecialRow(TableDescriptor table, string label,
                                         decimal balance, string bgColor, bool bold)
    {
        // 5 columnas fusionadas para etiqueta
        table.Cell().ColumnSpan(5).Background(bgColor).BorderColor(BorderC).BorderBottom(0.5f)
             .Padding(5).AlignRight()
             .Text(label).FontSize(10).Bold().FontColor(NavyHex);

        // Debe y Haber vacíos
        table.Cell().Background(bgColor).BorderColor(BorderC).BorderBottom(0.5f).Padding(5);
        table.Cell().Background(bgColor).BorderColor(BorderC).BorderBottom(0.5f).Padding(5);

        // Saldo
        table.Cell().Background(bgColor).BorderColor(BorderC).BorderBottom(0.5f)
             .Padding(5).AlignRight()
             .Text(FormatMoney(balance)).FontSize(10).Bold().FontColor(NavyHex);
    }

    private static void AppendTotalsRow(TableDescriptor table,
                                        decimal totalDebit, decimal totalCredit)
    {
        table.Cell().ColumnSpan(5).Background(TotalBg).BorderColor(BorderC).BorderBottom(0.5f)
             .Padding(5).AlignRight()
             .Text("TOTALES DEL PERÍODO").FontSize(10).Bold().FontColor(NavyHex);

        table.Cell().Background(TotalBg).BorderColor(BorderC).BorderBottom(0.5f)
             .Padding(5).AlignRight()
             .Text(FormatMoney(totalDebit)).FontSize(10).Bold().FontColor(NavyHex);

        table.Cell().Background(TotalBg).BorderColor(BorderC).BorderBottom(0.5f)
             .Padding(5).AlignRight()
             .Text(FormatMoney(totalCredit)).FontSize(10).Bold().FontColor(NavyHex);

        table.Cell().Background(TotalBg).BorderColor(BorderC).BorderBottom(0.5f).Padding(5);
    }

    private static void DataCell(TableDescriptor table, string text, string bg,
                                  bool center = false)
    {
        table.Cell().Background(bg).BorderColor(BorderC).BorderBottom(0.3f)
             .Padding(4)
             .Element(e => center ? e.AlignCenter() : e.AlignLeft())
             .Text(text).FontSize(9);
    }

    private static void MoneyCell(TableDescriptor table, decimal value,
                                   string bg, bool bold = false)
    {
        var cell = table.Cell().Background(bg).BorderColor(BorderC).BorderBottom(0.3f)
                        .Padding(4).AlignRight();

        if (value == 0)
            cell.Text(string.Empty).FontSize(9);
        else
        {
            var txt = cell.Text(FormatMoney(value)).FontSize(9);
            if (bold) txt.Bold();
        }
    }

    private static void ComposeFooter(IContainer container, DateTime generatedAt)
    {
        container.BorderTop(0.5f).BorderColor(BorderC).PaddingTop(4)
                 .Row(row =>
                 {
                     row.RelativeItem()
                        .Text($"Generado el {generatedAt:dd/MM/yyyy} a las {generatedAt:HH:mm:ss}  —  BDAplication")
                        .FontSize(8).Italic().FontColor(Colors.Grey.Medium);

                     row.ConstantItem(80).AlignRight()
                        .Text(x =>
                        {
                            x.Span("Página ").FontSize(8).FontColor(Colors.Grey.Medium);
                            x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                            x.Span(" de ").FontSize(8).FontColor(Colors.Grey.Medium);
                            x.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                 });
    }

    private static string FormatMoney(decimal value) =>
        value.ToString("N2");
}
