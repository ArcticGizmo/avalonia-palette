# CLAUDE.md — Avalonia Palette

A template that gives Avalonia apps eye-strain-friendly, user-swappable, WCAG-AA colour palettes.
Two projects: **`Palette.Theming`** (the reusable library) and **`Palette.Sample`** (the demo).

## Build / run / verify

```bash
dotnet build Palette.slnx
dotnet run   --project src/Palette.Sample                # launch the demo GUI (or run.bat)
dotnet run   --project src/Palette.Sample -- --verify    # headless WCAG gate (CI-friendly)
dotnet pack  src/Palette.Theming -c Release -o artifacts  # build the distributable NuGet package
```

`Palette.Theming` ships as a NuGet package (metadata in its `.csproj`; distribution options in
`docs/publishing.md`). Bump `<Version>` per release; the token key names are the public API, so
renaming one is a breaking change.

.NET 10 SDK · Avalonia 12.0.5 · CommunityToolkit.Mvvm · Fluent base theme. No central package
management (versions are pinned per-`.csproj`, matching the author's sibling apps).

## How theming works — the invariant that must not break

`ThemeManager` keeps **one `SolidColorBrush` per token** in `Application.Resources` and, on a
palette swap, **mutates each brush's `.Color` in place**. That in-place mutation is what makes
`{StaticResource}`, `{DynamicResource}` and code-behind consumers all recolour live.

- **Never replace a brush instance** in `Application.Resources` — always mutate. Replacing an
  instance silently breaks every `StaticResource` consumer.
- New UI paints via tokens only. In XAML use `{DynamicResource <Key>}`; in code use
  `ThemeManager.Current.Brush(ThemeTokens.<Name>)`. Don't hard-code hex in views.

## Where things live

| Concern | File |
|---|---|
| Token contract (~70 keys, incl. house aliases) | `src/Palette.Theming/ThemeTokens.cs` |
| A palette's seed roles + derivation | `src/Palette.Theming/PaletteDefinition.cs` |
| The 18 built-in palettes (9 families × L/D) | `src/Palette.Theming/PaletteCatalog.cs` |
| Live swap engine | `src/Palette.Theming/ThemeManager.cs` |
| Per-user persistence (optional) | `src/Palette.Theming/ThemePreferences.cs` |
| WCAG maths (Avalonia-free) | `src/Palette.Theming/Color/Contrast.cs` |
| WCAG report model | `src/Palette.Theming/ContrastReport.cs` |
| Shared control styles | `src/Palette.Sample/Styles/Controls.axaml` |
| Editor / diff surfaces (code-built) | `src/Palette.Sample/Controls/CodeRenderer.cs` |
| AvaloniaEdit syntax colouriser | `src/Palette.Sample/Controls/TokenColorizer.cs` |

## Rules when changing palettes

- After **any** palette edit or addition, run `-- --verify`. It exits non-zero if a
  text/syntax/status/diff pair drops below WCAG AA (4.5:1). Keep it green.
- A palette specifies only ~33 seed roles; everything else derives in `PaletteDefinition.Resolve()`.
  Prefer adjusting a seed over pinning an override.
- Preserve the house token keys (`FormBgBrush`, `AccentBrush`, `OkBrush`, …) — sibling apps
  (`sprig`, `perch`, `emuwren`) depend on those names.

## MVVM / view conventions

- `ViewLocator` maps `…ViewModels.FooViewModel` → `…Views.FooView`.
- Pages derive from `PageViewModel`; the shell (`MainWindowViewModel`) builds the nav and hosts
  the palette switcher.
- Compiled bindings are on (`x:DataType` on views). Item templates that bind to an ancestor
  command use `x:CompileBindings="False"` deliberately — keep that when editing those templates.

## Docs

- `docs/palette-rationale.md` — colour theory + WCAG citations + per-family sources.
- `docs/token-reference.md` — every token and its meaning.
- `docs/theming-guide.md` — how to consume the library in another app.
