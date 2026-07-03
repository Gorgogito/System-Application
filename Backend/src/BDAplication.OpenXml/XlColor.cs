namespace BDAplication.OpenXml;

public static class XlColor
{
    public const string Black       = "FF000000";
    public const string White       = "FFFFFFFF";
    public const string Red         = "FFFF0000";
    public const string Green       = "FF00B050";
    public const string Blue        = "FF0070C0";
    public const string Yellow      = "FFFFFF00";
    public const string Orange      = "FFFF6600";
    public const string Gray        = "FF808080";
    public const string LightGray   = "FFC0C0C0";
    public const string DarkGray    = "FF404040";
    public const string Navy        = "FF003366";
    public const string Teal        = "FF008080";
    public const string Purple      = "FF7030A0";
    public const string LightYellow = "FFFFFF99";
    public const string LightBlue   = "FFDCE6F1";
    public const string LightGreen  = "FFE2EFDA";

    public static string FromRGB(byte r, byte g, byte b) =>
        $"FF{r:X2}{g:X2}{b:X2}";

    public static string FromARGB(byte a, byte r, byte g, byte b) =>
        $"{a:X2}{r:X2}{g:X2}{b:X2}";
}
