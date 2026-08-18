using Avalonia.Media;
using ArcticGizmo.Avalonia.Palette;
using ArcticGizmo.Avalonia.Palette.Color;

namespace Palette.Sample.ViewModels;

/// <summary>
/// A single palette shown in the switcher and gallery. The swatch brushes are built from
/// the palette's own colours (not the active theme) so each row previews itself.
/// </summary>
public sealed class PaletteChoiceViewModel(PaletteDefinition palette)
{
    public PaletteDefinition Palette { get; } = palette;

    public string Id => Palette.Id;
    public string Name => Palette.Name;
    public string VariantLabel => Palette.IsDark ? "Dark" : "Light";
    public string Display => $"{Palette.Name} · {VariantLabel}";
    public string Description => Palette.Description;
    public bool IsDark => Palette.IsDark;

    public IBrush SurfaceSwatch => new SolidColorBrush(Palette.Surface.ToColor());
    public IBrush PanelSwatch => new SolidColorBrush(Palette.SurfaceRaised.ToColor());
    public IBrush AccentSwatch => new SolidColorBrush(Palette.Accent.ToColor());
    public IBrush TextSwatch => new SolidColorBrush(Palette.TextPrimary.ToColor());
    public IBrush EditorSwatch => new SolidColorBrush(Palette.EditorBg.ToColor());

    /// <summary>Whether every readability-critical pairing meets WCAG AA.</summary>
    public bool PassesAa => ContrastReport.For(Palette).AllTextPassesAa;

    public string WcagLabel => PassesAa ? "AA ✓" : "check";
}
