using DocumentFormat.OpenXml.Spreadsheet;

namespace BDAplication.OpenXml;

public class XlBorders
{
    public XlBorder Top    { get; set; } = new();
    public XlBorder Bottom { get; set; } = new();
    public XlBorder Left   { get; set; } = new();
    public XlBorder Right  { get; set; } = new();

    public string ToKey() => $"{Top.ToKey()}|{Bottom.ToKey()}|{Left.ToKey()}|{Right.ToKey()}";

    public XlBorders Clone() => new()
    {
        Top    = Top.Clone(),
        Bottom = Bottom.Clone(),
        Left   = Left.Clone(),
        Right  = Right.Clone()
    };

    public void SetAll(string color, BorderStyleValues style)
    {
        Top    = new XlBorder { UseBorder = true, Color = color, Style = style };
        Bottom = new XlBorder { UseBorder = true, Color = color, Style = style };
        Left   = new XlBorder { UseBorder = true, Color = color, Style = style };
        Right  = new XlBorder { UseBorder = true, Color = color, Style = style };
    }

    public void SetOuter(string color, BorderStyleValues style) => SetAll(color, style);
}
