# AGENTS.md

Guidance for AI coding agents. Cross-tool convention (see <https://agents.md>). If you are editing
a project that **depends on** the `ArcticGizmo.Avalonia.Palette` NuGet package, follow
**Using the package**. If you are editing **this repository**, also read `CLAUDE.md`.

---

## Using the package — `ArcticGizmo.Avalonia.Palette`

Runtime-swappable, WCAG-AA colour palettes for Avalonia. **NuGet id:** `ArcticGizmo.Avalonia.Palette`.
**Namespace / assembly:** `Palette.Theming`.

### Rules (do these; avoid the anti-patterns)

1. **Initialise once**, after `FluentTheme` is in `Application.Styles`, before any window is built:
   ```csharp
   ThemeManager.Initialize(this, PaletteCatalog.Default);
   ```
2. **Paint with tokens, never hard-coded hex.**
   - XAML: `{DynamicResource FormBgBrush}` (or `{StaticResource …}` — both track swaps).
   - Code: `ThemeManager.Current.Brush(ThemeTokens.Surface)`.
3. **NEVER replace the brush instances** in `Application.Resources`. The engine mutates each
   brush's `.Color` **in place** so every consumer recolours live; replacing an instance breaks
   all `StaticResource` consumers. Just call `ThemeManager.Current.Apply(...)`.
4. **Swap the palette** any time, from anywhere:
   ```csharp
   ThemeManager.Current.Apply("nord-dark");          // by id (built-in or custom)
   ThemeManager.Current.Apply(PaletteCatalog.SepiaLight);
   ```
5. **Token keys are the public API.** Groups: surfaces/layout, nav, editor, git-diff, syntax,
   buttons, status. House aliases you can rely on: `FormBgBrush`, `PanelBgBrush`, `FgBrush`,
   `TitleBrush`, `MutedBrush`, `AccentBrush`, `BorderBrush`, `ButtonBgBrush`, `OkBrush`,
   `WarnBrush`, `DangerBrush`. Full list: `docs/token-reference.md`.
6. **18 built-in palettes** in `PaletteCatalog` (Aurora, Solarized, Nord, Gruvbox, One,
   Tokyo Night, Rosé Pine, Sepia, High Contrast — each light + dark). `PaletteCatalog.All`
   enumerates them; ids look like `aurora-dark`, `solarized-light`.
7. **Custom palettes:** derive from a base and apply/persist —
   ```csharp
   var mine = PaletteCatalog.AuroraDark with { Id = "custom-mine", Name = "Mine",
                                               Family = "Mine", Accent = Rgb.FromHex("#FF7A00") };
   ThemeManager.Current.Apply(mine);                          // live
   new CustomPaletteStore("MyApp").SavePalette(mine);         // persist + register
   ```
   Resolve ids (built-in + custom) via `PaletteRegistry.Instance`; serialise via `PaletteCodec`.
8. **Extras:** `new ThemePreferences("MyApp")` remembers the choice;
   `ThemeManager.Current.FollowOsTheme(true)` tracks system light/dark;
   `ThemeManager.Current.SetCvd(Cvd.Deuteranopia)` previews colour-blindness;
   `Contrast.AdjustToMeet(fg, bg)` nudges a colour to WCAG AA.
9. **Keep AA.** If you add or edit a palette in a fork, run the gate — it exits non-zero on any
   sub-AA text pair: `dotnet run --project src/Palette.Sample -- --verify`.

### Minimal wire-up

```csharp
// App.axaml.cs
public override void OnFrameworkInitializationCompleted()
{
    ThemeManager.Initialize(this, PaletteCatalog.Default);
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d)
        d.MainWindow = new MainWindow();
    base.OnFrameworkInitializationCompleted();
}
```
```xml
<!-- App.axaml: FluentTheme must be present -->
<Application.Styles>
  <FluentTheme />
</Application.Styles>
```

### Reference

`README.md` · `docs/theming-guide.md` (integration, incl. AvaloniaEdit mapping) ·
`docs/token-reference.md` (every token) · `docs/palette-rationale.md` (WCAG sources).

---

## Contributing to this repository

See `CLAUDE.md` for build/run/verify, project layout, and conventions. The in-place-mutation
invariant in rule 3 above is the load-bearing design rule — do not break it.
