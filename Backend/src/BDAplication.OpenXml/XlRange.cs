using DocumentFormat.OpenXml.Spreadsheet;

namespace BDAplication.OpenXml;

public class XlRange
{
    private string _text    = string.Empty;
    private string _formula = string.Empty;

    public int    Row     { get; }
    public int    Col     { get; }
    public string CellRef { get; }

    public CellValues DataType { get; set; } = CellValues.String;
    public XlCellStyle Style   { get; set; } = new();
    internal bool IsDirty      { get; set; } = false;

    public string Text
    {
        get => _text;
        set { _text = value ?? string.Empty; _formula = string.Empty; IsDirty = true; }
    }

    public string Formula
    {
        get => _formula;
        set { _formula = value ?? string.Empty; _text = string.Empty; IsDirty = true; }
    }

    // Style shortcuts
    public string BackColor
    {
        get => Style.BackColor;
        set { Style.BackColor = value; IsDirty = true; }
    }

    public XlFont Font
    {
        get => Style.Font;
        set { Style.Font = value; IsDirty = true; }
    }

    public XlBorders Borders
    {
        get => Style.Borders;
        set { Style.Borders = value; IsDirty = true; }
    }

    public HorizontalAlignmentValues HorizontalAlignment
    {
        get => Style.HorizontalAlignment;
        set { Style.HorizontalAlignment = value; IsDirty = true; }
    }

    public VerticalAlignmentValues VerticalAlignment
    {
        get => Style.VerticalAlignment;
        set { Style.VerticalAlignment = value; IsDirty = true; }
    }

    public bool WrapText
    {
        get => Style.WrapText;
        set { Style.WrapText = value; IsDirty = true; }
    }

    public uint NumberFormatId
    {
        get => Style.NumberFormatId;
        set { Style.NumberFormatId = value; IsDirty = true; }
    }

    public string NumberFormatCode
    {
        get => Style.NumberFormatCode;
        set { Style.NumberFormatCode = value; IsDirty = true; }
    }

    internal XlRange(int row, int col)
    {
        Row     = row;
        Col     = col;
        CellRef = CellRefHelper.BuildCellRef(row, col);
    }
}
