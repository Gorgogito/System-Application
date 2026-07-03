using BDAplication.Application.DTOs.Finance;
using BDAplication.Application.Interfaces;
using BDAplication.Application.Interfaces.Finance;
using BDAplication.OpenXml;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BDAplication.Reports.Excel;

public class AccountStatementExcelService : IExcelReportService
{
    private readonly IDateTimeService _dateTime;

    public AccountStatementExcelService(IDateTimeService dateTime)
    {
        _dateTime = dateTime;
    }

    // Colores corporativos del sistema
    private const string ColorPrimary    = "FF003366"; // Navy — igual que XlColor.Navy
    private const string ColorPrimaryBg  = "FFDCE6F1"; // LightBlue — cabecera tabla
    private const string ColorAccent     = "FF0070C0"; // Blue
    private const string ColorWhite      = "FFFFFFFF";
    private const string ColorGray       = "FFF5F5F5"; // Fila alternada
    private const string ColorDebit      = "FFFFE0E0"; // Fondo rojo suave para débito
    private const string ColorCredit     = "FFE0F4E8"; // Fondo verde suave para crédito
    private const string ColorTotal      = "FFE8EEF7"; // Fondo fila totales
    private const string ColorPrevBal    = "FFFFF3CD"; // Fondo saldo anterior
    private const string ColorFinalBal   = "FFD0E8FF"; // Fondo saldo final
    private const string ColorBorder     = "FF8EA9C1"; // Borde tabla
    private const string ColorTextHeader = "FFFFFFFF"; // Texto de encabezado

