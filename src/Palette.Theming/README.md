# Palette.Theming

Runtime-swappable, **WCAG-AA** colour palettes for [Avalonia](https://avaloniaui.net) apps, tuned
to reduce eye strain over a full working day.

- **~70 semantic tokens** (surfaces, nav, editor, git-diff, syntax, buttons, status) published as
  `SolidColorBrush` resources.
- **Live `ThemeManager`** — swaps palettes at runtime by mutating brushes *in place*, so
  `{StaticResource}`, `{DynamicResource}` and code-behind consumers all recolour with no reload.
- **18 built-in palettes** — 9 families × light/dark (Aurora, Solarized, Nord, Gruvbox, One,
  Tokyo Night, Rosé Pine, Sepia, High Contrast). All pass WCAG AA on text/syntax/status/diff.
- **WCAG utilities** (`Contrast`, `ContrastReport`) and optional **per-user persistence**
  (`ThemePreferences`).

## Install

```bash
dotnet add package ArcticGizmo.Avalonia.Palette
```

(The package id is `ArcticGizmo.Avalonia.Palette`; the assembly/namespace is `Palette.Theming`.)

## Use

Ensure `FluentTheme` is in your `Application.Styles`, then:

```csharp
using Palette.Theming;

public override void OnFrameworkInitializationCompleted()
{
    ThemeManager.Initialize(this, PaletteCatalog.Default);   // registers all brushes
    // ... create MainWindow ...
    base.OnFrameworkInitializationCompleted();
}
```

Paint with the tokens:

```xml
<Border Background="{DynamicResource PanelBgBrush}"
        BorderBrush="{DynamicResource BorderBrush}">
    <TextBlock Foreground="{DynamicResource FgBrush}" Text="Hello"/>
</Border>
```

Swap any time — every surface recolours instantly:

```csharp
ThemeManager.Current.Apply("solarized-light");
```

Already using the house token names (`FormBgBrush`, `AccentBrush`, `OkBrush`, …)? They're
first-class keys, so existing views keep working and gain light mode + live swapping for free.

Full docs, the token reference, the palette rationale (with WCAG sources) and a sample app live in
the [project repository](https://github.com/ArcticGizmo/avalonia-pallete).

## For AI agents

Imperative usage rules for coding agents are in
[`AGENTS.md`](https://github.com/ArcticGizmo/avalonia-pallete/blob/main/AGENTS.md)
(raw: `https://raw.githubusercontent.com/ArcticGizmo/avalonia-pallete/main/AGENTS.md`). It's also
bundled in this package under `docs/AGENTS.md`. The one rule that matters most: **paint with the
token brushes and call `ThemeManager.Current.Apply(...)` to swap — never replace the brush
instances in `Application.Resources`** (the engine mutates their colour in place so everything
recolours live). Every public type carries XML doc comments, so IntelliSense-driven agents get the
API surface for free.

MIT licensed.
