using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using ArcticGizmo.Avalonia.Palette.Color;

namespace ArcticGizmo.Avalonia.Palette;

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

    // Consumer-registered tokens, keyed by resource key. A key that matches a built-in overrides
    // its derivation (e.g. to pin a shipped status colour); a new key adds an extra token that
    // rides the same swap/mutation/CVD machinery as the built-ins.
    private readonly Dictionary<string, TokenSpec> _extra = new(StringComparer.Ordinal);

    private Application? _app;
    private bool _manageFluentVariant = true;
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
    /// <param name="app">The running application.</param>
    /// <param name="initial">The palette to apply first.</param>
    /// <param name="manageFluentVariant">
    /// When true (default) each apply sets <c>Application.RequestedThemeVariant</c> to match the
    /// palette's light/dark polarity, so Fluent's built-in control chrome tracks the palette. Pass
    /// false if your app pins its own Fluent variant and doesn't want it overwritten on every swap.
    /// </param>
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
    public static ThemeManager Initialize(Application app, PaletteDefinition initial, bool manageFluentVariant = true)
    {
        _current ??= new ThemeManager();
        _current._app = app;
        _current._manageFluentVariant = manageFluentVariant;

        // Ensure every token — built-in and consumer-registered — has a brush instance in the
        // application's resources before first paint.
        foreach (var key in ThemeTokens.All) _current.EnsureBrush(key);
        foreach (var key in _current._extra.Keys) _current.EnsureBrush(key);

        _current.Write(initial);
        return _current;
    }

    /// <summary>
    /// Register extra theme tokens (or override built-in ones) so they ride the same palette-swap +
    /// in-place-mutation + CVD machinery as the built-ins. Call before <see cref="Initialize"/>;
    /// calling after re-applies the current palette so the new tokens paint immediately.
    /// <para>
    /// A <see cref="TokenSpec.Derived"/> token themes with the palette; a <see cref="TokenSpec.Fixed"/>
    /// token stays constant across swaps. A spec whose <see cref="TokenSpec.Key"/> matches a built-in
    /// key overrides that built-in (e.g. pin the shipped <see cref="ThemeTokens.Danger"/> colour).
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// ThemeManager.RegisterTokens(
    ///     TokenSpec.Derived("OverlayBgBrush", def => def.SurfaceSunken.MixWith(new Rgb(0, 0, 0), 0.35)),
    ///     TokenSpec.Fixed("StatusRunningBrush", Rgb.FromHex("#3FB950")));   // constant across themes
    /// ThemeManager.Initialize(this, PaletteCatalog.Default);
    /// </code>
    /// </example>
    public static void RegisterTokens(params TokenSpec[] specs) =>
        RegisterTokens((IEnumerable<TokenSpec>)specs);

    /// <inheritdoc cref="RegisterTokens(TokenSpec[])"/>
    public static void RegisterTokens(IEnumerable<TokenSpec> specs)
    {
        if (specs is null) throw new ArgumentNullException(nameof(specs));
        _current ??= new ThemeManager();

        foreach (var spec in specs)
        {
            _current._extra[spec.Key] = spec;
            if (_current._app is not null) _current.EnsureBrush(spec.Key);
        }

        // If we're already live, re-apply so the new/overridden tokens paint now.
        if (_current._app is not null && _current.CurrentPalette is not null)
            _current.Write(_current.CurrentPalette);
    }

    private void EnsureBrush(string key)
    {
        if (_brushes.ContainsKey(key)) return;
        var brush = new SolidColorBrush(Colors.Magenta);
        _brushes[key] = brush;
        _app!.Resources[key] = brush;
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

    /// <summary>
    /// The current (post-filter) colour for a token as a framework-agnostic <see cref="Rgb"/> —
    /// the symmetric accessor to <see cref="Brush"/> / <see cref="Color"/> for owner-drawn code that
    /// does colour arithmetic (blends, per-item tints, best-foreground picks). Mirrors
    /// <see cref="Color"/>, so a CVD filter is reflected here too; for the raw un-filtered palette
    /// value use <c>CurrentPalette.Resolve()[token]</c>.
    /// </summary>
    public Rgb Rgb(string tokenKey)
    {
        var b = Brush(tokenKey);
        if (b is null) return new Rgb(255, 0, 255);
        var c = b.Color;
        return new Rgb(c.R, c.G, c.B);
    }

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
        var settings = _app?.PlatformSettings;
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
        _app?.PlatformSettings?.GetColorValues().ThemeVariant == PlatformThemeVariant.Dark;

    // ── core write ───────────────────────────────────────────────────────

    private void Write(PaletteDefinition palette)
    {
        var resolved = palette.Resolve();

        // Iterate the registered brushes (built-ins + consumer tokens). For each key: a consumer
        // spec wins (add or override), otherwise the built-in derivation, then the CVD filter.
        foreach (var (key, brush) in _brushes)
        {
            Rgb rgb;
            if (_extra.TryGetValue(key, out var spec)) rgb = spec.Derive(palette);
            else if (resolved.TryGetValue(key, out var value)) rgb = value;
            else rgb = new Rgb(255, 0, 255);

            if (_cvd != Cvd.None) rgb = CvdSim.Simulate(rgb, _cvd);
            brush.Color = rgb.ToColor();
        }

        // Keep Fluent's built-in control chrome (popups, scrollbars, caret) aligned with the
        // palette's light/dark polarity; our tokens layer branded surfaces on top. Opt out via
        // Initialize(manageFluentVariant: false) if the app pins its own Fluent variant.
        if (_manageFluentVariant && _app is not null)
            _app.RequestedThemeVariant = palette.IsDark ? ThemeVariant.Dark : ThemeVariant.Light;

        CurrentPalette = palette;
        PaletteChanged?.Invoke(this, palette);
    }
}
