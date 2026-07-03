using System.Text;

namespace BDAplication.OpenXml;

public static class CellRefHelper
{
    public static int ColLetterToIndex(string colLetter)
    {
        int result = 0;
        foreach (char c in colLetter.ToUpperInvariant())
            result = result * 26 + (c - 'A' + 1);
        return result;
    }

    public static string IndexToColLetter(int colIndex)
    {
        string result = string.Empty;
        int idx = colIndex;
        while (idx > 0)
        {
            int remainder = (idx - 1) % 26;
            result = (char)('A' + remainder) + result;
            idx = (idx - 1) / 26;
        }
        return result;
    }

    public static void ParseCellRef(string cellRef, out int row, out int col)
    {
        var colStr = new StringBuilder();
        var rowStr = new StringBuilder();
        foreach (char c in cellRef.ToUpperInvariant())
        {
            if (char.IsLetter(c)) colStr.Append(c);
            else rowStr.Append(c);
        }
        col = ColLetterToIndex(colStr.ToString());
        row = int.Parse(rowStr.ToString());
    }

    public static string BuildCellRef(int row, int col) =>
        IndexToColLetter(col) + row.ToString();

    public static string ExtractColLetter(string cellRef)
    {
        var colStr = new StringBuilder();
        foreach (char c in cellRef.ToUpperInvariant())
        {
            if (char.IsLetter(c)) colStr.Append(c);
            else break;
        }
        return colStr.ToString();
    }
}
