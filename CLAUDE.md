# CLAUDE.md — Avalonia Palette

A template that gives Avalonia apps eye-strain-friendly, user-swappable, WCAG-AA colour palettes.
Three projects: **`Palette.Core`** (`src/Palette.Core`, the UI-free model — no Avalonia),
**`Palette.Theming`** (`src/Palette.Theming`, the Avalonia engine layered on Core) and
**`Palette.Sample`** (the demo). The namespace for both library projects is
`ArcticGizmo.Avalonia.Palette` (colour types under `…Palette.Color`).

## Build / run / verify

```bash
dotnet build Palette.slnx
dotnet run   --project src/Palette.Sample                # launch the demo GUI (or run.bat)
dotnet run   --project src/Palette.Sample -- --verify    # headless WCAG gate (CI-friendly)
dotnet pack  Palette.slnx -c Release -o artifacts        # build both distributable NuGet packages
```

The library ships as **two** NuGet packages: **`ArcticGizmo.Avalonia.Palette.Core`** (`Palette.Core`,
net10.0, no Avalonia — the model) and **`ArcticGizmo.Avalonia.Palette`** (`Palette.Theming`, the
Avalonia engine, which depends on Core). Assembly names match the package ids; the shared namespace
is `ArcticGizmo.Avalonia.Palette`. Releases are automated: pushing a `v*` tag runs
`.github/workflows/publish.yml`, which WCAG-gates, packs the **solution** with the tag's version
(both packages, engine pinned to the same Core version), and publishes to nuget.org via **Trusted
Publishing** (OIDC — no stored API key; needs repo secret `NUGET_USER` and a nuget.org policy
covering **both** package ids). Distribution options: `docs/publishing.md`. The token *string* key
names (`FormBgBrush`, …) are the public API, so renaming one is a breaking change; the C# namespace
was renamed off the generic `Palette` in 0.3.0 (string keys were untouched).

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
| Token contract (~75 keys, incl. house aliases + `Error`) | `src/Palette.Core/ThemeTokens.cs` |
| Consumer-registered / pinned token specs | `src/Palette.Core/TokenSpec.cs` |
| A palette's seed roles + derivation | `src/Palette.Core/PaletteDefinition.cs` |
| The 18 built-in palettes (9 families × L/D) | `src/Palette.Core/PaletteCatalog.cs` |
| Built-ins + custom palettes (runtime set) | `src/Palette.Core/PaletteRegistry.cs` |
| Palette ↔ JSON + compact share codes | `src/Palette.Core/PaletteCodec.cs` |
| Persist user palettes | `src/Palette.Core/CustomPaletteStore.cs` |
| Live swap engine (+ CVD filter, OS-follow, token registration) | `src/Palette.Theming/ThemeManager.cs` |
| Per-user persistence (optional) | `src/Palette.Core/ThemePreferences.cs` |
| WCAG maths + AdjustToMeet (Avalonia-free) | `src/Palette.Core/Color/Contrast.cs` |
| Colour-blindness simulation | `src/Palette.Core/Color/CvdSim.cs` |
| Rgb → Avalonia brush/colour bridge | `src/Palette.Theming/Color/RgbExtensions.cs` |
| WCAG report model | `src/Palette.Core/ContrastReport.cs` |
| Shared control styles | `src/Palette.Sample/Styles/Controls.axaml` |
| Editor / diff surfaces (code-built) | `src/Palette.Sample/Controls/CodeRenderer.cs` |
| AvaloniaEdit syntax colouriser | `src/Palette.Sample/Controls/TokenColorizer.cs` |
| Theme designer page | `src/Palette.Sample/ViewModels/DesignerViewModel.cs` |

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
