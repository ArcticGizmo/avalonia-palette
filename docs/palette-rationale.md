# Palette rationale & references

Why these colours, and the evidence behind them. Every claim here is reflected in code and
checked by `dotnet run --project src/Palette.Sample -- --verify`.

## The goals, in order

1. **Legibility that survives a full day.** The editor surface is used ~90% of the working day,
   so it must be comfortable *sustained*, not just "readable for a minute".
2. **WCAG AA on everything meaningful.** Body text, syntax tokens, status colours and diff colours
   all target ≥ 4.5:1. Non-text UI (borders/dividers) is judged against the 3:1 UI-component bar.
3. **A wide, tasteful choice.** Different rooms, times of day and eyes want different things, so
   the template ships a spread rather than one "correct" theme.

## The contrast rules we hold to (WCAG 2.1)

| Rule | Ratio | Applies to |
|---|---|---|
| 1.4.3 Contrast (Minimum), normal text | **4.5:1** | body, syntax, status, diff text |
| 1.4.3 / 1.4.11, large text & UI components | **3:1** | ≥ 24px (or 18.66px bold) text, control borders |
| 1.4.6 Contrast (Enhanced), normal text | **7:1** | the High-Contrast family targets this |

Definitions of *relative luminance* and *contrast ratio* are implemented verbatim from the W3C
spec in [`Color/Contrast.cs`](../src/Palette.Core/Color/Contrast.cs).

- WCAG 2.1 SC 1.4.3 Contrast (Minimum): <https://www.w3.org/TR/WCAG21/#contrast-minimum>
- WCAG 2.1 SC 1.4.6 Contrast (Enhanced): <https://www.w3.org/TR/WCAG21/#contrast-enhanced>
- WCAG 2.1 SC 1.4.11 Non-text Contrast: <https://www.w3.org/TR/WCAG21/#non-text-contrast>
- Relative luminance / contrast-ratio formulas: <https://www.w3.org/WAI/GL/wiki/Relative_luminance>
- WebAIM Contrast Checker (handy for spot checks): <https://webaim.org/resources/contrastchecker/>

## Why not pure black or pure white

White-on-black is 21:1 — technically "maximum contrast", yet it is *harder* to read for long
periods: the extreme luminance step causes **halation** (light text appears to bleed/glow) and
glare, especially in dark rooms. The accessible-design consensus is to soften both ends: a
near-black surface (≈ `#121212`–`#1a1a1a`) with off-white text (≈ `#e0e0e0`) keeps a comfortable
13–18:1 while removing the "buzz". Our dark palettes follow this; none use `#000000`, and the
High-Contrast dark uses `#0A0A0C`, not pure black.

- Dark-mode accessibility & halation: <https://www.accessibilitychecker.org/blog/dark-mode-accessibility/>
- Dark mode best practices (avoid pure black, soften contrast): <https://dubbot.com/dubblog/2023/dark-mode-a11y.html>

## Why "low brightness-contrast, high hue-contrast"

This is the core idea behind **Solarized** (Ethan Schoonover, 2010): reduce the *brightness*
difference between text and background — which is what tires the eye — while preserving *hue*
difference so tokens stay distinct. Schoonover designed it in CIELAB for perceptual evenness.
We apply the same principle to derived tokens: selections and current-line highlights are gentle
washes (not bright fills), and syntax hues are separated by colour rather than by loudness.

- Solarized (overview & rationale): <https://en.wikipedia.org/wiki/Solarized_(color_scheme)>
- Original site: <https://ethanschoonover.com/solarized/>

> Note: canonical Solarized deliberately runs some pairs (notably comments) *below* AA. Because
> this template prioritises WCAG, our Solarized variants nudge those specific values up to reach
> AA while keeping the hue. If you want pixel-exact canonical Solarized, override the seed values
> in [`PaletteCatalog.cs`](../src/Palette.Core/PaletteCatalog.cs).

## Why a warm / low-blue option (Sepia)

Reducing short-wavelength (blue) output lowers stimulation in dim environments — the premise of
Night Shift / f.lux and of "warm" reading modes. The **Sepia** family leans amber/olive/terracotta
and avoids harsh blues, which many people find calmer late in the day. It is intentionally
*lower* contrast than the others (still AA), for relaxed reading rather than maximum crispness.

## The families and their sources

| Family | Origin / source | Notes |
|---|---|---|
| **Aurora** | The author's house palette, shared across `sprig` / `perch` / `emuwren` | `#181820` surface, `#60A5FA` accent. Light "Daybreak" companion crafted to match, using GitHub-Light-style syntax hues for AA on white. |
| **Solarized** | Ethan Schoonover — <https://ethanschoonover.com/solarized/> | Low brightness-contrast; comment/body values raised to AA. |
| **Nord** | Arctic Ice Studio — <https://www.nordtheme.com/> | Desaturated "Polar Night" / "Snow Storm". Muted status hues darkened/lightened to reach AA. |
| **Gruvbox** | morhetz — <https://github.com/morhetz/gruvbox> | Warm, retro, reduced blue light; light variant hues deepened for AA on cream. |
| **One** | Atom / GitHub's Atom One — <https://github.com/atom/one-dark-syntax> | Balanced neutral slate with a wide syntax hue spread; light variant deepened for AA on white. |
| **Tokyo Night** | enkia — <https://github.com/enkia/tokyo-night-vscode-theme> | Deep indigo "Night" + a muted "Day" companion; a few Day hues deepened for AA. |
| **Rosé Pine** | Rosé Pine — <https://rosepinetheme.com/> | Soft "main" dark + "Dawn" light; deliberately low-contrast (still AA), pine/rose/iris. |
| **Sepia** | Original to this template | Warm low-blue "reading" pair; rationale above. |
| **High Contrast** | Original to this template | Targets WCAG AAA (7:1); dark avoids pure black to limit halation. |

All nine families ship a light and a dark variant (18 palettes). Where a source scheme runs a
pairing below AA, this template nudges that value toward AA while keeping the hue; the exact seed
is overridable in `PaletteCatalog.cs`.

## Verifying

The sample app's **Palettes & WCAG** page shows a live report for the active palette. The same
checks run headless:

```bash
dotnet run --project src/Palette.Sample -- --verify
# PASS — all text / syntax / status / diff pairs meet WCAG AA (>= 4.5:1).
```

If you edit or add a palette, run this. It exits non-zero on any sub-AA text pair, so it slots
straight into CI.
