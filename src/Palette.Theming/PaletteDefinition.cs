using Palette.Theming.Color;

namespace Palette.Theming;

/// <summary>Whether a palette is a light or dark scheme.</summary>
public enum PaletteVariant
{
    Light,
    Dark
}

/// <summary>
/// A single palette (one light or one dark scheme). Authors specify a compact seed of
/// perceptually meaningful roles; <see cref="Resolve"/> derives the full
/// <see cref="ThemeTokens"/> contract from them (hover states, tints, diff washes, etc.),
/// so a palette definition stays short and internally consistent.
/// <para>
/// A handful of derived tokens can be pinned via the <c>*Override</c> properties when a
/// scheme's canonical value matters (e.g. Solarized's specific selection colour).
/// </para>
/// </summary>
public sealed record PaletteDefinition
{
    // ── Identity ─────────────────────────────────────────────────────────
    public required string Id { get; init; }          // stable key, e.g. "nord-dark"
    public required string Name { get; init; }         // display, e.g. "Nord"
    public required string Family { get; init; }       // groups a light+dark pair, e.g. "Nord"
    public required PaletteVariant Variant { get; init; }
    public string Description { get; init; } = "";
    public bool IsDark => Variant == PaletteVariant.Dark;

    // ── Surfaces ─────────────────────────────────────────────────────────
    public required Rgb Surface { get; init; }
    public required Rgb SurfaceSunken { get; init; }
    public required Rgb SurfaceRaised { get; init; }
    public required Rgb Overlay { get; init; }
    public required Rgb Border { get; init; }
    public required Rgb Separator { get; init; }
    public required Rgb ButtonBg { get; init; }

    // ── Text ─────────────────────────────────────────────────────────────
    public required Rgb TextPrimary { get; init; }
    public required Rgb TextTitle { get; init; }
    public required Rgb TextMuted { get; init; }
    public required Rgb TextFaint { get; init; }

    // ── Accent ───────────────────────────────────────────────────────────
    public required Rgb Accent { get; init; }
    public required Rgb AccentHover { get; init; }
    public required Rgb OnAccent { get; init; }   // text/icon drawn on an accent fill
    public required Rgb Link { get; init; }

    // ── Status ───────────────────────────────────────────────────────────
    public required Rgb Success { get; init; }
    public required Rgb Warning { get; init; }
    public required Rgb Danger { get; init; }
    public required Rgb Info { get; init; }
    public required Rgb Dev { get; init; }

    // ── Editor ───────────────────────────────────────────────────────────
    public required Rgb EditorBg { get; init; }
    public required Rgb EditorFg { get; init; }
    public required Rgb EditorGutterFg { get; init; }

    // ── Syntax ───────────────────────────────────────────────────────────
    public required Rgb Keyword { get; init; }
    public required Rgb Str { get; init; }
    public required Rgb Number { get; init; }
    public required Rgb Comment { get; init; }
    public required Rgb Function { get; init; }
    public required Rgb Type { get; init; }
    public required Rgb Variable { get; init; }
    public required Rgb Operator { get; init; }
    public required Rgb Constant { get; init; }
    public required Rgb Tag { get; init; }
    public required Rgb Attribute { get; init; }
    public required Rgb Punctuation { get; init; }

    // ── Diff ─────────────────────────────────────────────────────────────
    public required Rgb DiffAddedText { get; init; }
    public required Rgb DiffRemovedText { get; init; }

    // ── Optional pins for otherwise-derived tokens ───────────────────────
    public Rgb? SelectionOverride { get; init; }
    public Rgb? CurrentLineOverride { get; init; }
    public Rgb? CaretOverride { get; init; }

