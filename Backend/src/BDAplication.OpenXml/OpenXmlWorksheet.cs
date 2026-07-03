using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using A   = DocumentFormat.OpenXml.Drawing;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace BDAplication.OpenXml;

public class OpenXmlWorksheet
{
    private WorksheetPart? _worksheetPart;
    private WorkbookPart?  _workbookPart;
    private XlStyleManager? _styleManager;

    private readonly Dictionary<string, XlRange> _cells = new(StringComparer.OrdinalIgnoreCase);
    private int _imageCounter = 1;
    private readonly Dictionary<int, double> _columnWidths = new();
    private readonly Dictionary<int, double> _rowHeights   = new();

    public WorksheetPart? WorksheetPart
    {
        get => _worksheetPart;
        set => _worksheetPart = value;
    }

    public WorkbookPart? WorkbookPart
    {
        get => _workbookPart;
        set { _workbookPart = value; _styleManager = value is null ? null : new XlStyleManager(value); }
    }

    public XlRange Range(string cellRef)
    {
        if (!_cells.TryGetValue(cellRef, out var result))
        {
            CellRefHelper.ParseCellRef(cellRef, out int row, out int col);
            result = new XlRange(row, col);
            _cells[cellRef] = result;
        }
        return result;
    }

    public XlRanges Ranges(string iniRange, string finRange)
    {
        CellRefHelper.ParseCellRef(iniRange, out int iniRow, out int iniCol);
        CellRefHelper.ParseCellRef(finRange, out int finRow, out int finCol);

        var cellList = new List<XlRange>();
        for (int r = iniRow; r <= finRow; r++)
            for (int c = iniCol; c <= finCol; c++)
                cellList.Add(Range(CellRefHelper.BuildCellRef(r, c)));

        return new XlRanges(cellList, this, iniRange, finRange);
    }

    public void Initialize(string iniCell, string finCell)
    {
        CellRefHelper.ParseCellRef(iniCell, out int iniRow, out int iniCol);
        CellRefHelper.ParseCellRef(finCell, out int finRow, out int finCol);

        for (int r = iniRow; r <= finRow; r++)
            for (int c = iniCol; c <= finCol; c++)
            {
                string cellRef = CellRefHelper.BuildCellRef(r, c);
                if (!_cells.ContainsKey(cellRef))
                    _cells[cellRef] = new XlRange(r, c);
            }
    }

    public void Merge(string iniRange, string finRange)
    {
        var ws         = _worksheetPart!.Worksheet;
        var mergeCells = ws.Elements<MergeCells>().FirstOrDefault();

        if (mergeCells is null)
        {
            mergeCells = new MergeCells();
            var sheetData = ws.GetFirstChild<SheetData>()!;
            ws.InsertAfter(mergeCells, sheetData);
        }

        string reference = $"{iniRange.ToUpperInvariant()}:{finRange.ToUpperInvariant()}";
        bool alreadyMerged = mergeCells.Elements<MergeCell>()
            .Any(mc => string.Equals(mc.Reference?.Value, reference, StringComparison.OrdinalIgnoreCase));

        if (!alreadyMerged)
            mergeCells.Append(new MergeCell { Reference = new StringValue(reference) });
    }

