using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using Palette.Theming.Color;

namespace Palette.Theming;

/// <summary>
/// Owns the live set of themed brushes and swaps palettes at runtime.
/// <para>
/// The engine keeps one <see cref="SolidColorBrush"/> instance per <see cref="ThemeTokens"/>
/// key and registers it in <c>Application.Current.Resources</c>. Applying a palette mutates
/// each brush's <see cref="SolidColorBrush.Color"/> <em>in place</em> rather than replacing
/// the instance. Because Avalonia controls observe brush property changes, every consumer
/// recolours instantly — whether it resolved the brush via <c>{StaticResource}</c>,
/// <c>{DynamicResource}</c>, or a direct code reference. No visual-tree walk required.
/// </para>
/// </summary>
public sealed class ThemeManager
{
    private static ThemeManager? _current;

    /// <summary>The process-wide instance. Call <see cref="Initialize"/> once at startup.</summary>
    public static ThemeManager Current =>
        _current ?? throw new InvalidOperationException(
            "ThemeManager.Initialize(app) must be called before ThemeManager.Current is used.");

    private readonly Dictionary<string, SolidColorBrush> _brushes = new(StringComparer.Ordinal);
    private Application _app = null!;

    private ThemeManager() { }

    /// <summary>The palette currently applied.</summary>
    public PaletteDefinition CurrentPalette { get; private set; } = null!;

    /// <summary>Raised after a new palette has been applied.</summary>
    public event EventHandler<PaletteDefinition>? PaletteChanged;

    /// <summary>
    /// Create the brush set, register it in the application's resources, and apply the
    /// starting palette. Idempotent: subsequent calls just re-apply.
    /// </summary>
    public static ThemeManager Initialize(Application app, PaletteDefinition initial)
    {
        _current ??= new ThemeManager();
        _current._app = app;

        // Seed every token with a brush and publish it into app resources so that both
        // {StaticResource Key} and {DynamicResource Key} resolve the same live instance.
        var resolved = initial.Resolve();
        foreach (var key in ThemeTokens.All)
        {
            var color = resolved.TryGetValue(key, out var rgb) ? rgb.ToColor() : Colors.Magenta;
            if (_current._brushes.TryGetValue(key, out var existing))
                existing.Color = color;
            else
            {
                var brush = new SolidColorBrush(color);
                _current._brushes[key] = brush;
                app.Resources[key] = brush;
            }
        }

        app.RequestedThemeVariant = initial.IsDark ? ThemeVariant.Dark : ThemeVariant.Light;
        _current.CurrentPalette = initial;
        _current.PaletteChanged?.Invoke(_current, initial);
        return _current;
    }

    /// <summary>Swap to a different palette. All bound surfaces recolour immediately.</summary>
    public void Apply(PaletteDefinition palette)
    {
        var resolved = palette.Resolve();
        foreach (var key in ThemeTokens.All)
        {
            if (!_brushes.TryGetValue(key, out var brush)) continue;
            brush.Color = resolved.TryGetValue(key, out var rgb) ? rgb.ToColor() : Colors.Magenta;
        }

        // Keep Fluent's built-in control chrome (popups, scrollbars, caret) aligned with
        // the palette's light/dark polarity; our tokens layer branded surfaces on top.
        _app.RequestedThemeVariant = palette.IsDark ? ThemeVariant.Dark : ThemeVariant.Light;

        CurrentPalette = palette;
        PaletteChanged?.Invoke(this, palette);
    }

    /// <summary>Swap by palette id (see <see cref="PaletteCatalog"/>).</summary>
    public void Apply(string paletteId) => Apply(PaletteCatalog.ById(paletteId));

    /// <summary>The live brush for a token, or null if the id is unknown.</summary>
    public SolidColorBrush? Brush(string tokenKey) =>
        _brushes.TryGetValue(tokenKey, out var b) ? b : null;

    /// <summary>The current colour for a token.</summary>
    public global::Avalonia.Media.Color Color(string tokenKey) =>
        Brush(tokenKey)?.Color ?? Colors.Magenta;

    /// <summary>Build a WCAG contrast report for the current palette.</summary>
    public ContrastReport Report() => ContrastReport.For(CurrentPalette);
}
