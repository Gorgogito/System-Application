namespace BDAplication.OpenXml;

public class XlFont
{
    public string Name      { get; set; } = "Calibri";
    public double Size      { get; set; } = 11.0;
    public bool   Bold      { get; set; } = false;
    public bool   Italic    { get; set; } = false;
    public bool   Underline { get; set; } = false;
    public string Color     { get; set; } = XlColor.Black;

    public string ToKey() => $"{Name}|{Size}|{Bold}|{Italic}|{Underline}|{Color}";

    public XlFont Clone() => new()
    {
        Name      = Name,
        Size      = Size,
        Bold      = Bold,
        Italic    = Italic,
        Underline = Underline,
        Color     = Color
    };
}
