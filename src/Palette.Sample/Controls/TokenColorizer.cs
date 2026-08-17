using System.Text.RegularExpressions;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using Palette.Theming;

namespace Palette.Sample.Controls;

/// <summary>
/// A minimal C#-ish syntax colouriser for AvaloniaEdit that paints tokens with the template's
/// <em>live</em> theme brushes. Because those brush instances are mutated in place on a palette
/// swap, calling <c>TextView.Redraw()</c> is enough to recolour — no colour tables to rebuild.
/// <para>
/// This is intentionally small (line comments, strings, numbers, keywords, types). For a real
/// editor use AvaloniaEdit.TextMate; the point here is to show the tokens driving a genuine,
/// editable editor control.
/// </para>
/// </summary>
public sealed class TokenColorizer : DocumentColorizingTransformer
{
    private static readonly HashSet<string> Keywords = new(StringComparer.Ordinal)
    {
        "public", "private", "internal", "protected", "sealed", "static", "class", "struct",
        "record", "interface", "enum", "void", "var", "new", "return", "if", "else", "for",
        "foreach", "while", "in", "out", "ref", "using", "namespace", "this", "base", "null",
        "true", "false", "const", "readonly", "get", "set", "async", "await", "continue", "break",
    };

    private static readonly HashSet<string> Types = new(StringComparer.Ordinal)
    {
        "string", "int", "double", "float", "bool", "byte", "char", "object", "long", "short",
        "decimal", "Color", "SolidColorBrush", "ThemeManager", "PaletteDefinition", "Rgb",
    };

    // Comment (rest of line), string/char literal, or number.
    private static readonly Regex Token = new(
        @"(?<com>//.*$)|(?<str>""(?:\\.|[^""\\])*""|'(?:\\.|[^'\\])')|(?<num>\b\d[\d._]*\b)",
        RegexOptions.Compiled);

    private static readonly Regex Word = new(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);

    private static IBrush B(string token) => ThemeManager.Current.Brush(token)!;

    protected override void ColorizeLine(DocumentLine line)
    {
        var text = CurrentContext.Document.GetText(line);
        var start = line.Offset;

        // Track which character ranges are already claimed by comment/string/number so we don't
        // also word-highlight inside them.
        var claimed = new bool[text.Length];

        foreach (Match m in Token.Matches(text))
        {
            string brushKey;
            if (m.Groups["com"].Success) brushKey = ThemeTokens.SyntaxComment;
            else if (m.Groups["str"].Success) brushKey = ThemeTokens.SyntaxString;
            else brushKey = ThemeTokens.SyntaxNumber;

            Paint(start + m.Index, start + m.Index + m.Length, B(brushKey));
            for (var i = m.Index; i < m.Index + m.Length && i < claimed.Length; i++) claimed[i] = true;
        }

        foreach (Match w in Word.Matches(text))
        {
            if (claimed[w.Index]) continue;
            string? key = Keywords.Contains(w.Value) ? ThemeTokens.SyntaxKeyword
                : Types.Contains(w.Value) ? ThemeTokens.SyntaxType
                : null;
            if (key is not null) Paint(start + w.Index, start + w.Index + w.Length, B(key));
        }
    }

    private void Paint(int from, int to, IBrush brush) =>
        ChangeLinePart(from, to, el => el.TextRunProperties.SetForegroundBrush(brush));
}
