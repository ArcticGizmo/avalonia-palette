using ArcticGizmo.Avalonia.Palette.Color;

namespace ArcticGizmo.Avalonia.Palette;

/// <summary>
/// Describes an <em>extra</em> theme token an app registers on top of the built-in
/// <see cref="ThemeTokens"/> set (via <c>ThemeManager.RegisterTokens</c>), so the token rides the
/// same palette-swap + in-place-mutation + CVD machinery as the built-ins.
/// <para>
/// <see cref="Key"/> is the resource key the token publishes — usable from XAML as
/// <c>{DynamicResource Key}</c> and from code via <c>ThemeManager.Current.Brush(Key)</c> /
/// <c>.Color(Key)</c> / <c>.Rgb(Key)</c>. <see cref="Derive"/> computes its colour for a given
/// palette:
/// </para>
/// <list type="bullet">
///   <item><description>
///     <see cref="Derived"/> — recomputed from each palette on every swap, so the token themes
///     along with the rest of the app (e.g. a translucent overlay surface derived from the seed).
///   </description></item>
///   <item><description>
///     <see cref="Fixed"/> — a constant hue that stays put across every swap: an app-owned semantic
///     colour (running=green, error=red, a per-teammate tint) whose meaning is muscle-memory and
///     must not drift with the theme.
///   </description></item>
/// </list>
/// <para>
/// If <see cref="Key"/> matches a <em>built-in</em> token key, the spec overrides that built-in's
/// derivation. So <c>TokenSpec.Fixed(ThemeTokens.Danger, myRed)</c> pins the shipped Danger colour
/// across palette swaps — the graceful middle between "everything themes" and "nothing themes".
/// </para>
/// <para>
/// Note: a <see cref="Fixed"/> colour opts out of the WCAG-AA verify gate — the gate checks the
/// built-in derivations against each palette's surfaces, and it can't reason about a constant you
/// pin, so verify your fixed hues against your surfaces yourself.
/// </para>
/// </summary>
public sealed class TokenSpec
{
    private TokenSpec(string key, Func<PaletteDefinition, Rgb> derive, bool pinned)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Token key must be a non-empty resource key.", nameof(key));
        Key = key;
        Derive = derive;
        Pinned = pinned;
    }

    /// <summary>The resource key this token publishes into <c>Application.Resources</c>.</summary>
    public string Key { get; }

    /// <summary>Computes the token's colour for a palette. Called on every apply.</summary>
    public Func<PaletteDefinition, Rgb> Derive { get; }

    /// <summary>True when this token is a <see cref="Fixed"/> constant (does not vary with the palette).</summary>
    public bool Pinned { get; }

    /// <summary>
    /// A token whose colour is recomputed from the palette on every swap. The derivation runs
    /// against the same seed roles the built-ins use, so a derived token stays internally
    /// consistent with the theme (mirror <see cref="PaletteDefinition.Resolve"/>'s <c>MixWith</c> /
    /// <c>OverlayedBy</c> ratios).
    /// </summary>
    public static TokenSpec Derived(string key, Func<PaletteDefinition, Rgb> derive) =>
        new(key, derive ?? throw new ArgumentNullException(nameof(derive)), pinned: false);

    /// <summary>
    /// A token pinned to a constant colour across every palette swap. Use for app-owned semantic
    /// hues that must not drift with the theme. Pass a built-in <see cref="ThemeTokens"/> key to
    /// pin a shipped role (e.g. hold <see cref="ThemeTokens.Danger"/> constant).
    /// </summary>
    public static TokenSpec Fixed(string key, Rgb color) =>
        new(key, _ => color, pinned: true);
}
