using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using Palette.Sample.Controls;
using ArcticGizmo.Avalonia.Palette;

namespace Palette.Sample.Views;

/// <summary>
/// A genuinely editable code editor (AvaloniaEdit) wired to the palette tokens: background,
/// foreground, gutter, current-line highlight, selection and syntax colours all come from the
/// live theme, and recolour on swap.
/// </summary>
public sealed class EditorLiveView : UserControl
{
    private readonly TextEditor _editor;

    private const string Sample =
        "using ArcticGizmo.Avalonia.Palette;\n\n" +
        "// A real, editable AvaloniaEdit surface — type here and swap palettes.\n" +
        "public sealed class Greeter\n" +
        "{\n" +
        "    private const int HoursPerDay = 8;   // where you actually live\n\n" +
        "    public string Greet(string name)\n" +
        "    {\n" +
        "        var message = $\"Hello, {name}!\";\n" +
        "        return message;\n" +
        "    }\n" +
        "}\n";

    public EditorLiveView()
    {
        _editor = new TextEditor
        {
            Document = new TextDocument(Sample),
            ShowLineNumbers = true,
            FontFamily = new FontFamily("Cascadia Code,Consolas,Menlo,monospace"),
            FontSize = 13,
            Padding = new Thickness(10),
            WordWrap = false,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
        };
        _editor.Options.HighlightCurrentLine = true;
        _editor.TextArea.TextView.LineTransformers.Add(new TokenColorizer());

        ApplyThemeToEditor();
        ThemeManager.Current.PaletteChanged += OnPaletteChanged;
        DetachedFromVisualTree += (_, _) => ThemeManager.Current.PaletteChanged -= OnPaletteChanged;

        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(new TextBlock { Text = "LIVE EDITOR — AVALONIAEDIT", Classes = { "phead" } });
        panel.Children.Add(new TextBlock
        {
            Text = "A real editable editor. The gutter, current-line highlight, selection and syntax " +
                   "colours are the same tokens the rest of the app uses — edit the text, then swap " +
                   "palettes from the top bar and watch it recolour in place.",
            Classes = { "help" },
        });

        var editorHost = new Border
        {
            Height = 320,
            CornerRadius = new CornerRadius(8),
            BorderThickness = new Thickness(1),
            BorderBrush = ThemeManager.Current.Brush(ThemeTokens.Border),
            Background = ThemeManager.Current.Brush(ThemeTokens.EditorBackground),
            Child = _editor,
        };
        panel.Children.Add(editorHost);

        var card = new Border { Child = panel };
        card.Classes.Add("panel");

        Content = new ScrollViewer
        {
            Content = new StackPanel
            {
                Spacing = 16,
                Margin = new Thickness(28, 24),
                MaxWidth = 980,
                HorizontalAlignment = HorizontalAlignment.Left,
                Children = { card },
            },
        };
    }

    private void OnPaletteChanged(object? sender, PaletteDefinition e)
    {
        ApplyThemeToEditor();
        _editor.TextArea.TextView.Redraw(); // re-run the colouriser with updated brush colours
    }

    private void ApplyThemeToEditor()
    {
        _editor.Background = ThemeManager.Current.Brush(ThemeTokens.EditorBackground);
        _editor.Foreground = ThemeManager.Current.Brush(ThemeTokens.EditorForeground);
        _editor.LineNumbersForeground = ThemeManager.Current.Brush(ThemeTokens.EditorGutterForeground);
        _editor.TextArea.SelectionBrush = ThemeManager.Current.Brush(ThemeTokens.EditorSelection);
        _editor.TextArea.TextView.CurrentLineBackground = ThemeManager.Current.Brush(ThemeTokens.EditorCurrentLine);
        _editor.TextArea.TextView.CurrentLineBorder =
            new Pen(ThemeManager.Current.Brush(ThemeTokens.EditorCurrentLine), 0);
    }
}
