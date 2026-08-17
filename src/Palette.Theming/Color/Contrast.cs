namespace Palette.Theming.Color;

/// <summary>
/// WCAG 2.1 contrast calculations.
/// <para>
/// Reference: W3C WCAG 2.1, Success Criterion 1.4.3 (Contrast Minimum) and 1.4.6
/// (Contrast Enhanced), and the relative-luminance / contrast-ratio definitions at
/// https://www.w3.org/WAI/GL/wiki/Relative_luminance and
/// https://www.w3.org/TR/WCAG21/#dfn-contrast-ratio.
/// </para>
/// </summary>
public static class Contrast
{
    /// <summary>Minimum contrast for normal body text (WCAG AA, 1.4.3).</summary>
    public const double AaText = 4.5;

    /// <summary>Minimum contrast for large text and UI component boundaries (WCAG AA, 1.4.3 / 1.4.11).</summary>
    public const double AaLargeOrUi = 3.0;

    /// <summary>Enhanced contrast for normal body text (WCAG AAA, 1.4.6).</summary>
    public const double AaaText = 7.0;

    /// <summary>Enhanced contrast for large text (WCAG AAA, 1.4.6).</summary>
    public const double AaaLarge = 4.5;

    /// <summary>
    /// Relative luminance of a colour per the WCAG definition (sRGB → linear, then
    /// the 0.2126/0.7152/0.0722 luma weights).
    /// </summary>
    public static double RelativeLuminance(Rgb c)
    {
        static double Channel(byte v)
        {
            var s = v / 255.0;
            return s <= 0.04045 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
        }

        return 0.2126 * Channel(c.R) + 0.7152 * Channel(c.G) + 0.0722 * Channel(c.B);
    }

    /// <summary>
    /// WCAG contrast ratio between two colours, in the range 1.0 (identical) to 21.0
    /// (black vs white). Order-independent.
    /// </summary>
    public static double Ratio(Rgb a, Rgb b)
    {
        var la = RelativeLuminance(a);
        var lb = RelativeLuminance(b);
        var lighter = Math.Max(la, lb);
        var darker = Math.Min(la, lb);
        return (lighter + 0.05) / (darker + 0.05);
    }

    /// <summary>Does the pair meet WCAG AA for normal body text (≥ 4.5:1)?</summary>
    public static bool MeetsAaText(Rgb fg, Rgb bg) => Ratio(fg, bg) >= AaText;

    /// <summary>Does the pair meet WCAG AAA for normal body text (≥ 7:1)?</summary>
    public static bool MeetsAaaText(Rgb fg, Rgb bg) => Ratio(fg, bg) >= AaaText;

    /// <summary>The WCAG level a foreground/background pair achieves for normal text.</summary>
    public static WcagLevel LevelFor(Rgb fg, Rgb bg)
    {
        var r = Ratio(fg, bg);
        if (r >= AaaText) return WcagLevel.Aaa;
        if (r >= AaText) return WcagLevel.Aa;
        if (r >= AaLargeOrUi) return WcagLevel.AaLarge;
        return WcagLevel.Fail;
    }

    /// <summary>
    /// Pick whichever of <paramref name="dark"/> / <paramref name="light"/> reads best on
    /// <paramref name="background"/>. Handy for choosing on-accent text colour.
    /// </summary>
    public static Rgb BestForeground(Rgb background, Rgb dark, Rgb light) =>
        Ratio(dark, background) >= Ratio(light, background) ? dark : light;

    /// <summary>
    /// Nudge <paramref name="fg"/> toward black or white (whichever direction increases contrast
    /// against <paramref name="bg"/>) by the smallest amount that reaches <paramref name="target"/>.
    /// Hue is broadly preserved because the adjustment is a straight blend toward the achromatic
    /// extreme. Returns the input unchanged if it already meets the target, or the best achievable
    /// colour if the target is unreachable against this background.
    /// </summary>
    public static Rgb AdjustToMeet(Rgb fg, Rgb bg, double target = AaText)
    {
        if (Ratio(fg, bg) >= target) return fg;

        // Lighten on a dark background, darken on a light one.
        var toward = RelativeLuminance(bg) < 0.5 ? new Rgb(255, 255, 255) : new Rgb(0, 0, 0);

        double lo = 0, hi = 1;
        var best = toward; // full blend = maximum achievable contrast
        for (var i = 0; i < 24; i++)
        {
            var mid = (lo + hi) / 2;
            var candidate = fg.MixWith(toward, mid);
            if (Ratio(candidate, bg) >= target) { best = candidate; hi = mid; }
            else lo = mid;
        }

        return best;
    }
}

/// <summary>WCAG compliance level achieved by a colour pair for normal text.</summary>
public enum WcagLevel
{
    /// <summary>Below 3:1 — fails even for large text.</summary>
    Fail,

    /// <summary>≥ 3:1 — passes AA for large text / UI components only.</summary>
    AaLarge,

    /// <summary>≥ 4.5:1 — passes AA for normal text.</summary>
    Aa,

    /// <summary>≥ 7:1 — passes AAA for normal text.</summary>
    Aaa
}
