using ArcticGizmo.Avalonia.Palette.Color;

namespace ArcticGizmo.Avalonia.Palette;

/// <summary>One foreground/background pairing and the WCAG result it achieves.</summary>
public sealed record ContrastCheck(string Label, Rgb Foreground, Rgb Background)
{
    public double Ratio => Contrast.Ratio(Foreground, Background);
    public WcagLevel Level => Contrast.LevelFor(Foreground, Background);

    /// <summary>Passes AA for normal body text (≥ 4.5:1).</summary>
    public bool PassesAa => Ratio >= Contrast.AaText;

    /// <summary>The ratio formatted as e.g. "12.4:1".</summary>
    public string RatioText => $"{Ratio:0.0}:1";
}

/// <summary>
/// A set of the contrast pairings that matter most for readability during long editing
/// sessions. Every built-in palette is expected to pass AA (≥ 4.5:1) on the text pairs;
/// the sample app renders this live so regressions are obvious.
/// </summary>
public sealed class ContrastReport
{
    public required PaletteDefinition Palette { get; init; }
    public required IReadOnlyList<ContrastCheck> Checks { get; init; }

    /// <summary>Text-critical checks that fall below AA, if any.</summary>
    public IEnumerable<ContrastCheck> Failures => Checks.Where(c => c.Label.StartsWith("Text") && !c.PassesAa);

    public bool AllTextPassesAa => !Failures.Any();

    public static ContrastReport For(PaletteDefinition p)
    {
        var checks = new List<ContrastCheck>
        {
            new("Text · body on app surface", p.TextPrimary, p.Surface),
            new("Text · body on panel", p.TextPrimary, p.SurfaceRaised),
            new("Text · body on editor", p.EditorFg, p.EditorBg),
            new("Text · muted on surface", p.TextMuted, p.Surface),
            new("Text · title on surface", p.TextTitle, p.Surface),
            new("Text · on-accent on accent", p.OnAccent, p.Accent),
            new("Text · link on surface", p.Link, p.Surface),
            new("Syntax · keyword", p.Keyword, p.EditorBg),
            new("Syntax · string", p.Str, p.EditorBg),
            new("Syntax · number", p.Number, p.EditorBg),
            new("Syntax · comment", p.Comment, p.EditorBg),
            new("Syntax · function", p.Function, p.EditorBg),
            new("Syntax · type", p.Type, p.EditorBg),
            new("Status · success", p.Success, p.Surface),
            new("Status · warning", p.Warning, p.Surface),
            new("Status · danger", p.Danger, p.Surface),
            new("Diff · added", p.DiffAddedText, p.EditorBg),
            new("Diff · removed", p.DiffRemovedText, p.EditorBg),
            new("UI · border on surface", p.Border, p.Surface),
        };

        return new ContrastReport { Palette = p, Checks = checks };
    }
}
