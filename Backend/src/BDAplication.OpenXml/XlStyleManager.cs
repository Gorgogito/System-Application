using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BDAplication.OpenXml;

internal class XlStyleManager
{
    private readonly WorkbookPart _workbookPart;

    private readonly Dictionary<string, uint> _cellFormatCache = new();
    private readonly Dictionary<string, uint> _fontCache       = new();
    private readonly Dictionary<string, uint> _fillCache       = new();
    private readonly Dictionary<string, uint> _borderCache     = new();
    private readonly Dictionary<string, uint> _numFmtCache     = new();

    private uint _nextNumFmtId = 164u;

    internal XlStyleManager(WorkbookPart workbookPart)
    {
        _workbookPart = workbookPart;
        EnsureStylesheet();
    }

    public uint GetOrCreateStyleIndex(XlCellStyle style)
    {
        string key = style.ToKey();
        if (_cellFormatCache.TryGetValue(key, out uint cached)) return cached;

        uint fontId   = GetOrCreateFont(style.Font);
        uint fillId   = GetOrCreateFill(style.BackColor);
        uint borderId = GetOrCreateBorder(style.Borders);
        uint numFmtId = GetOrCreateNumberFormat(style);

        var ss = _workbookPart.WorkbookStylesPart!.Stylesheet;

        var xf = new CellFormat
        {
            NumberFormatId  = numFmtId,
            FontId          = fontId,
            FillId          = fillId,
            BorderId        = borderId,
            FormatId        = 0u,
            ApplyFont       = true,
            ApplyFill       = fillId > 0u,
            ApplyBorder     = borderId > 0u,
            ApplyNumberFormat = numFmtId > 0u,
            ApplyAlignment  = true
        };

        bool needsAlignment = false;
        var alignment = new Alignment();

        if (style.HorizontalAlignment != HorizontalAlignmentValues.General)
        { alignment.Horizontal = style.HorizontalAlignment; needsAlignment = true; }
        if (style.VerticalAlignment != VerticalAlignmentValues.Bottom)
        { alignment.Vertical = style.VerticalAlignment; needsAlignment = true; }
        if (style.WrapText)
        { alignment.WrapText = true; needsAlignment = true; }

        if (needsAlignment) xf.Append(alignment);

        uint newIdx = (uint)ss.CellFormats!.ChildElements.Count;
        ss.CellFormats.Append(xf);
        ss.CellFormats.Count = newIdx + 1u;

        _cellFormatCache[key] = newIdx;
        return newIdx;
    }

    private uint GetOrCreateFont(XlFont font)
    {
        string key = font.ToKey();
        if (_fontCache.TryGetValue(key, out uint cached)) return cached;

        var ss = _workbookPart.WorkbookStylesPart!.Stylesheet;
        var f  = new Font();

        if (font.Bold)      f.Append(new Bold());
        if (font.Italic)    f.Append(new Italic());
        if (font.Underline) f.Append(new Underline());
        f.Append(new FontSize { Val = font.Size });
        f.Append(new Color { Rgb = new HexBinaryValue(font.Color) });
        f.Append(new FontName { Val = font.Name });

        uint newIdx = (uint)ss.Fonts!.ChildElements.Count;
        ss.Fonts.Append(f);
        ss.Fonts.Count = newIdx + 1u;

        _fontCache[key] = newIdx;
        return newIdx;
    }

    private uint GetOrCreateFill(string backColor)
    {
        if (string.IsNullOrEmpty(backColor)) return 0u;
        if (_fillCache.TryGetValue(backColor, out uint cached)) return cached;

        var ss          = _workbookPart.WorkbookStylesPart!.Stylesheet;
        var patternFill = new PatternFill { PatternType = PatternValues.Solid };
        patternFill.Append(new ForegroundColor { Rgb = new HexBinaryValue(backColor) });
        patternFill.Append(new BackgroundColor { Indexed = 64u });

        uint newIdx = (uint)ss.Fills!.ChildElements.Count;
        ss.Fills.Append(new Fill(patternFill));
        ss.Fills.Count = newIdx + 1u;

        _fillCache[backColor] = newIdx;
        return newIdx;
    }

