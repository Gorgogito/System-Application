using DocumentFormat.OpenXml.Spreadsheet;

namespace BDAplication.OpenXml;

public class XlBorder
{
    public bool             UseBorder { get; set; } = false;
    public string           Color     { get; set; } = XlColor.Black;
    public BorderStyleValues Style    { get; set; } = BorderStyleValues.Thin;

    public string ToKey() => $"{UseBorder}|{Color}|{Style}";

    public XlBorder Clone() => new()
    {
        UseBorder = UseBorder,
        Color     = Color,
        Style     = Style
    };
}
