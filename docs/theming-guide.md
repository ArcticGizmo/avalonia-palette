# Integration guide

How to pull this template into another Avalonia app.

## 1. Reference the library

The library ships as two NuGet packages:

| Package | Depends on | Contains |
|---|---|---|
| **`ArcticGizmo.Avalonia.Palette`** | Core + Avalonia | The live engine: `ThemeManager`, the `Rgb`→brush bridge. |
| **`ArcticGizmo.Avalonia.Palette.Core`** | *(nothing — no Avalonia)* | The UI-free model: `Rgb`, `Contrast`, `CvdSim`, `ThemeTokens`, `TokenSpec`, `PaletteDefinition`, `PaletteCatalog`, `PaletteRegistry`, `PaletteCodec`, persistence. |

```bash
dotnet add package ArcticGizmo.Avalonia.Palette          # in your UI head — pulls in Core too
dotnet add package ArcticGizmo.Avalonia.Palette.Core     # in a UI-free core / tests / alt heads
```

Referencing the Avalonia package restores Core transitively, so a normal app only needs the first
line. A **UI-free core assembly** (or a headless test project, or a macOS/Linux head that has no UI
stack) can reference just `...Core` and consume the palette model, WCAG maths and CVD sim without
taking an Avalonia dependency.

The Avalonia package targets a floating `Avalonia [12.0.5, 13.0.0)`, so you don't have to
lockstep-match our exact version. Everything lives under the `ArcticGizmo.Avalonia.Palette`
namespace (colour types under `ArcticGizmo.Avalonia.Palette.Color`) — no generic top-level
`Palette` namespace to collide with a `Palette` symbol in your own app.

## 2. Initialise once at startup

`App.axaml` must have `FluentTheme` in `Application.Styles` (the tokens layer branded surfaces on
top of Fluent's control chrome):

```xml
<Application.Styles>
    <FluentTheme />
    <!-- optional: the ready-made control styles -->
    <StyleInclude Source="avares://YourApp/Styles/Controls.axaml" />
</Application.Styles>
```

Then, in `App.axaml.cs`:

```csharp
using ArcticGizmo.Avalonia.Palette;

public override void OnFrameworkInitializationCompleted()
{
    // Registers a live SolidColorBrush for every token in Application.Resources
    // and applies the starting palette. Do this BEFORE creating any window.
    ThemeManager.Initialize(this, PaletteCatalog.Default);

    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d)
        d.MainWindow = new MainWindow { DataContext = new MainWindowViewModel() };

    base.OnFrameworkInitializationCompleted();
}
```

`Initialize` also sets `Application.RequestedThemeVariant` to match the palette's light/dark
polarity, so Fluent's built-in popups, scrollbars and carets align automatically. If your app pins
its own Fluent variant and doesn't want it overwritten on every swap, opt out:

```csharp
ThemeManager.Initialize(this, PaletteCatalog.Default, manageFluentVariant: false);
```

## 3. Paint with the tokens

**XAML** — prefer `DynamicResource`:

```xml
<Border Background="{DynamicResource PanelBgBrush}"
        BorderBrush="{DynamicResource BorderBrush}" BorderThickness="1" CornerRadius="12">
    <TextBlock Foreground="{DynamicResource FgBrush}" Text="Ready"/>
</Border>
```

`StaticResource` also works and still recolours on swap (the brush instance is mutated in place),
but `DynamicResource` is the clean default.

**Code-behind** — grab the live brush instance and keep it; it recolours itself:

```csharp
myBorder.Background = ThemeManager.Current.Brush(ThemeTokens.SurfaceRaised);
myRun.Foreground   = ThemeManager.Current.Brush(ThemeTokens.SyntaxKeyword);
```

### Owner-drawn surfaces (`DrawingContext`)

Three accessors, by what you need:

```csharp
SolidColorBrush b = ThemeManager.Current.Brush(ThemeTokens.Accent); // live instance — recolours itself
Color          c = ThemeManager.Current.Color(ThemeTokens.Accent);  // current Avalonia Color (snapshot)
Rgb            r = ThemeManager.Current.Rgb(ThemeTokens.Accent);     // current Rgb, for colour arithmetic
```

Use `Rgb(token)` when you compute colours — per-item tints, blends, best-foreground picks, gradient
stops — because `Rgb` carries `MixWith` / `OverlayedBy` / `ToHex`:

```csharp
var track = ThemeManager.Current.Rgb(ThemeTokens.SurfaceSunken);
var fill  = ThemeManager.Current.Rgb(ThemeTokens.Accent);
var tint  = track.MixWith(fill, 0.5).ToColor();     // Rgb → Avalonia Color via RgbExtensions.ToColor()
```

`Color(token)` and `Rgb(token)` both return the **current, post-CVD** value — a *snapshot*, not a
live object. For the raw un-filtered palette value use `ThemeManager.Current.CurrentPalette.Resolve()[token]`.

> ⚠️ **Cached `Pen`/`ImmutablePen`/geometry don't self-recolour.** A live `SolidColorBrush` from
> `Brush()` mutates itself on swap, but the moment you read a *colour* into a cached pen or immutable
> object you've taken a snapshot that goes stale. Rebuild those on `PaletteChanged`:

```csharp
private IPen _grid = null!;

public MyOverlay()
{
    RebuildPens();
    ThemeManager.Current.PaletteChanged += (_, _) => { RebuildPens(); InvalidateVisual(); };
}

private void RebuildPens() =>
    _grid = new Pen(ThemeManager.Current.Brush(ThemeTokens.Separator), 1); // brush arg → tracks swaps
    // (a Pen built from a Color snapshot would NOT — that's the footgun.)
```

## 4. Let users swap palettes

```csharp
ThemeManager.Current.Apply("nord-dark");                 // by id
ThemeManager.Current.Apply(PaletteCatalog.SolarizedLight); // by definition

// react to swaps (e.g. to persist the choice, or repaint owner-drawn surfaces):
ThemeManager.Current.PaletteChanged += (_, palette) => Save(palette.Id);
```

`PaletteCatalog.All` enumerates the 18 built-ins; `PaletteCatalog.Family("Nord")` returns a
family's light+dark pair. Bind `All` to a `ComboBox` and you have a palette picker (see
`MainWindowViewModel` in the sample).

