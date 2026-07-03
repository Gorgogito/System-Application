using DocumentFormat.OpenXml.Spreadsheet;

namespace BDAplication.OpenXml;

public class XlRanges
{
    private readonly List<XlRange>       _cells;
    private readonly OpenXmlWorksheet    _worksheet;
    private readonly string              _iniRange;
    private readonly string              _finRange;

    internal XlRanges(List<XlRange> cells, OpenXmlWorksheet worksheet,
                      string iniRange, string finRange)
    {
        _cells     = cells;
        _worksheet = worksheet;
        _iniRange  = iniRange;
        _finRange  = finRange;
    }

    public IReadOnlyList<XlRange> Cells => _cells.AsReadOnly();

    public string BackColor
    {
        get => _cells.Count > 0 ? _cells[0].BackColor : string.Empty;
        set { foreach (var c in _cells) c.BackColor = value; }
    }

    public XlFont Font
    {
        get => _cells.Count > 0 ? _cells[0].Font : new XlFont();
        set { foreach (var c in _cells) c.Font = value.Clone(); }
    }

    public XlBorders Borders
    {
        get => _cells.Count > 0 ? _cells[0].Borders : new XlBorders();
        set { foreach (var c in _cells) c.Borders = value.Clone(); }
    }

    public HorizontalAlignmentValues HorizontalAlignment
    {
        get => _cells.Count > 0 ? _cells[0].HorizontalAlignment : HorizontalAlignmentValues.General;
        set { foreach (var c in _cells) c.HorizontalAlignment = value; }
    }

    public VerticalAlignmentValues VerticalAlignment
    {
        get => _cells.Count > 0 ? _cells[0].VerticalAlignment : VerticalAlignmentValues.Bottom;
        set { foreach (var c in _cells) c.VerticalAlignment = value; }
    }

    public bool WrapText
    {
        get => _cells.Count > 0 && _cells[0].WrapText;
        set { foreach (var c in _cells) c.WrapText = value; }
    }

    public string NumberFormatCode
    {
        get => _cells.Count > 0 ? _cells[0].NumberFormatCode : string.Empty;
        set { foreach (var c in _cells) c.NumberFormatCode = value; }
    }

    public void Merge() => _worksheet.Merge(_iniRange, _finRange);

    public void SetOuterBorder(string color, BorderStyleValues style)
    {
        CellRefHelper.ParseCellRef(_iniRange, out int iniRow, out int iniCol);
        CellRefHelper.ParseCellRef(_finRange, out int finRow, out int finCol);

        var borde = new XlBorder { UseBorder = true, Color = color, Style = style };
        foreach (var cell in _cells)
        {
            var b       = cell.Style.Borders;
            bool changed = false;
            if (cell.Row == iniRow) { b.Top    = borde.Clone(); changed = true; }
            if (cell.Row == finRow) { b.Bottom = borde.Clone(); changed = true; }
            if (cell.Col == iniCol) { b.Left   = borde.Clone(); changed = true; }
            if (cell.Col == finCol) { b.Right  = borde.Clone(); changed = true; }
            if (changed) cell.IsDirty = true;
        }
    }

    public void SetOuterBorder(XlBorder top, XlBorder bottom, XlBorder left, XlBorder right)
    {
        CellRefHelper.ParseCellRef(_iniRange, out int iniRow, out int iniCol);
        CellRefHelper.ParseCellRef(_finRange, out int finRow, out int finCol);

        foreach (var cell in _cells)
        {
            var b       = cell.Style.Borders;
            bool changed = false;
            if (cell.Row == iniRow) { b.Top    = top.Clone();    changed = true; }
            if (cell.Row == finRow) { b.Bottom = bottom.Clone(); changed = true; }
            if (cell.Col == iniCol) { b.Left   = left.Clone();   changed = true; }
            if (cell.Col == finCol) { b.Right  = right.Clone();  changed = true; }
            if (changed) cell.IsDirty = true;
        }
    }
}