    private uint GetOrCreateBorder(XlBorders borders)
    {
        string key = borders.ToKey();
        if (_borderCache.TryGetValue(key, out uint cached)) return cached;

        if (!borders.Top.UseBorder && !borders.Bottom.UseBorder &&
            !borders.Left.UseBorder && !borders.Right.UseBorder)
        {
            _borderCache[key] = 0u;
            return 0u;
        }

        var ss = _workbookPart.WorkbookStylesPart!.Stylesheet;
        var b  = new Border();
        b.Append(BuildBorderSide<LeftBorder>(borders.Left));
        b.Append(BuildBorderSide<RightBorder>(borders.Right));
        b.Append(BuildBorderSide<TopBorder>(borders.Top));
        b.Append(BuildBorderSide<BottomBorder>(borders.Bottom));
        b.Append(new DiagonalBorder());

        uint newIdx = (uint)ss.Borders!.ChildElements.Count;
        ss.Borders.Append(b);
        ss.Borders.Count = newIdx + 1u;

        _borderCache[key] = newIdx;
        return newIdx;
    }

    private static T BuildBorderSide<T>(XlBorder xlBorder) where T : BorderPropertiesType, new()
    {
        var side = new T();
        if (xlBorder.UseBorder)
        {
            side.Style = xlBorder.Style;
            side.Append(new Color { Rgb = new HexBinaryValue(xlBorder.Color) });
        }
        return side;
    }

    private uint GetOrCreateNumberFormat(XlCellStyle style)
    {
        if (!string.IsNullOrEmpty(style.NumberFormatCode))
        {
            if (_numFmtCache.TryGetValue(style.NumberFormatCode, out uint cachedId))
                return cachedId;

            var ss = _workbookPart.WorkbookStylesPart!.Stylesheet;
            ss.NumberingFormats ??= new NumberingFormats { Count = 0u };

            uint newId = _nextNumFmtId++;
            ss.NumberingFormats.Append(new NumberingFormat
            {
                NumberFormatId = newId,
                FormatCode     = style.NumberFormatCode
            });
            ss.NumberingFormats.Count = (uint)ss.NumberingFormats.ChildElements.Count;

            _numFmtCache[style.NumberFormatCode] = newId;
            return newId;
        }

        return style.NumberFormatId;
    }

    private void EnsureStylesheet()
    {
        _workbookPart.AddNewPart<WorkbookStylesPart>();
        var ss = new Stylesheet();
        _workbookPart.WorkbookStylesPart!.Stylesheet = ss;

        ss.NumberingFormats = new NumberingFormats { Count = 0u };

        ss.Fonts = new Fonts { Count = 1u };
        ss.Fonts.Append(BuildDefaultFont());

        ss.Fills = new Fills { Count = 2u };
        ss.Fills.Append(new Fill(new PatternFill { PatternType = PatternValues.None }));
        ss.Fills.Append(new Fill(new PatternFill { PatternType = PatternValues.Gray125 }));

        ss.Borders = new Borders { Count = 1u };
        ss.Borders.Append(BuildDefaultBorder());

        ss.CellStyleFormats = new CellStyleFormats { Count = 1u };
        ss.CellStyleFormats.Append(new CellFormat
        { NumberFormatId = 0u, FontId = 0u, FillId = 0u, BorderId = 0u });

        ss.CellFormats = new CellFormats { Count = 1u };
        ss.CellFormats.Append(new CellFormat
        { NumberFormatId = 0u, FontId = 0u, FillId = 0u, BorderId = 0u, FormatId = 0u });

        ss.CellStyles = new CellStyles { Count = 1u };
        ss.CellStyles.Append(new CellStyle { Name = "Normal", FormatId = 0u, BuiltinId = 0u });

        _fontCache[new XlFont().ToKey()]     = 0u;
        _fillCache[string.Empty]             = 0u;
        _borderCache[new XlBorders().ToKey()] = 0u;
        _cellFormatCache[new XlCellStyle().ToKey()] = 0u;
    }

    private static Font BuildDefaultFont()
    {
        var f = new Font();
        f.Append(new FontSize { Val = 11.0 });
        f.Append(new Color { Rgb = new HexBinaryValue(XlColor.Black) });
        f.Append(new FontName { Val = "Calibri" });
        return f;
    }

    private static Border BuildDefaultBorder()
    {
        var b = new Border();
        b.Append(new LeftBorder());
        b.Append(new RightBorder());
        b.Append(new TopBorder());
        b.Append(new BottomBorder());
        b.Append(new DiagonalBorder());
        return b;
    }
}