### Remember the choice (optional)

`ThemePreferences` persists the selected palette to a tiny JSON file under the user's app-data
folder — no dependency, best-effort, never throws:

```csharp
var prefs = new ThemePreferences("MyApp");             // folder name under %APPDATA%
ThemeManager.Initialize(this, prefs.LoadOrDefault());  // restore last choice (or default)
ThemeManager.Current.PaletteChanged += (_, p) => prefs.Save(p.Id);
```

## 5. Already using the house palette?

If your app uses `FormBgBrush`, `PanelBgBrush`, `FgBrush`, `TitleBrush`, `MutedBrush`,
`AccentBrush`, `BorderBrush`, `ButtonBgBrush`, `OkBrush`, `WarnBrush`, `DangerBrush` or `DevBrush`,
**delete your inline `<SolidColorBrush>` definitions from `App.axaml`** and call
`ThemeManager.Initialize`. Those exact keys are provided by the token contract, so your views keep
working and immediately gain light mode + live swapping. See the mapping in
[`token-reference.md`](token-reference.md) (rows tagged *(house)*).

## 6. Add your own palette

Add a `PaletteDefinition` to `PaletteCatalog` (copy the nearest family, change the seed roles),
then add it to the `All` array. Only ~33 seed values are needed — the rest derive. Finally:

```bash
dotnet run --project src/Palette.Sample -- --verify
```

to confirm it meets AA. Pin `SelectionOverride` / `CurrentLineOverride` / `CaretOverride` if a
derived value doesn't suit your scheme.

## 7. Register your own tokens (and pin semantic colours)

The built-in `ThemeTokens` set is IDE-shaped (lots of `Editor*` / `Syntax*` / `Diff*`). Apps with
their own roles — an overlay surface, a usage-bar track, per-status hues — register extra tokens so
they ride the **same** swap + in-place-mutation + CVD engine as the built-ins. Call
`RegisterTokens` **before** `Initialize`:

```csharp
ThemeManager.RegisterTokens(
    // Themes WITH the palette: recomputed from the seed on every swap.
    TokenSpec.Derived("OverlayBgBrush", def => def.SurfaceSunken.MixWith(new Rgb(0, 0, 0), 0.35)),
    TokenSpec.Derived("TreeLineBrush",  def => def.Border.MixWith(def.TextPrimary, 0.12)),

    // PINNED: a constant hue that stays put across every theme (app-owned semantics —
    // running=green, error=red — whose meaning is muscle-memory and must not drift).
    TokenSpec.Fixed("StatusRunningBrush", Rgb.FromHex("#3FB950")),
    TokenSpec.Fixed("StatusErrorBrush",   Rgb.FromHex("#F85149")));

ThemeManager.Initialize(this, PaletteCatalog.Default);
```

Each registered key is published into `Application.Resources` like a built-in, so you consume it the
same way: `{DynamicResource OverlayBgBrush}` in XAML, or `ThemeManager.Current.Brush("OverlayBgBrush")`
in code. This is the graceful middle between "everything themes" and "nothing themes": theme the
chrome, pin the semantics.

A spec whose key **matches a built-in** overrides that built-in's derivation — so you can pin a
shipped status colour without leaving the engine:

