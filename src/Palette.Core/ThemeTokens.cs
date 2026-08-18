namespace ArcticGizmo.Avalonia.Palette;

/// <summary>
/// The semantic token contract. Every palette resolves to a value for each of these
/// keys, and every key is published into <c>Application.Current.Resources</c> as a
/// <c>SolidColorBrush</c> under the same string name.
/// <para>
/// Consume from XAML with <c>{DynamicResource FormBgBrush}</c> (recommended) or
/// <c>{StaticResource FormBgBrush}</c>; both track live palette swaps because the
/// brush instances are mutated in place. Consume from code via
/// <c>ThemeManager.Current.Brush(ThemeTokens.Surface)</c>.
/// </para>
/// <para>
/// Keys marked "house alias" match the names used across the author's existing apps
/// (sprig / perch / emuwren) so those projects drop in unchanged.
/// </para>
/// </summary>
public static class ThemeTokens
{
    // ── Surfaces & layout ────────────────────────────────────────────────
    public const string Surface = "FormBgBrush";               // house alias — app/window background
    public const string SurfaceSunken = "SurfaceSunkenBrush";  // recessed wells, gutters, sidebars
    public const string SurfaceRaised = "PanelBgBrush";        // house alias — cards / panels
    public const string SurfaceRaisedHover = "SurfaceRaisedHoverBrush";
    public const string Overlay = "OverlayBrush";              // flyouts / popovers / dialogs
    public const string Scrim = "ScrimBrush";                  // modal dim behind overlays
    public const string Border = "BorderBrush";                // house alias
    public const string Separator = "SeparatorBrush";          // hairline dividers
    public const string FocusRing = "FocusRingBrush";          // keyboard-focus outline

    // ── Text ─────────────────────────────────────────────────────────────
    public const string TextPrimary = "FgBrush";               // house alias — body text
    public const string TextTitle = "TitleBrush";              // house alias — headings
    public const string TextMuted = "MutedBrush";              // house alias — secondary text
    public const string TextFaint = "FaintBrush";              // house alias — tertiary/disabled
    public const string OnAccent = "OnAccentBrush";            // text/icon on an accent fill
    public const string Link = "LinkBrush";                    // hyperlink text

    // ── Navigation ───────────────────────────────────────────────────────
    public const string NavBackground = "NavBgBrush";
    public const string NavItemText = "NavItemTextBrush";
    public const string NavItemHover = "NavItemHoverBrush";
    public const string NavItemActiveBg = "NavItemActiveBgBrush";
    public const string NavItemActiveText = "NavItemActiveTextBrush";
    public const string NavSectionHeader = "NavSectionBrush";
    public const string NavIndicator = "NavIndicatorBrush";    // active-row accent bar

    // ── Editor surface (the 90%-of-the-day view) ─────────────────────────
    public const string EditorBackground = "EditorBgBrush";
    public const string EditorForeground = "EditorFgBrush";
    public const string EditorGutterBackground = "EditorGutterBgBrush";
    public const string EditorGutterForeground = "EditorGutterFgBrush"; // line numbers
    public const string EditorCurrentLine = "EditorCurrentLineBrush";   // active-line highlight
    public const string EditorSelection = "EditorSelectionBrush";
    public const string EditorCaret = "EditorCaretBrush";
    public const string EditorIndentGuide = "EditorIndentGuideBrush";
    public const string EditorRuler = "EditorRulerBrush";               // 80/120-col ruler
    public const string EditorFindMatch = "EditorFindMatchBrush";
    public const string EditorFindMatchCurrent = "EditorFindMatchCurrentBrush";
    public const string EditorBracketMatch = "EditorBracketMatchBrush";
    public const string EditorWhitespace = "EditorWhitespaceBrush";     // rendered dots/arrows

    // ── Syntax highlighting ──────────────────────────────────────────────
    public const string SyntaxKeyword = "SyntaxKeywordBrush";
    public const string SyntaxString = "SyntaxStringBrush";
    public const string SyntaxNumber = "SyntaxNumberBrush";
    public const string SyntaxComment = "SyntaxCommentBrush";
    public const string SyntaxFunction = "SyntaxFunctionBrush";
    public const string SyntaxType = "SyntaxTypeBrush";
    public const string SyntaxVariable = "SyntaxVariableBrush";
    public const string SyntaxOperator = "SyntaxOperatorBrush";
    public const string SyntaxConstant = "SyntaxConstantBrush";
    public const string SyntaxTag = "SyntaxTagBrush";
    public const string SyntaxAttribute = "SyntaxAttributeBrush";
    public const string SyntaxPunctuation = "SyntaxPunctuationBrush";

