# Avalonia Palette

Runtime-swappable, **WCAG-AA** colour palettes for [Avalonia](https://avaloniaui.net) apps — tuned
to reduce eye strain over a full working day, with light **and** dark modes, a live theme
switcher, and a built-in designer for user themes.

[![NuGet](https://img.shields.io/nuget/v/ArcticGizmo.Avalonia.Palette?logo=nuget&label=NuGet)](https://www.nuget.org/packages/ArcticGizmo.Avalonia.Palette)
[![Downloads](https://img.shields.io/nuget/dt/ArcticGizmo.Avalonia.Palette?logo=nuget&label=downloads)](https://www.nuget.org/packages/ArcticGizmo.Avalonia.Palette)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![Avalonia](https://img.shields.io/badge/Avalonia-12.0.5-8B5CF6)](https://avaloniaui.net)
![WCAG](https://img.shields.io/badge/contrast-WCAG%20AA-4ADE80)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](#license)

The reusable library ships as two NuGet packages — **`ArcticGizmo.Avalonia.Palette`** (the
Avalonia-facing library you install) and **`ArcticGizmo.Avalonia.Palette.Core`** (the UI-free
colour/WCAG/palette model it layers on, and which non-UI or test code can reference alone) —
plus **`Palette.Sample`**, a runnable demo of everything. Install the library, call one line at
startup, and paint through ~70 semantic tokens. Apps already using the house token names
(`FormBgBrush`, `AccentBrush`, `OkBrush`, …) get light mode and live swapping for free.

| Aurora · Dark (overview) | Aurora · Light (editor) | Theme designer |
|---|---|---|
| ![dark](docs/images/aurora-dark-overview.png) | ![light](docs/images/aurora-light-editor.png) | ![designer](docs/images/theme-designer.png) |

> Every screenshot is the **same app**. Swapping a palette recolours in place — no reload.

---

## Features

- **18 built-in palettes** — 9 families × light/dark, all passing WCAG AA on text, syntax, status
  and diff colours.
- **Live swapping** — change palette from anywhere; every surface recolours instantly, no reload,
  no visual-tree walk.
- **~70 semantic tokens** — surfaces/layout, nav, editor (gutter, current-line, selection, caret),
  git-diff, a full syntax set, buttons, and status/alerts.
- **Editing surfaces** — a syntax-highlighted file view, a git diff, and a genuinely editable
  [AvaloniaEdit](https://github.com/AvaloniaUI/AvaloniaEdit) surface, all driven by the tokens.
- **Theme designer** — build a custom palette from any base with live preview, one-click WCAG
  **Fix**, colour-blindness preview, and save / export / import.
- **Accessibility built in** — a WCAG contrast report + auto-fix, colour-blindness simulation, and
  a headless `--verify` gate for CI.
- **Extensible tokens** — register your app's own tokens (`ThemeManager.RegisterTokens` with
  `TokenSpec.Derived`/`TokenSpec.Fixed`), or pin a built-in to a fixed colour.
- **Quality-of-life** — follow the OS light/dark setting, remember the user's choice, opt out of
  Fluent-variant management, and share palettes as compact `pal1:` codes (great for QR).

---

## Install

```bash
dotnet add package ArcticGizmo.Avalonia.Palette
```

> Package id: `ArcticGizmo.Avalonia.Palette` · assembly/namespace: `ArcticGizmo.Avalonia.Palette`.
> It pulls in `ArcticGizmo.Avalonia.Palette.Core` (the UI-free model) transitively — non-UI or
> test code can install `...Core` alone. .NET 10 · Avalonia 12.x (floating `[12.0.5,13.0.0)`).
> Prefer no package? Reference the project directly, or see other distribution options in
> [`docs/publishing.md`](docs/publishing.md).

### Wire it up (three steps)

Ensure `FluentTheme` is in your `Application.Styles`, then:

```csharp
// 1) once at startup, before any window is built:
public override void OnFrameworkInitializationCompleted()
{
    ThemeManager.Initialize(this, PaletteCatalog.Default);   // registers all brushes
    // ... set MainWindow ...
    base.OnFrameworkInitializationCompleted();
}
```

```xml
<!-- 2) paint with the tokens (DynamicResource recommended) -->
<Border Background="{DynamicResource PanelBgBrush}"
        BorderBrush="{DynamicResource BorderBrush}">
    <TextBlock Text="Hello" Foreground="{DynamicResource FgBrush}"/>
</Border>
```

```csharp
// 3) swap any time — every surface recolours instantly:
ThemeManager.Current.Apply("solarized-light");
```

Optionally copy `src/Palette.Sample/Styles/Controls.axaml` for ready-made button / alert / panel /
editor styles. Full integration notes (incl. mapping tokens onto AvaloniaEdit):
[`docs/theming-guide.md`](docs/theming-guide.md).

---

## Run the demo

```bash
run.bat                                               # Windows: build + run
run.bat --verify                                      # Windows: headless WCAG gate

dotnet run --project src/Palette.Sample               # or directly
dotnet run --project src/Palette.Sample -- --verify   # exits 1 if any palette drops below AA
```

---

## The palettes

Each family ships a **Light** and a **Dark** variant. All pass WCAG AA on text/syntax/status/diff.

| Family | Character | Best for |
|---|---|---|
| **Aurora** | The house scheme — indigo surfaces, sky-blue accent | General day-to-day |
| **Solarized** | Ethan Schoonover's low-brightness-contrast classic | Very long sessions |
| **Nord** | Desaturated arctic blue-grey | Calm, low colour-clash |
| **Gruvbox** | Warm retro, reduced blue light | Evening / low-light rooms |
| **One** | Atom's balanced neutral slate | Broad syntax work |
| **Tokyo Night** | Deep indigo, saturated but low-glare | Focused night coding |
| **Rosé Pine** | Soft rose-and-pine, low contrast | Relaxed, aesthetic |
| **Sepia** | Paper-warm, minimal blue | Night work, reading-heavy |
| **High Contrast** | AAA-targeted, maximum legibility | Accessibility needs, glare |

---

## Design your own

The **Theme designer** builds a custom palette from any base: tweak the key roles with pickers and
the whole app previews live, check contrast as you go (one-click **Fix** to reach AA), preview it
under colour-blindness, then **Save** — it joins the switcher and persists. Export/import is JSON.

Because the engine applies *any* `PaletteDefinition`, custom themes are first-class:

```csharp
var mine = PaletteCatalog.AuroraDark with { Accent = Rgb.FromHex("#FF7A00") };
ThemeManager.Current.Apply(mine);                       // recolours live
new CustomPaletteStore("MyApp").SavePalette(mine);      // persist + add to the switcher
```

More runtime helpers:

```csharp
ThemeManager.Current.FollowOsTheme(true);               // track the system light/dark setting
ThemeManager.Current.SetCvd(Cvd.Deuteranopia);          // preview the app under colour-blindness
var ok = Contrast.AdjustToMeet(fg, bg, Contrast.AaText);// nudge a colour until it passes AA
new ThemePreferences("MyApp").LoadOrDefault();          // remember the user's choice
```

---

## Why these colours

Editing surfaces are where a developer spends ~90% of the day, so the defaults matter:

- **No pure black, no pure white.** `#000`/`#fff` hits 21:1 and causes halation and glare;
  softened near-black surfaces with off-white text stay comfortable and still clear AAA.
- **Low brightness-contrast, high hue-contrast** — the Solarized principle — so syntax stays
  legible without the page "buzzing".
- **WCAG AA on everything meaningful** — body text, syntax, status and diff all meet ≥ 4.5:1,
  proven by the `--verify` gate.

Full reasoning and sources: [`docs/palette-rationale.md`](docs/palette-rationale.md).

---

## How it works (the one clever bit)

`ThemeManager` keeps **one `SolidColorBrush` instance per token** in `Application.Resources` and,
on a swap, **mutates each brush's `.Color` in place** rather than replacing the instance. Avalonia
controls observe brush property changes, so every consumer recolours — whether it resolved the
brush via `{StaticResource}`, `{DynamicResource}`, or a direct code reference. No reload, no
visual-tree walk. **Corollary: never replace the brush instances — always call `Apply(...)`.**

---

## For AI agents

Working in a repo that uses this package? Point your agent at [`AGENTS.md`](AGENTS.md) — imperative
usage rules in the cross-tool [agents.md](https://agents.md) convention. It's also bundled in the
NuGet package under `docs/AGENTS.md`, and every public type carries XML doc comments (so
IntelliSense-driven agents get the API surface with no setup).

---

## Project layout

```
Palette.slnx
src/
  Palette.Core/             # UI-free model — pkg ArcticGizmo.Avalonia.Palette.Core (no Avalonia)
    Color/                  #   Rgb, Contrast (WCAG + auto-fix), CvdSim
    ThemeTokens.cs          #   the ~70-key token contract
    TokenSpec.cs            #   consumer-registerable / pinned token specs
    PaletteDefinition.cs    #   a palette's seed roles + derivation
    PaletteCatalog.cs       #   the 18 built-in palettes
    PaletteRegistry.cs      #   built-ins + custom palettes (runtime set)
    PaletteCodec.cs         #   palette <-> JSON + compact share codes
    CustomPaletteStore.cs   #   persist user palettes
    ContrastReport.cs       #   WCAG report model
    ThemePreferences.cs     #   optional per-user persistence
  Palette.Theming/          # Avalonia layer — pkg ArcticGizmo.Avalonia.Palette (install this)
    Color/                  #   RgbExtensions (Rgb -> Avalonia Color/brush bridge)
    ThemeManager.cs         #   live swap engine (+ CVD filter, OS-follow)
  Palette.Sample/           # the demo app
    Styles/Controls.axaml   #   shared, copy-pasteable control styles
    Controls/               #   CodeRenderer (file/diff), TokenColorizer (AvaloniaEdit)
    Views/  ViewModels/     #   pages incl. the theme designer
run.bat                     # Windows launcher
.github/workflows/          # Trusted-Publishing release workflow
docs/                       # guides (below)
```

---

## Documentation

| Doc | What's in it |
|---|---|
| [`docs/theming-guide.md`](docs/theming-guide.md) | Integrate the library; custom palettes, OS-follow, CVD, AvaloniaEdit mapping |
| [`docs/token-reference.md`](docs/token-reference.md) | Every token and its meaning |
| [`docs/palette-rationale.md`](docs/palette-rationale.md) | Colour theory + WCAG sources + per-family origins |
| [`docs/publishing.md`](docs/publishing.md) | Distribution: local feed, GitHub Packages, nuget.org (Trusted Publishing) |
| [`AGENTS.md`](AGENTS.md) · [`CLAUDE.md`](CLAUDE.md) | Guidance for AI coding agents |

---

## License

MIT — see [`LICENSE`](LICENSE). Use it however you like.
