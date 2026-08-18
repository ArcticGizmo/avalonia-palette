# ArcticGizmo.Avalonia.Palette.Core

The **UI-free core** of [ArcticGizmo.Avalonia.Palette](https://www.nuget.org/packages/ArcticGizmo.Avalonia.Palette).
No Avalonia dependency — so a non-UI core assembly, headless tests, or an alternate app head can
consume the palette model without pulling in a UI framework.

What's here:

- **`Rgb`** — a framework-agnostic byte-triple colour (`FromHex` / `MixWith` / `OverlayedBy` / `ToHex`).
- **`Contrast`** — WCAG 2.1 luminance + contrast-ratio maths, `BestForeground`, and `AdjustToMeet`.
- **`CvdSim`** — colour-vision-deficiency simulation.
- **`ThemeTokens`** — the ~75-key semantic token contract (string resource keys).
- **`TokenSpec`** — describe extra app tokens (derived from the palette, or pinned constant).
- **`PaletteDefinition` + `Resolve()`** — the seed-role model and its derivation to the full token set.
- **`PaletteCatalog` / `PaletteRegistry`** — the 18 built-in palettes and the runtime set.
- **`PaletteCodec`** — palette ↔ JSON and compact single-line share codes.
- **`ContrastReport`** — the WCAG report model.
- **`ThemePreferences` / `CustomPaletteStore`** — optional file-backed persistence.

Everything lives under the `ArcticGizmo.Avalonia.Palette` namespace (colour types under
`ArcticGizmo.Avalonia.Palette.Color`).

To actually theme a running Avalonia app — live palette swaps, `{DynamicResource}` wiring, OS-follow —
reference **`ArcticGizmo.Avalonia.Palette`**, which depends on this package and adds the `ThemeManager`
engine and the `Rgb`→brush bridge.