    public void InsertImage(string imagePath, string iniCell, string finCell)
    {
        if (!File.Exists(imagePath))
            throw new FileNotFoundException($"Imagen no encontrada: {imagePath}");

        var imgType = GetImagePartType(imagePath);

        bool isNew = _worksheetPart!.DrawingsPart is null;
        DrawingsPart drawingsPart;
        if (isNew)
        {
            drawingsPart = _worksheetPart.AddNewPart<DrawingsPart>();
            drawingsPart.WorksheetDrawing = new Xdr.WorksheetDrawing();
        }
        else drawingsPart = _worksheetPart.DrawingsPart!;

        var imagePart = drawingsPart.AddImagePart(imgType);
        using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            imagePart.FeedData(stream);

        string imageRelId = drawingsPart.GetIdOfPart(imagePart);

        CellRefHelper.ParseCellRef(iniCell, out int fromRow, out int fromCol);
        CellRefHelper.ParseCellRef(finCell,  out int toRow,   out int toCol);

        var anchor = new Xdr.TwoCellAnchor { EditAs = Xdr.EditAsValues.OneCell };

        var fromMarker = new Xdr.FromMarker();
        fromMarker.Append(new Xdr.ColumnId((fromCol - 1).ToString()));
        fromMarker.Append(new Xdr.ColumnOffset("0"));
        fromMarker.Append(new Xdr.RowId((fromRow - 1).ToString()));
        fromMarker.Append(new Xdr.RowOffset("0"));

        var toMarker = new Xdr.ToMarker();
        toMarker.Append(new Xdr.ColumnId((toCol - 1).ToString()));
        toMarker.Append(new Xdr.ColumnOffset("0"));
        toMarker.Append(new Xdr.RowId((toRow - 1).ToString()));
        toMarker.Append(new Xdr.RowOffset("0"));

        var nvPicPr = new Xdr.NonVisualPictureProperties();
        nvPicPr.Append(new Xdr.NonVisualDrawingProperties { Id = (uint)(_imageCounter + 1), Name = $"Imagen{_imageCounter}" });
        nvPicPr.Append(new Xdr.NonVisualPictureDrawingProperties());

        var blipFill = new Xdr.BlipFill();
        blipFill.Append(new A.Blip { Embed = imageRelId });
        blipFill.Append(new A.Stretch(new A.FillRectangle()));

        var spPr = new Xdr.ShapeProperties();
        var xfrm = new A.Transform2D();
        xfrm.Append(new A.Offset { X = 0L, Y = 0L });
        xfrm.Append(new A.Extents { Cx = 0L, Cy = 0L });
        spPr.Append(xfrm);
        spPr.Append(new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle });

        var picture = new Xdr.Picture();
        picture.Append(nvPicPr);
        picture.Append(blipFill);
        picture.Append(spPr);

        anchor.Append(fromMarker);
        anchor.Append(toMarker);
        anchor.Append(picture);
        anchor.Append(new Xdr.ClientData());

        drawingsPart.WorksheetDrawing.Append(anchor);
        _imageCounter++;

