# Token reference

The full semantic-token contract. Every palette resolves a value for each key, and each key is
published into `Application.Current.Resources` as a `SolidColorBrush` under the **same string
name**. Consume from XAML with `{DynamicResource <Key>}` or from code with
`ThemeManager.Current.Brush(ThemeTokens.<Name>)`.

Keys tagged **(house)** match the names used across the author's existing apps, so they drop in
unchanged. Constant names (left column) live in
[`ThemeTokens.cs`](../src/Palette.Theming/ThemeTokens.cs).

## Surfaces & layout

| `ThemeTokens.` | Resource key | Meaning |
|---|---|---|
| `Surface` | `FormBgBrush` **(house)** | App / window background |
| `SurfaceSunken` | `SurfaceSunkenBrush` | Recessed wells, gutters, sidebars |
| `SurfaceRaised` | `PanelBgBrush` **(house)** | Cards / panels |
| `SurfaceRaisedHover` | `SurfaceRaisedHoverBrush` | Panel/row hover |
| `Overlay` | `OverlayBrush` | Flyouts / popovers / dialogs |
| `Scrim` | `ScrimBrush` | Dim behind modal overlays |
| `Border` | `BorderBrush` **(house)** | Component borders |
| `Separator` | `SeparatorBrush` | Hairline dividers |
| `FocusRing` | `FocusRingBrush` | Keyboard-focus outline |

## Text

| `ThemeTokens.` | Resource key | Meaning |
|---|---|---|
| `TextPrimary` | `FgBrush` **(house)** | Body text |
| `TextTitle` | `TitleBrush` **(house)** | Headings |
| `TextMuted` | `MutedBrush` **(house)** | Secondary text |
| `TextFaint` | `FaintBrush` **(house)** | Tertiary / disabled |
| `OnAccent` | `OnAccentBrush` | Text/icon on an accent fill |
| `Link` | `LinkBrush` | Hyperlink text |

## Navigation

| `ThemeTokens.` | Resource key | Meaning |
|---|---|---|
| `NavBackground` | `NavBgBrush` | Nav rail background |
| `NavItemText` | `NavItemTextBrush` | Resting row text |
| `NavItemHover` | `NavItemHoverBrush` | Row hover fill |
| `NavItemActiveBg` | `NavItemActiveBgBrush` | Active row fill (accent wash) |
| `NavItemActiveText` | `NavItemActiveTextBrush` | Active row text |
| `NavSectionHeader` | `NavSectionBrush` | Section labels |
| `NavIndicator` | `NavIndicatorBrush` | Active-row accent bar / glyph |

## Editor surface

| `ThemeTokens.` | Resource key | Meaning |
|---|---|---|
| `EditorBackground` | `EditorBgBrush` | Editor body background |
| `EditorForeground` | `EditorFgBrush` | Default code text |
| `EditorGutterBackground` | `EditorGutterBgBrush` | Gutter background |
| `EditorGutterForeground` | `EditorGutterFgBrush` | Line numbers |
| `EditorCurrentLine` | `EditorCurrentLineBrush` | Active-line highlight |
| `EditorSelection` | `EditorSelectionBrush` | Selection wash |
| `EditorCaret` | `EditorCaretBrush` | Caret |
| `EditorIndentGuide` | `EditorIndentGuideBrush` | Indent guides |
| `EditorRuler` | `EditorRulerBrush` | 80/120-col ruler |
| `EditorFindMatch` | `EditorFindMatchBrush` | All find matches |
| `EditorFindMatchCurrent` | `EditorFindMatchCurrentBrush` | Current find match |
| `EditorBracketMatch` | `EditorBracketMatchBrush` | Matching bracket |
| `EditorWhitespace` | `EditorWhitespaceBrush` | Rendered whitespace dots/arrows |

## Syntax

| `ThemeTokens.` | Resource key |
|---|---|
| `SyntaxKeyword` | `SyntaxKeywordBrush` |
| `SyntaxString` | `SyntaxStringBrush` |
| `SyntaxNumber` | `SyntaxNumberBrush` |
| `SyntaxComment` | `SyntaxCommentBrush` |
| `SyntaxFunction` | `SyntaxFunctionBrush` |
| `SyntaxType` | `SyntaxTypeBrush` |
| `SyntaxVariable` | `SyntaxVariableBrush` |
| `SyntaxOperator` | `SyntaxOperatorBrush` |
| `SyntaxConstant` | `SyntaxConstantBrush` |
| `SyntaxTag` | `SyntaxTagBrush` |
| `SyntaxAttribute` | `SyntaxAttributeBrush` |
| `SyntaxPunctuation` | `SyntaxPunctuationBrush` |

## Git diff

| `ThemeTokens.` | Resource key | Meaning |
|---|---|---|
| `DiffAddedBg` | `DiffAddedBgBrush` | Added-line background wash |
| `DiffAddedText` | `DiffAddedFgBrush` | Added text / marker |
| `DiffAddedGutter` | `DiffAddedGutterBrush` | Added gutter |
| `DiffRemovedBg` | `DiffRemovedBgBrush` | Removed-line background wash |
| `DiffRemovedText` | `DiffRemovedFgBrush` | Removed text / marker |
| `DiffRemovedGutter` | `DiffRemovedGutterBrush` | Removed gutter |
| `DiffHunkBg` | `DiffHunkBgBrush` | Hunk-header background |
| `DiffHunkText` | `DiffHunkFgBrush` | Hunk-header text |
| `DiffWordAdded` | `DiffWordAddedBrush` | Intra-line added highlight |
| `DiffWordRemoved` | `DiffWordRemovedBrush` | Intra-line removed highlight |
| `DiffLineNumber` | `DiffLineNumberBrush` | Diff line numbers |

## Buttons & accent

| `ThemeTokens.` | Resource key | Meaning |
|---|---|---|
| `Accent` | `AccentBrush` **(house)** | Primary action fill |
| `AccentHover` | `AccentHoverBrush` | Accent hover |
| `AccentPressed` | `AccentPressedBrush` | Accent pressed |
| `AccentMuted` | `AccentMutedBrush` | Low-alpha accent wash (badges) |
| `ButtonNeutralBg` | `ButtonBgBrush` **(house)** | Secondary button fill |
| `ButtonNeutralHover` | `ButtonNeutralHoverBrush` | Secondary hover |
| `ButtonNeutralText` | `ButtonNeutralTextBrush` | Secondary text |
| `ButtonNeutralBorder` | `ButtonNeutralBorderBrush` | Secondary border |

## Status & alerts

| `ThemeTokens.` | Resource key | Meaning |
|---|---|---|
| `Success` | `OkBrush` **(house)** | Success text/icon |
| `SuccessBg` | `SuccessBgBrush` | Success banner background |
| `Warning` | `WarnBrush` **(house)** | Warning text/icon |
| `WarningBg` | `WarningBgBrush` | Warning banner background |
| `Danger` | `DangerBrush` **(house)** | Error/destructive text/icon |
| `DangerBg` | `DangerBgBrush` | Error banner background |
| `Info` | `InfoBrush` | Info text/icon |
| `InfoBg` | `InfoBgBrush` | Info banner background |
| `Dev` | `DevBrush` **(house)** | Dev/experimental badge |

## Deriving vs. seeding

A palette author only specifies ~33 **seed** roles in
[`PaletteDefinition`](../src/Palette.Theming/PaletteDefinition.cs); the rest (hover states, tints,
diff washes, selection, current-line, find matches) are **derived** in `Resolve()`. Three derived
tokens can be pinned when a scheme's canonical value matters: `SelectionOverride`,
`CurrentLineOverride`, `CaretOverride`.