    /// <summary>
    /// Expand the seed into the full <see cref="ThemeTokens"/> map. Derivation rules are
    /// tuned to keep brightness contrast comfortable (soft current-line/selection washes)
    /// while preserving hue contrast for legibility — the Solarized principle.
    /// </summary>
    public IReadOnlyDictionary<string, Rgb> Resolve()
    {
        var selection = SelectionOverride ?? EditorBg.OverlayedBy(Accent, 0.26);
        var currentLine = CurrentLineOverride ?? EditorBg.MixWith(TextPrimary, IsDark ? 0.06 : 0.05);
        var caret = CaretOverride ?? Accent;

        return new Dictionary<string, Rgb>
        {
            // Surfaces & layout
            [ThemeTokens.Surface] = Surface,
            [ThemeTokens.SurfaceSunken] = SurfaceSunken,
            [ThemeTokens.SurfaceRaised] = SurfaceRaised,
            [ThemeTokens.SurfaceRaisedHover] = SurfaceRaised.MixWith(TextPrimary, 0.07),
            [ThemeTokens.Overlay] = Overlay,
            [ThemeTokens.Scrim] = Surface.MixWith(new Rgb(0, 0, 0), IsDark ? 0.6 : 0.45),
            [ThemeTokens.Border] = Border,
            [ThemeTokens.Separator] = Separator,
            [ThemeTokens.FocusRing] = AccentHover,

            // Text
            [ThemeTokens.TextPrimary] = TextPrimary,
            [ThemeTokens.TextTitle] = TextTitle,
            [ThemeTokens.TextMuted] = TextMuted,
            [ThemeTokens.TextFaint] = TextFaint,
            [ThemeTokens.OnAccent] = OnAccent,
            [ThemeTokens.Link] = Link,

            // Navigation
            [ThemeTokens.NavBackground] = SurfaceRaised,
            [ThemeTokens.NavItemText] = TextMuted,
            [ThemeTokens.NavItemHover] = SurfaceRaised.MixWith(TextPrimary, 0.07),
            [ThemeTokens.NavItemActiveBg] = SurfaceRaised.OverlayedBy(Accent, 0.16),
            [ThemeTokens.NavItemActiveText] = TextTitle,
            [ThemeTokens.NavSectionHeader] = TextFaint,
            [ThemeTokens.NavIndicator] = Accent,

            // Editor surface
            [ThemeTokens.EditorBackground] = EditorBg,
            [ThemeTokens.EditorForeground] = EditorFg,
            [ThemeTokens.EditorGutterBackground] = EditorBg,
            [ThemeTokens.EditorGutterForeground] = EditorGutterFg,
            [ThemeTokens.EditorCurrentLine] = currentLine,
            [ThemeTokens.EditorSelection] = selection,
            [ThemeTokens.EditorCaret] = caret,
            [ThemeTokens.EditorIndentGuide] = EditorBg.MixWith(TextPrimary, 0.14),
            [ThemeTokens.EditorRuler] = EditorBg.MixWith(TextPrimary, 0.11),
            [ThemeTokens.EditorFindMatch] = EditorBg.OverlayedBy(Warning, 0.34),
            [ThemeTokens.EditorFindMatchCurrent] = EditorBg.OverlayedBy(Warning, 0.60),
            [ThemeTokens.EditorBracketMatch] = EditorBg.OverlayedBy(Accent, 0.32),
            [ThemeTokens.EditorWhitespace] = EditorBg.MixWith(TextPrimary, 0.28),

            // Syntax
            [ThemeTokens.SyntaxKeyword] = Keyword,
            [ThemeTokens.SyntaxString] = Str,
            [ThemeTokens.SyntaxNumber] = Number,
            [ThemeTokens.SyntaxComment] = Comment,
            [ThemeTokens.SyntaxFunction] = Function,
            [ThemeTokens.SyntaxType] = Type,
            [ThemeTokens.SyntaxVariable] = Variable,
            [ThemeTokens.SyntaxOperator] = Operator,
            [ThemeTokens.SyntaxConstant] = Constant,
            [ThemeTokens.SyntaxTag] = Tag,
            [ThemeTokens.SyntaxAttribute] = Attribute,
            [ThemeTokens.SyntaxPunctuation] = Punctuation,

            // Git diff
            [ThemeTokens.DiffAddedBg] = EditorBg.OverlayedBy(DiffAddedText, IsDark ? 0.16 : 0.13),
            [ThemeTokens.DiffAddedText] = DiffAddedText,
            [ThemeTokens.DiffAddedGutter] = DiffAddedText,
            [ThemeTokens.DiffRemovedBg] = EditorBg.OverlayedBy(DiffRemovedText, IsDark ? 0.16 : 0.13),
            [ThemeTokens.DiffRemovedText] = DiffRemovedText,
            [ThemeTokens.DiffRemovedGutter] = DiffRemovedText,
            [ThemeTokens.DiffHunkBg] = EditorBg.OverlayedBy(Accent, 0.12),
            [ThemeTokens.DiffHunkText] = Accent,
            [ThemeTokens.DiffWordAdded] = EditorBg.OverlayedBy(DiffAddedText, 0.32),
            [ThemeTokens.DiffWordRemoved] = EditorBg.OverlayedBy(DiffRemovedText, 0.32),
            [ThemeTokens.DiffLineNumber] = EditorGutterFg,

            // Buttons & accent
            [ThemeTokens.Accent] = Accent,
            [ThemeTokens.AccentHover] = AccentHover,
            [ThemeTokens.AccentPressed] = Accent.MixWith(new Rgb(0, 0, 0), 0.16),
            [ThemeTokens.AccentMuted] = Surface.OverlayedBy(Accent, 0.16),
            [ThemeTokens.ButtonNeutralBg] = ButtonBg,
            [ThemeTokens.ButtonNeutralHover] = ButtonBg.MixWith(TextPrimary, 0.09),
            [ThemeTokens.ButtonNeutralText] = TextPrimary,
            [ThemeTokens.ButtonNeutralBorder] = Border,

            // Status & alerts
            [ThemeTokens.Success] = Success,
            [ThemeTokens.SuccessBg] = Surface.OverlayedBy(Success, IsDark ? 0.16 : 0.14),
            [ThemeTokens.Warning] = Warning,
            [ThemeTokens.WarningBg] = Surface.OverlayedBy(Warning, IsDark ? 0.16 : 0.14),
            [ThemeTokens.Danger] = Danger,
            [ThemeTokens.DangerBg] = Surface.OverlayedBy(Danger, IsDark ? 0.16 : 0.14),
            [ThemeTokens.Info] = Info,
            [ThemeTokens.InfoBg] = Surface.OverlayedBy(Info, IsDark ? 0.16 : 0.14),
            [ThemeTokens.Dev] = Dev,
        };
    }
}
