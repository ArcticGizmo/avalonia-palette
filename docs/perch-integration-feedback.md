# Integration feedback — from a real consumer (Perch)

> Written after integrating `ArcticGizmo.Avalonia.Palette` **v0.2.0** into **Perch**, a Windows
> system-tray app (.NET 10 / Avalonia 12.0.5) that already had its own hand-built theme system. Perch is
> ~80% owner-drawn (a `DrawingContext` overlay + dashboards) with a minority of XAML windows, and it keeps
> a strict UI-free core assembly. That makes it a useful stress test for the package's edges. This doc is
> feedback for the maintainer — ordered by practical impact, each item with *what it is*, *why it bit / would
> bite*, and a *concrete suggestion*.
>
> **Bottom line:** the design is genuinely close to what a themed Avalonia app needs — the in-place-mutation
> engine, the AA gate, the CVD sim, `Contrast.AdjustToMeet`, and the curated palettes are all excellent and
> we happily adopted the palettes. The items below are what stopped us going all-in on the *engine* (we
> currently harvest the palette *data* into our own model instead).

---

## 1. The root namespace `Palette` collides with consumer code  ⚠️ highest practical impact

**What.** The assembly roots a top-level namespace `Palette` (types live under `Palette.Theming`,
`Palette.Theming.Color`). `Palette` is an extremely common identifier in UI apps.

