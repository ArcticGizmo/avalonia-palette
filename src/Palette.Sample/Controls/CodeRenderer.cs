using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;
using Palette.Theming;

namespace Palette.Sample.Controls;

/// <summary>
/// Builds the syntax-highlighted "full file" surface and the git-diff surface out of plain
/// controls. Every colour is pulled from <see cref="ThemeManager"/>'s live brush instances,
/// so both surfaces recolour in place when the palette swaps — no rebuild needed.
/// <para>
/// The sample is pre-tokenised (rather than lexed at runtime) so the highlighting is stable
/// and the focus stays on the palette, not on a toy parser.
/// </para>
/// </summary>
public static class CodeRenderer
{
    private static readonly FontFamily Mono = new("Cascadia Code,Consolas,Menlo,monospace");
    private const double FontSize = 13;
    private const double LineHeight = 20;

    private static IBrush B(string token) => ThemeManager.Current.Brush(token)!;

    // token kinds → theme token keys
    private static string Key(string kind) => kind switch
    {
        "kw" => ThemeTokens.SyntaxKeyword,
        "str" => ThemeTokens.SyntaxString,
        "num" => ThemeTokens.SyntaxNumber,
        "com" => ThemeTokens.SyntaxComment,
        "fn" => ThemeTokens.SyntaxFunction,
        "ty" => ThemeTokens.SyntaxType,
        "op" => ThemeTokens.SyntaxOperator,
        "cn" => ThemeTokens.SyntaxConstant,
        "at" => ThemeTokens.SyntaxAttribute,
        _ => ThemeTokens.EditorForeground,
    };

    // ── Sample source, pre-tokenised as lines of (kind, text) spans ──────
    private static readonly (string kind, string text)[][] Sample =
    {
        new[] { ("com", "// ThemeManager swaps palettes by mutating brushes in place.") },
        new[] { ("kw", "public "), ("kw", "sealed "), ("kw", "class "), ("ty", "ThemeManager") },
        new[] { ("", "{") },
        new[] { ("", "    "), ("kw", "public "), ("kw", "void "), ("fn", "Apply"), ("", "("), ("ty", "PaletteDefinition"), ("", " palette)") },
        new[] { ("", "    {") },
        new[] { ("", "        "), ("kw", "var"), ("", " resolved = palette."), ("fn", "Resolve"), ("", "();") },
        new[] { ("", "        "), ("kw", "foreach"), ("", " ("), ("kw", "var"), ("", " key "), ("kw", "in"), ("", " "), ("ty", "ThemeTokens"), ("", "."), ("cn", "All"), ("", ")") },
        new[] { ("", "        {") },
        new[] { ("", "            "), ("kw", "if"), ("", " (!_brushes."), ("fn", "TryGetValue"), ("", "(key, "), ("kw", "out"), ("", " "), ("kw", "var"), ("", " brush)) "), ("kw", "continue"), ("", ";") },
        new[] { ("", "            brush."), ("at", "Color"), ("", " = resolved[key]."), ("fn", "ToColor"), ("", "();  "), ("com", "// in place → live") },
        new[] { ("", "        }") },
        new[] { ("", "") },
        new[] { ("", "        "), ("ty", "CurrentPalette"), ("op", " = "), ("", "palette;") },
        new[] { ("", "        PaletteChanged?."), ("fn", "Invoke"), ("", "("), ("kw", "this"), ("", ", palette);") },
        new[] { ("", "    }") },
        new[] { ("", "") },
        new[] { ("", "    "), ("kw", "const "), ("ty", "double"), ("", " AaText = "), ("num", "4.5"), ("", ";  "), ("com", "// WCAG 1.4.3") },
        new[] { ("", "}") },
    };