    // ── Git diff surface ─────────────────────────────────────────────────
    public const string DiffAddedBg = "DiffAddedBgBrush";
    public const string DiffAddedText = "DiffAddedFgBrush";
    public const string DiffAddedGutter = "DiffAddedGutterBrush";
    public const string DiffRemovedBg = "DiffRemovedBgBrush";
    public const string DiffRemovedText = "DiffRemovedFgBrush";
    public const string DiffRemovedGutter = "DiffRemovedGutterBrush";
    public const string DiffHunkBg = "DiffHunkBgBrush";
    public const string DiffHunkText = "DiffHunkFgBrush";
    public const string DiffWordAdded = "DiffWordAddedBrush";   // intra-line word highlight
    public const string DiffWordRemoved = "DiffWordRemovedBrush";
    public const string DiffLineNumber = "DiffLineNumberBrush";

    // ── Buttons & accent ─────────────────────────────────────────────────
    public const string Accent = "AccentBrush";               // house alias — primary action
    public const string AccentHover = "AccentHoverBrush";
    public const string AccentPressed = "AccentPressedBrush";
    public const string AccentMuted = "AccentMutedBrush";      // low-alpha accent wash
    public const string ButtonNeutralBg = "ButtonBgBrush";     // house alias — secondary button
    public const string ButtonNeutralHover = "ButtonNeutralHoverBrush";
    public const string ButtonNeutralText = "ButtonNeutralTextBrush";
    public const string ButtonNeutralBorder = "ButtonNeutralBorderBrush";

    // ── Status & alerts ──────────────────────────────────────────────────
    public const string Success = "OkBrush";                  // house alias
    public const string SuccessBg = "SuccessBgBrush";
    public const string Warning = "WarnBrush";                // house alias
    public const string WarningBg = "WarningBgBrush";
    public const string Danger = "DangerBrush";               // house alias
    public const string DangerBg = "DangerBgBrush";
    public const string Info = "InfoBrush";
    public const string InfoBg = "InfoBgBrush";
    public const string Dev = "DevBrush";                     // house alias — dev/experimental badge

    /// <summary>All token keys, in a stable declaration order (useful for reports/UIs).</summary>
    public static readonly IReadOnlyList<string> All = new[]
    {
        Surface, SurfaceSunken, SurfaceRaised, SurfaceRaisedHover, Overlay, Scrim,
        Border, Separator, FocusRing,
        TextPrimary, TextTitle, TextMuted, TextFaint, OnAccent, Link,
        NavBackground, NavItemText, NavItemHover, NavItemActiveBg, NavItemActiveText,
        NavSectionHeader, NavIndicator,
        EditorBackground, EditorForeground, EditorGutterBackground, EditorGutterForeground,
        EditorCurrentLine, EditorSelection, EditorCaret, EditorIndentGuide, EditorRuler,
        EditorFindMatch, EditorFindMatchCurrent, EditorBracketMatch, EditorWhitespace,
        SyntaxKeyword, SyntaxString, SyntaxNumber, SyntaxComment, SyntaxFunction, SyntaxType,
        SyntaxVariable, SyntaxOperator, SyntaxConstant, SyntaxTag, SyntaxAttribute, SyntaxPunctuation,
        DiffAddedBg, DiffAddedText, DiffAddedGutter, DiffRemovedBg, DiffRemovedText, DiffRemovedGutter,
        DiffHunkBg, DiffHunkText, DiffWordAdded, DiffWordRemoved, DiffLineNumber,
        Accent, AccentHover, AccentPressed, AccentMuted,
        ButtonNeutralBg, ButtonNeutralHover, ButtonNeutralText, ButtonNeutralBorder,
        Success, SuccessBg, Warning, WarningBg, Danger, DangerBg, Info, InfoBg, Dev
    };
}
