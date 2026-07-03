using DocumentFormat.OpenXml.Spreadsheet;

namespace BDAplication.OpenXml;

public class XlCellStyle
{
    public string                    BackColor           { get; set; } = string.Empty;
    public XlFont                    Font                { get; set; } = new();
    public XlBorders                 Borders             { get; set; } = new();
    public HorizontalAlignmentValues HorizontalAlignment { get; set; } = HorizontalAlignmentValues.General;
    public VerticalAlignmentValues   VerticalAlignment   { get; set; } = VerticalAlignmentValues.Bottom;
    public bool                      WrapText            { get; set; } = false;
    public uint                      NumberFormatId      { get; set; } = 0;
    public string                    NumberFormatCode    { get; set; } = string.Empty;

    public string ToKey() =>
        $"{BackColor}|{Font.ToKey()}|{Borders.ToKey()}|{HorizontalAlignment}|{VerticalAlignment}|{WrapText}|{NumberFormatId}|{NumberFormatCode}";

    public XlCellStyle Clone() => new()
    {
        BackColor           = BackColor,
        Font                = Font.Clone(),
        Borders             = Borders.Clone(),
        HorizontalAlignment = HorizontalAlignment,
        VerticalAlignment   = VerticalAlignment,
        WrapText            = WrapText,
        NumberFormatId      = NumberFormatId,
        NumberFormatCode    = NumberFormatCode
    };
}