    /// <summary>The syntax-highlighted, line-numbered read-only file surface.</summary>
    public static Control BuildFileSurface(int currentLine = 9)
    {
        var lines = new StackPanel { Orientation = Orientation.Vertical };

        for (var i = 0; i < Sample.Length; i++)
        {
            var isCurrent = i == currentLine;

            var gutter = new TextBlock
            {
                Text = (i + 1).ToString().PadLeft(2),
                FontFamily = Mono,
                FontSize = FontSize,
                Foreground = B(ThemeTokens.EditorGutterForeground),
                Width = 40,
                Height = LineHeight,
                TextAlignment = TextAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 14, 0),
            };

            var text = new TextBlock
            {
                FontFamily = Mono,
                FontSize = FontSize,
                Height = LineHeight,
                VerticalAlignment = VerticalAlignment.Center,
            };
            foreach (var (kind, span) in Sample[i])
                text.Inlines!.Add(new Run(span) { Foreground = B(Key(kind)) });

            var row = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*"), Height = LineHeight };
            Grid.SetColumn(gutter, 0);
            Grid.SetColumn(text, 1);
            row.Children.Add(gutter);
            row.Children.Add(text);

            var rowHost = new Border
            {
                Child = row,
                Padding = new Thickness(12, 0),
                Background = isCurrent ? B(ThemeTokens.EditorCurrentLine) : Brushes.Transparent,
            };
            lines.Children.Add(rowHost);
        }

        var body = new Border
        {
            Background = B(ThemeTokens.EditorBackground),
            CornerRadius = new CornerRadius(8),
            BorderBrush = B(ThemeTokens.Border),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(0, 12),
            Child = new ScrollViewer
            {
                HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                Content = lines,
            },
        };

        return body;
    }

    // ── Diff sample: unified hunk ────────────────────────────────────────
    private enum DiffKind { Hunk, Context, Add, Remove }

    private static readonly (DiffKind kind, string text)[] Diff =
    {
        (DiffKind.Hunk, "@@ -14,7 +14,9 @@ public void Apply(PaletteDefinition palette)"),
        (DiffKind.Context, "         var resolved = palette.Resolve();"),
        (DiffKind.Context, "         foreach (var key in ThemeTokens.All)"),
        (DiffKind.Remove, "             _brushes[key] = new SolidColorBrush(resolved[key]);"),
        (DiffKind.Add, "             if (!_brushes.TryGetValue(key, out var brush)) continue;"),
        (DiffKind.Add, "             brush.Color = resolved[key].ToColor();"),
        (DiffKind.Context, ""),
        (DiffKind.Remove, "         CurrentPalette = palette;"),
        (DiffKind.Add, "         CurrentPalette = palette;"),
        (DiffKind.Add, "         _app.RequestedThemeVariant = palette.IsDark ? Dark : Light;"),
        (DiffKind.Context, "         PaletteChanged?.Invoke(this, palette);"),
    };

    /// <summary>The unified git-diff surface.</summary>
    public static Control BuildDiffSurface()
    {
        var lines = new StackPanel();

        foreach (var (kind, content) in Diff)
        {
            var (bg, fg, gutterFg, sign) = kind switch
            {
                DiffKind.Add => (B(ThemeTokens.DiffAddedBg), B(ThemeTokens.EditorForeground), B(ThemeTokens.DiffAddedText), "+"),
                DiffKind.Remove => (B(ThemeTokens.DiffRemovedBg), B(ThemeTokens.EditorForeground), B(ThemeTokens.DiffRemovedText), "-"),
                DiffKind.Hunk => (B(ThemeTokens.DiffHunkBg), B(ThemeTokens.DiffHunkText), B(ThemeTokens.DiffHunkText), " "),
                _ => (B(ThemeTokens.EditorBackground), B(ThemeTokens.EditorForeground), B(ThemeTokens.DiffLineNumber), " "),
            };

            var marker = new TextBlock
            {
                Text = sign, FontFamily = Mono, FontSize = FontSize, Foreground = gutterFg,
                Width = 22, TextAlignment = TextAlignment.Center, Height = LineHeight,
                VerticalAlignment = VerticalAlignment.Center,
            };
            var text = new TextBlock
            {
                Text = content, FontFamily = Mono, FontSize = FontSize, Foreground = fg,
                Height = LineHeight, VerticalAlignment = VerticalAlignment.Center,
            };

            var row = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*"), Height = LineHeight };
            Grid.SetColumn(marker, 0);
            Grid.SetColumn(text, 1);
            row.Children.Add(marker);
            row.Children.Add(text);

            lines.Children.Add(new Border { Child = row, Background = bg, Padding = new Thickness(8, 0) });
        }

        return new Border
        {
            Background = B(ThemeTokens.EditorBackground),
            CornerRadius = new CornerRadius(8),
            BorderBrush = B(ThemeTokens.Border),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(0, 10),
            Child = new ScrollViewer
            {
                HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                Content = lines,
            },
        };
    }
}