**Why it bit.** Perch has its own `static class Palette` (the app's colour façade) referenced at ~200 call
sites as `Palette.Accent`, `Palette.FormBgBrush`, etc. The moment we added the PackageReference, **every one
of those call sites failed to compile** — the global `Palette` *namespace* introduced by the package shadows
a using-imported `Palette` *type* (namespaces declared in the global namespace outrank using-directive
imports in C# name lookup). 486 errors from one `<PackageReference>`.

```
error CS0234: The type or namespace name 'Accent' does not exist in the namespace 'Palette'
```

We worked around it with an **extern alias** on the reference so the package's namespaces stay out of global
scope:

```xml
<PackageReference Include="ArcticGizmo.Avalonia.Palette" Version="0.2.0" Aliases="palettepkg" />
```
```csharp
extern alias palettepkg;
using palettepkg::Palette.Theming;
```

That works, but it's a sharp edge every consumer who has a `Palette` symbol will hit, and the fix isn't
obvious.

**Suggestion.** Root the namespace under the package identity, e.g. `ArcticGizmo.Palette` (or
`ArcticGizmo.Avalonia.Palette`), matching the NuGet id. A generic top-level `Palette` namespace is
collision-prone by construction. This is a one-time breaking change best done early, before the install base
grows — v0.x is the moment. If you keep `Palette` for terseness, at least call it out prominently in
`AGENTS.md`/README with the extern-alias remedy.

---

## 2. No UI-free core — referencing the model drags Avalonia in everywhere

**What.** `Rgb`, `Contrast`, `CvdSim`, `PaletteDefinition`, `ContrastReport`, `PaletteCodec` are all
*logically* Avalonia-free (and the docs invite reusing them in tests), but they live in the **same
`net10.0` assembly** that references `Avalonia`. So any project that references the package to touch the
*model* transitively pulls in Avalonia.

**Why it bit.** Perch keeps a strict `Perch.Core` that is UI-free on purpose — it targets plain `net10.0`,
builds on any host, backs the macOS/Linux heads, and is where the xUnit suite runs without a UI stack.
Perch already has its *own* `Rgb`/`Theme`/`Contrast` there. To adopt your model in Core we'd have to take an
Avalonia dependency into Core, which breaks the whole layering. Net effect: your engine can only live in our
UI head, quarantined away from the model/tests/other heads — so we harvested palette *values* at the head
edge instead of adopting the engine.

**Suggestion.** Split the package in two:
- **`ArcticGizmo.Palette.Core`** — netstandard2.0 / net10.0, **no Avalonia**: `Rgb`, `Contrast`, `CvdSim`,
  `PaletteDefinition`, `ContrastReport`, `PaletteCodec`, `PaletteCatalog`, `PaletteRegistry`.
- **`ArcticGizmo.Avalonia.Palette`** — depends on Core + Avalonia: `ThemeManager`, `RgbExtensions`,
  resource registration, `FollowOsTheme`, the Fluent wiring.

This mirrors how well-layered apps are structured (it's exactly Perch's `Core` vs `App` split) and lets
non-UI code, headless tests, and alternate heads consume the palette model without a UI framework. **This is
the single biggest unlock** — it's the prerequisite that would let us move from "harvest the data" to "use
the engine".

---

## 3. The token set is closed — a consumer can't register its own roles

**What.** `ThemeTokens.All` is a fixed list of ~75 keys baked into the package; `ThemeManager.Initialize`
registers exactly those brushes. There's no API to add app-specific tokens that ride the same
swap + in-place-mutation + CVD machinery.

**Why it would bite.** Perch paints ~15 roles the package has no token for: a translucent **overlay panel
surface** + its **row-hover**, a usage-bar **track** + an **expected-usage tick**, a teammate **tree-line**
connector, and (crucially, see §4) a full **semantic status set** far richer than the 5 you ship. Today
those can't participate in your engine at all — we'd have to run a second, parallel colour system beside
`ThemeManager` for them, which defeats the point.

**Suggestion.** Allow consumer-registered tokens, e.g.:

```csharp
// Register extra keys + optional derivation from the seed, before Initialize.
ThemeManager.RegisterTokens(new[]
{
    new TokenSpec("OverlayBgBrush",   def => def.SurfaceSunken.MixWith(Rgb.Black, 0.35)),
    new TokenSpec("TreeLineBrush",    def => def.Border.MixWith(def.TextPrimary, 0.12)),
    // ...
});
```

They'd then flow through `Write()`/CVD/`PaletteChanged` like the built-ins. Even a simpler "here's a bag of
extra static brushes to also register and mutate" would help.

---

## 4. No pinned / consumer-supplied semantic group; status vocabulary is small and IDE-shaped

**What.** Status colours (`Success`, `Warning`, `Danger`, `Info`, `Dev`) are **required per-palette fields**,
so they vary with every palette swap, and there's no way to (a) hold a token constant across swaps or
(b) supply status hues from the app rather than the palette. Also the vocabulary is small and coding-tool
flavoured — `Danger` doubles as "error", and there's no `running`/`idle`/`attention`/`awaiting`.

**Why it matters.** Perch is a *status* app: an at-a-glance overlay where **running=green, awaiting=yellow,
attention=orange, idle=slate, error=red, sub-agent=purple**, plus a per-teammate colour map and
permission-mode accents. Those hues *are* the app's meaning — muscle-memory across themes. Our deliberate
design rule is that themes tint chrome/text/accent and **leave the semantic hues fixed**. Your model is the
exact opposite (everything themes, and only 5 coarse status roles exist), so a straight adoption would both
*lose* most of our semantic distinctions and *destabilise* the ones that survive. This is the other reason
we kept our own semantic layer (`FixedColors`) rather than taking yours.

Worth noting: of your ~75 tokens, ~25 are `Editor*`/`Syntax*`/`Diff*` (AvaloniaEdit-oriented). A non-IDE app
uses ~15 tokens and needs ~15 you don't have — the ratio is inverted from what the token set assumes.

**Suggestion.** Two complementary features:
1. A **pinned / app-supplied semantic group** — tokens the app provides once that stay constant across
   palette swaps (or a per-token "pinned" flag). This is the graceful middle between "all themed" (you) and
   "all fixed" (us): an app opts *specific* roles into theming and pins the rest.
2. Treat the extra status roles as **consumer-registered tokens** (§3) rather than trying to enumerate every
   app's semantics upstream — but do consider renaming/aliasing `Danger→Error` and documenting that status
   is meant to be extended, so status-centric apps know the built-in 5 are a starting point, not the ceiling.

---

## 5. Owner-drawn ergonomics are a notch below the XAML path

**What.** The headline ergonomics (`{DynamicResource FormBgBrush}`) only benefit XAML. For owner-drawn code
you have `ThemeManager.Current.Brush(token)` and `.Color(token)`, which is fine — but two rough spots:
- A consumer that reads `.Color` into a cached `Pen`/`ImmutablePen`/geometry gets **no** live recolour and
  must remember to rebuild it on `PaletteChanged`. This is easy to get wrong and silent when you do.
- There's no direct "give me the raw `Rgb` of a token" for apps that do colour *arithmetic* (blends,
  per-teammate tints, best-foreground picks, usage-gradient stops). You can go via `CurrentPalette` +
  `Resolve()`, but a `ThemeManager.Current.Rgb(token)` accessor would be the natural symmetric API next to
  `.Brush()`/`.Color()`.

**Why it matters.** Perch is mostly owner-drawn and does a lot of computed colour, so this is the path we
live on, not the XAML one.

**Suggestion.** Add `Rgb Rgb(string token)` alongside `Brush`/`Color`; and in the guide, make the
"cached pens must refresh on `PaletteChanged`" caveat a prominent, worked example (the AvaloniaEdit note
hints at it, but owner-drawn `DrawingContext` consumers need it front-and-centre).

---

## 6. Smaller items

- **`ThemeManager.Write` unconditionally sets `RequestedThemeVariant`.** Apps that manage their own Fluent
  variant (Perch pins Dark) get it overwritten on every apply. Suggest an opt-out
  (`ThemeManager.Initialize(..., manageFluentVariant: false)`).
- **Persistence is JSON-only; no share code.** Perch ships a compact `perch1:` Base64 code for
  copy/paste + QR sharing of a theme. A `PaletteCodec.ToShareCode/FromShareCode` (compact, single-line)
  beside the JSON list would be a nice parity feature for any app with a "share my theme" affordance.
- **`PaletteCatalog.ById` throws `KeyNotFoundException`** while `PaletteRegistry.Find` returns null. The
  throwing default is a mild footgun for id round-tripping (a stale saved id crashes rather than falling
  back). A `TryById`/nullable `Find` on the catalog would match the registry and be safer.
- **Tight framework pin (Avalonia 12.0.5).** Fine for us (we're on 12.0.5 too), but a floating minimum
  (`[12.0.5,13.0.0)`) rather than an exact pin would reduce lockstep-upgrade friction for consumers.
- **CVD sim only re-filters through `SetCvd`.** Great for a designer preview; just document that it mutates
  the *live* app palette (not a side channel), so consumers don't accidentally leave a filter on.

---

## 7. What we happily kept

To be clear about the balance — these are genuinely good and we adopted them as-is:

- The **18 curated AA palettes** (we imported the 7 non-duplicate dark ones: Nord, Gruvbox, Solarized, One,
  Tokyo Night, Rosé Pine, Sepia). Recognisable, calm, and the AA seed values meant our text contrast came
  along for free.
- The **seed→`Resolve()`** derivation (hover/pressed/tints/washes from ~37 seeds) — we reused the same
  `Rgb.MixWith` ratios to derive our own extra roles, so imported themes stay internally consistent.
- `Rgb` being **Avalonia-free** in shape (byte triple + `FromHex`/`MixWith`/`OverlayedBy`) made the
  `PaletteDefinition → our Theme` adapter a trivial byte copy + a few blends.
- `Contrast` (incl. `AdjustToMeet`) and `CvdSim` are exactly the right utilities; the only ask is §2 (get
  them out of the Avalonia assembly).
- The **in-place brush mutation** invariant is the right core design — it's independently what our own
  engine landed on, so we know it holds up.

---

## 8. Priority summary

| # | Item | Impact | Upstream effort |
|---|------|--------|-----------------|
| 1 | Rename root namespace off generic `Palette` | **High** — breaks any consumer with a `Palette` symbol | Low (rename) |
| 2 | Split a UI-free `*.Core` package | **High** — gates model reuse in non-UI code | Medium |
| 3 | Consumer-registerable tokens | **High** for non-IDE apps | Medium |
| 4 | Pinned/app-supplied semantic group + status naming | **High** for status-centric apps | Medium |
| 5 | Owner-drawn `Rgb(token)` + cached-pen guidance | Medium | Low |
| 6 | Variant opt-out / share code / `TryById` / version range | Low–Medium | Low each |

Items **1–4** are what would let a consumer like Perch move from *harvesting the palette data* to *using the
engine* — with §2 (the core split) being the linchpin the rest depend on.

*— Perch (see its side of this analysis in `perch/docs/palette-integration-assessment.md`).*
