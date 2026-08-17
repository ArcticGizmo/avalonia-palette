using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Palette.Sample.Controls;

namespace Palette.Sample.Views;

/// <summary>The git-diff page: added / removed / hunk colours over the editor surface.</summary>
public sealed class DiffView : UserControl
{
    public DiffView()
    {
        var stack = new StackPanel { Spacing = 16, Margin = new Thickness(28, 24), MaxWidth = 980, HorizontalAlignment = HorizontalAlignment.Left };

        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(new TextBlock { Text = "UNIFIED DIFF", Classes = { "phead" } });
        panel.Children.Add(new TextBlock
        {
            Text = "Added and removed lines use low-saturation background washes (derived from the diff " +
                   "text colour) so a large diff scans cleanly without the eye fatigue of hard green/red fills.",
            Classes = { "help" },
        });
        panel.Children.Add(CodeRenderer.BuildDiffSurface());

        var card = new Border { Child = panel };
        card.Classes.Add("panel");
        stack.Children.Add(card);

        Content = new ScrollViewer { Content = stack };
    }
}