        if (isNew)
            _worksheetPart.Worksheet.Append(
                new Drawing { Id = _worksheetPart.GetIdOfPart(drawingsPart) });
    }

    private static PartTypeInfo GetImagePartType(string imagePath) =>
        Path.GetExtension(imagePath).ToLowerInvariant() switch
        {
            ".png"            => ImagePartType.Png,
            ".jpg" or ".jpeg" => ImagePartType.Jpeg,
            ".gif"            => ImagePartType.Gif,
            ".bmp"            => ImagePartType.Bmp,
            ".tiff" or ".tif" => ImagePartType.Tiff,
            var ext           => throw new NotSupportedException($"Formato no soportado: {ext}")
        };

    public void SetColumnWidth(string colLetter, double width) =>
        _columnWidths[CellRefHelper.ColLetterToIndex(colLetter)] = width;

    public void SetColumnWidth(string iniCol, string finCol, double width)
    {
        int iniIdx = CellRefHelper.ColLetterToIndex(iniCol);
        int finIdx = CellRefHelper.ColLetterToIndex(finCol);
        for (int i = iniIdx; i <= finIdx; i++)
            _columnWidths[i] = width;
    }

    public void SetRowHeight(int rowIndex, double height) => _rowHeights[rowIndex] = height;

    public void SetRowHeight(int iniRow, int finRow, double height)
    {
        for (int i = iniRow; i <= finRow; i++)
            _rowHeights[i] = height;
    }

    public void Apply()
    {
        if (_worksheetPart is null) throw new InvalidOperationException("WorksheetPart no asignado.");
        if (_styleManager is null)  throw new InvalidOperationException("WorkbookPart no asignado.");

        var ws        = _worksheetPart.Worksheet;
        var sheetData = ws.GetFirstChild<SheetData>()!;

        var dirtyCells = _cells.Values
            .Where(c => c.IsDirty)
            .OrderBy(c => c.Row)
            .ThenBy(c => c.Col)
            .ToList();

        foreach (var xlRange in dirtyCells)
        {
            var xmlRow = GetOrCreateRow(sheetData, (uint)xlRange.Row);
            var cell   = GetOrCreateCell(xmlRow, xlRange.CellRef);

            cell.StyleIndex = _styleManager.GetOrCreateStyleIndex(xlRange.Style);

            cell.RemoveAllChildren<CellFormula>();
            cell.RemoveAllChildren<CellValue>();
            cell.RemoveAllChildren<InlineString>();
            cell.DataType = null;

            if (!string.IsNullOrEmpty(xlRange.Formula))
            {
                cell.Append(new CellFormula(xlRange.Formula));
                cell.Append(new CellValue());
            }
            else if (!string.IsNullOrEmpty(xlRange.Text))
            {
                if (xlRange.DataType == CellValues.Number)
                {
                    cell.Append(new CellValue(xlRange.Text));
                }
                else if (xlRange.DataType == CellValues.Boolean)
                {
                    cell.DataType = CellValues.Boolean;
                    cell.Append(new CellValue(xlRange.Text));
                }
                else
                {
                    cell.DataType = CellValues.InlineString;
                    cell.Append(new InlineString(new Text(xlRange.Text)));
                }
            }

            xlRange.IsDirty = false;
        }

        if (_columnWidths.Count > 0) ApplyColumnWidths(ws, sheetData);
        foreach (var kvp in _rowHeights)
        {
            var hr = GetOrCreateRow(sheetData, (uint)kvp.Key);
            hr.Height       = kvp.Value;
            hr.CustomHeight = true;
        }

        _worksheetPart.Worksheet.Save();
        _workbookPart!.WorkbookStylesPart!.Stylesheet.Save();

        if (_worksheetPart.DrawingsPart is not null)
            _worksheetPart.DrawingsPart.WorksheetDrawing.Save();
    }

    private static Row GetOrCreateRow(SheetData sheetData, uint rowIndex)
    {
        var existing = sheetData.Elements<Row>()
            .FirstOrDefault(r => r.RowIndex?.Value == rowIndex);

        if (existing is not null) return existing;

        var newRow  = new Row { RowIndex = rowIndex };
        var refRow  = sheetData.Elements<Row>()
            .FirstOrDefault(r => r.RowIndex?.Value > rowIndex);

        if (refRow is not null) sheetData.InsertBefore(newRow, refRow);
        else                    sheetData.Append(newRow);

        return newRow;
    }

    private void ApplyColumnWidths(Worksheet ws, SheetData sheetData)
    {
        var cols = ws.GetFirstChild<Columns>();
        if (cols is null)
        {
            cols = new Columns();
            ws.InsertBefore(cols, sheetData);
        }

        var sorted = _columnWidths.OrderBy(k => k.Key).ToList();
        int i = 0;
        while (i < sorted.Count)
        {
            int    startIdx = sorted[i].Key;
            int    endIdx   = startIdx;
            double colWidth = sorted[i].Value;

            while (i + 1 < sorted.Count &&
                   sorted[i + 1].Key == endIdx + 1 &&
                   sorted[i + 1].Value == colWidth)
            { i++; endIdx = sorted[i].Key; }

            uint minU = (uint)startIdx;
            uint maxU = (uint)endIdx;

            var existing = cols.Elements<Column>()
                .FirstOrDefault(c => c.Min?.Value == minU && c.Max?.Value == maxU);

            if (existing is not null) { existing.Width = colWidth; existing.CustomWidth = true; }
            else cols.Append(new Column { Min = minU, Max = maxU, Width = colWidth, CustomWidth = true });

            i++;
        }
    }

    private static Cell GetOrCreateCell(Row xmlRow, string cellReference)
    {
        var existing = xmlRow.Elements<Cell>()
            .FirstOrDefault(c => string.Equals(c.CellReference?.Value, cellReference,
                                               StringComparison.OrdinalIgnoreCase));
        if (existing is not null) return existing;

        var newCell = new Cell { CellReference = cellReference };

        CellRefHelper.ParseCellRef(cellReference, out _, out int newCol);
        var refCell = xmlRow.Elements<Cell>().FirstOrDefault(c =>
        {
            if (c.CellReference is null) return false;
            CellRefHelper.ParseCellRef(c.CellReference.Value!, out _, out int cCol);
            return cCol > newCol;
        });

        if (refCell is not null) xmlRow.InsertBefore(newCell, refCell);
        else                     xmlRow.Append(newCell);

        return newCell;
    }
}