    public byte[] GenerateAccountStatement(AccountStatementDto statement,
                                           DateTime startDate, DateTime endDate)
    {
        var generatedAt = _dateTime.ToLimaTime(_dateTime.UtcNow);

        using var stream = new MemoryStream();
        using (var doc = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
        {
            var wbPart = doc.AddWorkbookPart();
            wbPart.Workbook = new Workbook();

            var wsPart = wbPart.AddNewPart<WorksheetPart>();
            wsPart.Worksheet = new Worksheet(new SheetData());

            var sheets = wbPart.Workbook.AppendChild(new Sheets());
            sheets.Append(new Sheet
            {
                Id      = wbPart.GetIdOfPart(wsPart),
                SheetId = 1u,
                Name    = "Estado de Cuenta"
            });

            var ws = new OpenXmlWorksheet();
            ws.WorkbookPart  = wbPart;
            ws.WorksheetPart = wsPart;

            WriteReport(ws, statement, startDate, endDate, generatedAt);
            ws.Apply();
            wbPart.Workbook.Save();
        }

        return stream.ToArray();
    }

    private static void WriteReport(OpenXmlWorksheet ws, AccountStatementDto stmt,
                                    DateTime startDate, DateTime endDate, DateTime generatedAt)
    {
        const int COL_DATE    = 1;  // A
        const int COL_CODE    = 2;  // B
        const int COL_DOCTYPE = 3;  // C
        const int COL_CONCEPT = 4;  // D
        const int COL_TYCONCEPT = 5; // E
        const int COL_DEBIT   = 6;  // F
        const int COL_CREDIT  = 7;  // G
        const int COL_BALANCE = 8;  // H
        const int TOTAL_COLS  = 8;

        // Anchos de columna
        ws.SetColumnWidth("A", 14);  // Fecha
        ws.SetColumnWidth("B", 14);  // Código
        ws.SetColumnWidth("C", 20);  // Tipo Documento
        ws.SetColumnWidth("D", 36);  // Concepto
        ws.SetColumnWidth("E", 20);  // Tipo Concepto
        ws.SetColumnWidth("F", 16);  // Debe
        ws.SetColumnWidth("G", 16);  // Haber
        ws.SetColumnWidth("H", 16);  // Saldo

        int row = 1;

        // ── Bloque de encabezado ─────────────────────────────────────
        // Fila 1: Título principal
        ws.SetRowHeight(row, 28);
        var titleRange = ws.Ranges($"A{row}", $"H{row}");
        titleRange.Merge();
        titleRange.BackColor = ColorPrimary;
        titleRange.Font = new XlFont { Name = "Calibri", Size = 16, Bold = true, Color = ColorTextHeader };
        titleRange.HorizontalAlignment = HorizontalAlignmentValues.Center;
        titleRange.VerticalAlignment   = VerticalAlignmentValues.Center;
        ws.Range($"A{row}").Text = "ESTADO DE CUENTA";
        row++;

        // Fila 2: Nombre empresa
        ws.SetRowHeight(row, 20);
        var companyRange = ws.Ranges($"A{row}", $"H{row}");
        companyRange.Merge();
        companyRange.BackColor = ColorAccent;
        companyRange.Font = new XlFont { Name = "Calibri", Size = 11, Bold = true, Color = ColorTextHeader };
        companyRange.HorizontalAlignment = HorizontalAlignmentValues.Center;
        companyRange.VerticalAlignment   = VerticalAlignmentValues.Center;
        ws.Range($"A{row}").Text = "BDAplication";
        row++;

        // Fila 3: vacía separadora
        ws.SetRowHeight(row, 6);
        ws.Ranges($"A{row}", $"H{row}").BackColor = ColorPrimaryBg;
        row++;

        // Filas 4-7: Metadatos del reporte
        ws.SetRowHeight(row, 4, 18);

        WriteMetaRow(ws, row, "Cuenta:",       stmt.Account.Code, COL_DATE, TOTAL_COLS); row++;
        WriteMetaRow(ws, row, "Nombre:",        stmt.Account.Name, COL_DATE, TOTAL_COLS); row++;
        WriteMetaRow(ws, row, "Período:",       $"{startDate:dd/MM/yyyy}  →  {endDate:dd/MM/yyyy}", COL_DATE, TOTAL_COLS); row++;
        WriteMetaRow(ws, row, "Generado:",      generatedAt.ToString("dd/MM/yyyy HH:mm:ss"), COL_DATE, TOTAL_COLS); row++;

        // Fila 8: separadora
        ws.SetRowHeight(row, 6);
        ws.Ranges($"A{row}", $"H{row}").BackColor = ColorPrimaryBg;
        row++;

        // ── Encabezado de la tabla ───────────────────────────────────
        ws.SetRowHeight(row, 22);
        string[] headers = { "Fecha", "Código", "Tipo Doc.", "Concepto", "Tipo Concepto", "Debe", "Haber", "Saldo" };
        for (int col = 1; col <= TOTAL_COLS; col++)
        {
            var cell = ws.Range(CellRefHelper.BuildCellRef(row, col));
            cell.Text       = headers[col - 1];
            cell.BackColor  = ColorPrimary;
            cell.Font       = new XlFont { Name = "Calibri", Size = 10, Bold = true, Color = ColorTextHeader };
            cell.HorizontalAlignment = col >= COL_DEBIT
                ? HorizontalAlignmentValues.Right
                : HorizontalAlignmentValues.Center;
            cell.VerticalAlignment = VerticalAlignmentValues.Center;
            cell.Borders.SetAll(ColorBorder, BorderStyleValues.Thin);
        }
        row++;

        // ── Fila Saldo Anterior ──────────────────────────────────────
        ws.SetRowHeight(row, 18);
        WriteBalanceRow(ws, row, "Saldo Anterior", stmt.PreviousBalance, ColorPrevBal, TOTAL_COLS);
        row++;

        // ── Filas de movimientos ─────────────────────────────────────
        bool alternate = false;
        foreach (var line in stmt.Lines)
        {
            ws.SetRowHeight(row, 17);
            string rowColor = alternate ? ColorGray : ColorWhite;

            // Si hay débito, tinte rojo suave; si crédito, verde suave
            if (line.Debit  > 0) rowColor = ColorDebit;
            if (line.Credit > 0) rowColor = ColorCredit;

            void SetDataCell(int col, string text, bool isNumber = false,
                             HorizontalAlignmentValues? align = null)
            {
                var c = ws.Range(CellRefHelper.BuildCellRef(row, col));
                c.Text                = text;
                c.BackColor           = rowColor;
                c.Font                = new XlFont { Name = "Calibri", Size = 9, Color = XlColor.Black };
                c.HorizontalAlignment = align ?? HorizontalAlignmentValues.Left;
                if (isNumber) c.DataType = CellValues.Number;
                c.Borders.SetAll(ColorBorder, BorderStyleValues.Thin);
            }

            SetDataCell(COL_DATE,     line.Date.ToString("dd/MM/yyyy HH:mm"), align: HorizontalAlignmentValues.Center);
            SetDataCell(COL_CODE,     line.DocumentCode,                align: HorizontalAlignmentValues.Center);
            SetDataCell(COL_DOCTYPE,  line.DocumentType,                align: HorizontalAlignmentValues.Left);
            SetDataCell(COL_CONCEPT,  line.Concept);
            SetDataCell(COL_TYCONCEPT, line.TypeConcept);

            SetMoneyCell(ws, row, COL_DEBIT,   line.Debit,   rowColor, ColorBorder);
            SetMoneyCell(ws, row, COL_CREDIT,  line.Credit,  rowColor, ColorBorder);
            SetMoneyCell(ws, row, COL_BALANCE, line.Balance, rowColor, ColorBorder);

            alternate = !alternate;
            row++;
        }

        // ── Fila Saldo Final ─────────────────────────────────────────
        ws.SetRowHeight(row, 18);
        WriteBalanceRow(ws, row, "Saldo Final", stmt.FinalBalance, ColorFinalBal, TOTAL_COLS);
        row++;

        // ── Fila Totales ─────────────────────────────────────────────
        ws.SetRowHeight(row, 20);
        WriteTotalsRow(ws, row, stmt.TotalDebit, stmt.TotalCredit, TOTAL_COLS, COL_DEBIT, COL_CREDIT, COL_BALANCE);
        row++;

        // ── Pie del reporte ──────────────────────────────────────────
        row++;
        var footRange = ws.Ranges($"A{row}", $"H{row}");
        footRange.Merge();
        footRange.Font = new XlFont { Name = "Calibri", Size = 8, Italic = true, Color = XlColor.Gray };
        footRange.HorizontalAlignment = HorizontalAlignmentValues.Right;
        ws.Range($"A{row}").Text = $"Generado el {generatedAt:dd/MM/yyyy} a las {generatedAt:HH:mm:ss}  —  BDAplication";
    }

    private static void WriteMetaRow(OpenXmlWorksheet ws, int row, string label, string value,
                                     int colStart, int totalCols)
    {
        var labelCell = ws.Range(CellRefHelper.BuildCellRef(row, colStart));
        labelCell.Text     = label;
        labelCell.Font     = new XlFont { Name = "Calibri", Size = 10, Bold = true, Color = XlColor.Navy };
        labelCell.BackColor = ColorPrimaryBg;

        var valueRange = ws.Ranges(CellRefHelper.BuildCellRef(row, colStart + 1),
                                   CellRefHelper.BuildCellRef(row, totalCols));
        valueRange.Merge();
        ws.Range(CellRefHelper.BuildCellRef(row, colStart + 1)).Text = value;
        valueRange.Font    = new XlFont { Name = "Calibri", Size = 10, Color = XlColor.DarkGray };
        valueRange.BackColor = ColorPrimaryBg;
    }

    private static void WriteBalanceRow(OpenXmlWorksheet ws, int row, string label,
                                        decimal balance, string bgColor, int totalCols)
    {
        for (int c = 1; c <= totalCols; c++)
        {
            var cell    = ws.Range(CellRefHelper.BuildCellRef(row, c));
            cell.BackColor = bgColor;
            cell.Font   = new XlFont { Name = "Calibri", Size = 10, Bold = true, Color = XlColor.Navy };
            cell.Borders.SetAll(ColorBorder, BorderStyleValues.Medium);
        }

        // Merge columnas 1..5 para la etiqueta
        var labelRange = ws.Ranges($"A{row}", CellRefHelper.BuildCellRef(row, 5));
        labelRange.Merge();
        ws.Range($"A{row}").Text = label;
        ws.Range($"A{row}").HorizontalAlignment = HorizontalAlignmentValues.Right;

        // Saldo en columna H
        var balCell = ws.Range(CellRefHelper.BuildCellRef(row, 8));
        balCell.Text             = balance.ToString("F2");
        balCell.DataType         = CellValues.Number;
        balCell.NumberFormatCode = XlNumberFormat.Decimal2Miles;
        balCell.HorizontalAlignment = HorizontalAlignmentValues.Right;
    }

    private static void WriteTotalsRow(OpenXmlWorksheet ws, int row,
                                       decimal totalDebit, decimal totalCredit,
                                       int totalCols, int colDebit, int colCredit, int colBalance)
    {
        for (int c = 1; c <= totalCols; c++)
        {
            var cell    = ws.Range(CellRefHelper.BuildCellRef(row, c));
            cell.BackColor = ColorTotal;
            cell.Font   = new XlFont { Name = "Calibri", Size = 10, Bold = true, Color = XlColor.Navy };
            cell.Borders.SetAll(ColorBorder, BorderStyleValues.Medium);
        }

        var labelRange = ws.Ranges($"A{row}", CellRefHelper.BuildCellRef(row, colDebit - 1));
        labelRange.Merge();
        ws.Range($"A{row}").Text = "TOTALES DEL PERÍODO";
        ws.Range($"A{row}").HorizontalAlignment = HorizontalAlignmentValues.Right;

        SetMoneyCell(ws, row, colDebit,   totalDebit,   ColorTotal, ColorBorder, bold: true);
        SetMoneyCell(ws, row, colCredit,  totalCredit,  ColorTotal, ColorBorder, bold: true);

        // Columna de saldo totales vacía (el saldo final ya está en fila anterior)
        var emptySaldo = ws.Range(CellRefHelper.BuildCellRef(row, colBalance));
        emptySaldo.BackColor = ColorTotal;
        emptySaldo.Borders.SetAll(ColorBorder, BorderStyleValues.Medium);
    }

    private static void SetMoneyCell(OpenXmlWorksheet ws, int row, int col,
                                     decimal value, string bgColor, string borderColor,
                                     bool bold = false)
    {
        var c = ws.Range(CellRefHelper.BuildCellRef(row, col));
        c.BackColor           = bgColor;
        c.Font                = new XlFont { Name = "Calibri", Size = 9, Bold = bold, Color = XlColor.Black };
        c.Text                = value > 0 ? value.ToString("F2") : string.Empty;
        if (value > 0) c.DataType = CellValues.Number;
        c.NumberFormatCode    = XlNumberFormat.Decimal2Miles;
        c.HorizontalAlignment = HorizontalAlignmentValues.Right;
        c.Borders.SetAll(borderColor, BorderStyleValues.Thin);
    }
}
