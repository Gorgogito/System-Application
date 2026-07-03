namespace BDAplication.OpenXml;

public static class XlNumberFormat
{
    public const string Entero        = "0";
    public const string Decimal1      = "0.0";
    public const string Decimal2      = "0.00";
    public const string Decimal3      = "0.000";
    public const string EnteroMiles   = "#,##0";
    public const string Decimal1Miles = "#,##0.0";
    public const string Decimal2Miles = "#,##0.00";
    public const string Decimal3Miles = "#,##0.000";
    public const string Porcentaje    = "0%";
    public const string Porcentaje2   = "0.00%";
    public const string Cientifico    = "0.00E+00";

    public static string Build(int decimales, bool miles = false)
    {
        if (decimales < 0)
            throw new ArgumentOutOfRangeException(nameof(decimales), "No puede ser negativo.");

        string base_ = miles ? "#,##0" : "0";
        if (decimales > 0)
            base_ += "." + new string('0', decimales);
        return base_;
    }
}
