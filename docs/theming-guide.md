# Integration guide

How to pull this template into another Avalonia app.

## 1. Reference the library

Either add the project:

```xml
<ProjectReference Include="..\avalonia-pallete\src\Palette.Theming\Palette.Theming.csproj" />
```

…or copy the `src/Palette.Theming` folder into your solution. It depends only on the base
`Avalonia` package (12.0.5), so it works with any Avalonia app head (Desktop, Browser, Mobile).

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
using Palette.Theming;

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
polarity, so Fluent's built-in popups, scrollbars and carets align automatically.

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

## 7. Real code editors (AvaloniaEdit)

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

## Notes

- **Do not** replace the brush *instances* in `Application.Resources`; let `ThemeManager` mutate
  them. Replacing an instance breaks `StaticResource` consumers.
- The library is UI-thread agnostic for construction but apply palettes on the UI thread.
- `Contrast` and `Rgb` are Avalonia-free — reuse them in unit tests.
