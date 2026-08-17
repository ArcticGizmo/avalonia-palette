using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Palette.Sample.Controls;
using Palette.Theming;

namespace Palette.Sample.Views;

/// <summary>
/// The editing-surface page: an editable "text area" and a read-only syntax-highlighted
/// "full file" view — the two things a developer stares at most of the day. Built in code
/// so the file surface can pull the live theme brushes directly.
/// </summary>
public sealed class EditorView : UserControl
{
    public EditorView()
    {
        var stack = new StackPanel { Spacing = 16, Margin = new Thickness(28, 24), MaxWidth = 980, HorizontalAlignment = HorizontalAlignment.Left };

        stack.Children.Add(Card("EDITABLE TEXT AREA",
            "Real selection and caret colours come from the editor tokens. Select text or type — the caret uses EditorCaret, the selection uses EditorSelection.",
            TextArea()));

        stack.Children.Add(Card("FULL FILE — SYNTAX HIGHLIGHTED",
            "Line-number gutter, current-line highlight, and a full syntax palette. Swap themes from the top bar and watch every token recolour in place.",
            CodeRenderer.BuildFileSurface()));

        Content = new ScrollViewer { Content = stack };
    }

    private static TextBox TextArea()
    {
        var box = new TextBox
        {
            Height = 150,
            Text =
                "// Try selecting this text, or place the caret and type.\n" +
                "let greeting = \"eye-strain-friendly editing\";\n" +
                "const hoursPerDay = 8;   // this is where you live\n" +
                "function focus(surface) {\n" +
                "    return surface.background; // soft, never pure black/white\n" +
                "}",
        };
        box.Classes.Add("editor");
        return box;
    }

    private static Border Card(string head, string help, Control body)
    {
        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(new TextBlock { Text = head, Classes = { "phead" } });
        panel.Children.Add(new TextBlock { Text = help, Classes = { "help" } });
        panel.Children.Add(body);

        var card = new Border { Child = panel };
        card.Classes.Add("panel");
        return card;
    }
}
