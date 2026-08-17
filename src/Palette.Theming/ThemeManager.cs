using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Palette.Theming.Color;

namespace Palette.Theming;

/// <summary>
/// Owns the live set of themed brushes and swaps palettes at runtime.
/// <para>
/// The engine keeps one <see cref="SolidColorBrush"/> instance per <see cref="ThemeTokens"/>
/// key and registers it in <c>Application.Current.Resources</c>. Applying a palette mutates
/// each brush's <see cref="SolidColorBrush.Color"/> <em>in place</em> rather than replacing
/// the instance, so every consumer recolours instantly — whether it resolved the brush via
/// <c>{StaticResource}</c>, <c>{DynamicResource}</c>, or a direct code reference.
/// </para>
/// <para>
/// Also supports an optional colour-vision-deficiency filter (see <see cref="SetCvd"/>) and
/// following the OS light/dark setting (see <see cref="FollowOsTheme"/>).
/// </para>
/// </summary>
public sealed class ThemeManager
{
    private static ThemeManager? _current;

    public static ThemeManager Current =>
        _current ?? throw new InvalidOperationException(
            "ThemeManager.Initialize(app) must be called before ThemeManager.Current is used.");

    private readonly Dictionary<string, SolidColorBrush> _brushes = new(StringComparer.Ordinal);
    private Application _app = null!;
    private Cvd _cvd = Cvd.None;
    private bool _followingOs;
    private bool _osHooked;

    private ThemeManager() { }

    /// <summary>The palette currently applied.</summary>
    public PaletteDefinition CurrentPalette { get; private set; } = null!;

    /// <summary>The active colour-vision-deficiency simulation (None = off).</summary>
    public Cvd CurrentCvd => _cvd;

    /// <summary>True while the manager is tracking the OS light/dark setting.</summary>
    public bool FollowingOsTheme => _followingOs;

    /// <summary>Raised after a new palette has been applied (or re-applied under a new filter).</summary>
    public event EventHandler<PaletteDefinition>? PaletteChanged;

    /// <summary>
    /// Create the brush set, register it in the application's resources, and apply the starting
    /// palette. Idempotent: subsequent calls just re-apply. Call once at startup, after
    /// <c>FluentTheme</c> is in <c>Application.Styles</c> and before any window is built.
    /// </summary>
    /// <example>
    /// <code>
    /// public override void OnFrameworkInitializationCompleted()
    /// {
    ///     ThemeManager.Initialize(this, PaletteCatalog.Default);
    ///     // paint views with {DynamicResource FormBgBrush} etc.; swap with:
    ///     // ThemeManager.Current.Apply("solarized-light");
    ///     base.OnFrameworkInitializationCompleted();
    /// }
    /// </code>
    /// </example>
    public static ThemeManager Initialize(Application app, PaletteDefinition initial)
    {
        _current ??= new ThemeManager();
        _current._app = app;

        // Ensure every token has a brush instance registered before first paint.
        foreach (var key in ThemeTokens.All)
        {
            if (_current._brushes.ContainsKey(key)) continue;
            var brush = new SolidColorBrush(Colors.Magenta);
            _current._brushes[key] = brush;
            app.Resources[key] = brush;
        }

        _current.Write(initial);
        return _current;
    }

    /// <summary>Swap to a different palette. All bound surfaces recolour immediately.</summary>
    public void Apply(PaletteDefinition palette) => Write(palette);

    /// <summary>Swap by palette id (built-in or custom — resolved via <see cref="PaletteRegistry"/>).</summary>
    public void Apply(string paletteId) => Write(PaletteRegistry.Instance.ById(paletteId));

    /// <summary>
    /// Set (or clear) a colour-vision-deficiency simulation applied to every token. Re-applies the
    /// current palette through the filter so the whole app updates.
    /// </summary>
    public void SetCvd(Cvd cvd)
    {
        _cvd = cvd;
        if (CurrentPalette is not null) Write(CurrentPalette);
    }

    /// <summary>The live brush for a token, or null if the id is unknown.</summary>
    public SolidColorBrush? Brush(string tokenKey) =>
        _brushes.TryGetValue(tokenKey, out var b) ? b : null;

    /// <summary>The current (post-filter) colour for a token.</summary>
    public global::Avalonia.Media.Color Color(string tokenKey) =>
        Brush(tokenKey)?.Color ?? Colors.Magenta;

    /// <summary>Build a WCAG contrast report for the current palette (pre-filter colours).</summary>
    public ContrastReport Report() => ContrastReport.For(CurrentPalette);

    // ── OS theme following ───────────────────────────────────────────────

    /// <summary>
    /// Start (or stop) following the OS light/dark setting. While on, the manager keeps the
    /// current palette's <em>family</em> but switches to whichever light/dark variant matches the
    /// OS, and reacts to the user changing their system setting. Selecting a palette from a
    /// different family keeps following enabled and re-picks the OS-appropriate variant.
    /// </summary>
    public void FollowOsTheme(bool enabled)
    {
        _followingOs = enabled;
        if (!enabled) return;

        HookOs();
        ApplyOsVariant();
    }

    private void HookOs()
    {
        if (_osHooked) return;
        var settings = _app.PlatformSettings;
        if (settings is null) return; // headless / no platform
        settings.ColorValuesChanged += (_, _) => { if (_followingOs) ApplyOsVariant(); };
        _osHooked = true;
    }

    private void ApplyOsVariant()
    {
        var wantDark = OsPrefersDark();
        var family = CurrentPalette?.Family;
        if (family is null) return;

        var match = PaletteRegistry.Instance.All
            .FirstOrDefault(p => string.Equals(p.Family, family, StringComparison.OrdinalIgnoreCase)
                                 && p.IsDark == wantDark);
        if (match is not null) Write(match);
    }

    private bool OsPrefersDark() =>
        _app.PlatformSettings?.GetColorValues().ThemeVariant == PlatformThemeVariant.Dark;

    // ── core write ───────────────────────────────────────────────────────

    private void Write(PaletteDefinition palette)
    {
        var resolved = palette.Resolve();
        foreach (var key in ThemeTokens.All)
        {
            if (!_brushes.TryGetValue(key, out var brush)) continue;
            var rgb = resolved.TryGetValue(key, out var value) ? value : new Rgb(255, 0, 255);
            if (_cvd != Cvd.None) rgb = CvdSim.Simulate(rgb, _cvd);
            brush.Color = rgb.ToColor();
        }

        // Keep Fluent's built-in control chrome (popups, scrollbars, caret) aligned with the
        // palette's light/dark polarity; our tokens layer branded surfaces on top.
        _app.RequestedThemeVariant = palette.IsDark ? ThemeVariant.Dark : ThemeVariant.Light;

        CurrentPalette = palette;
        PaletteChanged?.Invoke(this, palette);
    }
}