```csharp
// Hold the shipped Danger/Error colour constant across every palette swap.
ThemeManager.RegisterTokens(TokenSpec.Fixed(ThemeTokens.Danger, Rgb.FromHex("#F85149")));
```

`ThemeTokens.Error` is an alias of `ThemeTokens.Danger` (same `DangerBrush` key) if `Error` reads
better in your app.

> Pinned (`Fixed`) colours opt out of the WCAG-AA verify gate — the gate checks the built-in
> derivations against each palette's surfaces and can't reason about a constant you pin. Check your
> fixed hues against your surfaces yourself (`Contrast.Ratio` / `Contrast.AdjustToMeet` help).

## 8. Real code editors (AvaloniaEdit)

This template demonstrates the editor surface with lightweight custom controls (see
`src/Palette.Sample/Controls/CodeRenderer.cs`). If you use **AvaloniaEdit** for genuine editing,
map the tokens onto its properties:

| AvaloniaEdit | Token |
|---|---|
| `TextEditor.Background` | `EditorBackground` |
| `TextEditor.Foreground` | `EditorForeground` |
| `TextArea.SelectionBrush` | `EditorSelection` |
| `TextArea.Caret` brush | `EditorCaret` |
| current-line renderer | `EditorCurrentLine` |
| line-number margin | `EditorGutterForeground` |
| TextMate / HighlightingColor foregrounds | the `Syntax*` tokens |

Fetch each with `ThemeManager.Current.Brush(...)` and refresh on `PaletteChanged` (AvaloniaEdit's
`HighlightingColor` values are copied, not observed, so re-apply them in the handler).

## 9. Runtime custom palettes, OS-follow, colour-blind preview

**User-defined palettes.** Any `PaletteDefinition` can be applied — built-in or not — so a custom
theme is first-class. `PaletteRegistry.Instance` is the runtime set (built-ins + custom) that
pickers and `ThemeManager` resolve ids against; `CustomPaletteStore` persists user palettes as
JSON and loads them back:

```csharp
// startup: load saved custom palettes BEFORE restoring the preference
new CustomPaletteStore("MyApp").Load();
ThemeManager.Initialize(this, new ThemePreferences("MyApp").LoadOrDefault());

// build one from a base and save it (adds to the registry → appears in your picker)
var mine = PaletteCatalog.NordDark with { Id = "custom-mine", Name = "Mine", Family = "Mine",
                                          Accent = Rgb.FromHex("#FF7A00") };
new CustomPaletteStore("MyApp").SavePalette(mine);

// serialise for export / import
string json = PaletteCodec.ToJson(mine);
PaletteDefinition back = PaletteCodec.FromJson(json);

// …or a compact single-line share code for copy/paste + QR (e.g. "pal1:H4sI…")
string code = PaletteCodec.ToShareCode(mine);
PaletteDefinition shared = PaletteCodec.FromShareCode(code);  // throws FormatException if malformed
```

Bind your picker to `PaletteRegistry.Instance.All` and subscribe to `PaletteRegistry.Instance.Changed`
so custom palettes show up as they're added. (The sample's `DesignerViewModel` is a full worked
example — pickers, live preview, WCAG fixes, export/import.)

**Follow the OS.** `ThemeManager.Current.FollowOsTheme(true)` tracks the system light/dark setting
and switches to the matching variant of the current family, reacting to the user changing it.

**Contrast auto-fix.** `Contrast.AdjustToMeet(fg, bg, Contrast.AaText)` returns the nearest colour
to `fg` (blended toward black/white) that reaches the target ratio — the engine behind the
designer's *Fix* buttons; also handy to harden a palette programmatically.

**Colour-blindness preview.** `ThemeManager.Current.SetCvd(Cvd.Deuteranopia)` re-applies the
current palette through a simulation filter so you can see the whole app as a colour-blind user
would; `CvdSim.Simulate(rgb, type)` transforms a single colour. Note this mutates the **live** app
palette (every token, in place) — it's a real filter over the running app, not a side-channel
preview, so remember to clear it with `SetCvd(Cvd.None)`.

## Notes

- **Do not** replace the brush *instances* in `Application.Resources`; let `ThemeManager` mutate
  them. Replacing an instance breaks `StaticResource` consumers.
- The library is UI-thread agnostic for construction but apply palettes on the UI thread.
- The whole model — `Rgb`, `Contrast`, `CvdSim`, `PaletteDefinition`, `PaletteCatalog`,
  `PaletteCodec`, `ContrastReport` — lives in the Avalonia-free **`ArcticGizmo.Avalonia.Palette.Core`**
  package, so you can reference it from a UI-free core assembly or a headless test project and reuse
  the palette data, WCAG maths and CVD sim without an Avalonia dependency.
