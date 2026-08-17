# Distributing Palette.Theming

`Palette.Theming` is a normal .NET library, so other Avalonia apps consume it the same way they
consume any dependency. Pick the channel that matches how widely you want to share it.

Build the package first:

```bash
dotnet pack src/Palette.Theming -c Release -o artifacts
# → artifacts/ArcticGizmo.Avalonia.Palette.0.1.0.nupkg  (+ .snupkg symbols)
```

> Package id: `ArcticGizmo.Avalonia.Palette` (assembly/namespace stays `Palette.Theming`).

The package metadata (id, version, description, license, README) lives in
`src/Palette.Theming/Palette.Theming.csproj` — bump `<Version>` for each release and update
`<RepositoryUrl>` / `<PackageProjectUrl>` to your real repo before publishing publicly.

---

## Option A — Local folder feed (private, zero setup)

Best for "just let my other apps on this machine use it". The consuming app adds the `artifacts`
folder as a source. **If your machine has NuGet _package source mapping_ enabled** (many do), a
plain `--source` is ignored — add a `nuget.config` next to the consumer's `.csproj`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="palette-local" value="C:/path/to/avalonia-pallete/artifacts" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="nuget.org"><package pattern="*" /></packageSource>
    <packageSource key="palette-local"><package pattern="Palette.*" /></packageSource>
  </packageSourceMapping>
</configuration>
```

Then:

```bash
dotnet add package Palette.Theming
```

---

## Option B — GitHub Packages (private/team)

Good for sharing across a team without going public.

```bash
# one-time: add the GitHub NuGet feed (use a PAT with read/write:packages)
dotnet nuget add source "https://nuget.pkg.github.com/<OWNER>/index.json" \
  --name github --username <OWNER> --password <PAT> --store-password-in-clear-text

dotnet nuget push artifacts/ArcticGizmo.Avalonia.Palette.0.1.0.nupkg --source github --api-key <PAT>
```

Consumers add the same feed (with their own PAT) and `dotnet add package ArcticGizmo.Avalonia.Palette`.

---

## Option C — nuget.org (public)  ← chosen

For a package anyone can install. **Irreversible-ish**: the id is claimed by your account on first
push and a version can only be unlisted, not deleted.

### One-time account setup

1. Sign in at <https://www.nuget.org> with your Microsoft or GitHub account (creates the profile).
2. Enable two-factor auth if prompted — nuget.org requires it to publish.
3. Create an API key: **Account → API Keys → Create**.
   - Key name: e.g. `arcticgizmo-push`.
   - Scopes: **Push** → *Push new packages and package versions*.
   - **Glob pattern**: `ArcticGizmo.*` — scopes the key to your prefix so a leaked key can't touch
     anything else.
   - Expiry: up to 365 days.
4. You do **not** need to pre-create the package page. The first `dotnet nuget push` creates
   `ArcticGizmo.Avalonia.Palette` and assigns ownership to your account.
5. *(Optional)* Reserve the `ArcticGizmo.*` **ID prefix** (nuget.org → package → *Reserve ID
   prefix*, or via support) so only you can publish ids under it. Not required to publish.

### Push

```bash
dotnet pack src/Palette.Theming -c Release -o artifacts

dotnet nuget push artifacts/ArcticGizmo.Avalonia.Palette.0.1.0.nupkg \
  --source https://api.nuget.org/v3/index.json --api-key <NUGET_API_KEY>
# symbols too (optional, enables source-linked debugging):
dotnet nuget push artifacts/ArcticGizmo.Avalonia.Palette.0.1.0.snupkg \
  --source https://api.nuget.org/v3/index.json --api-key <NUGET_API_KEY>
```

It appears under *Manage Packages* within a minute and is installable (after indexing, a few
minutes) with `dotnet add package ArcticGizmo.Avalonia.Palette`.

---

## Option D — no package at all

If you don't want a feed, consumers can reference the project directly or add the repo as a git
submodule:

```xml
<ProjectReference Include="..\avalonia-pallete\src\Palette.Theming\Palette.Theming.csproj" />
```

---

## Versioning

Use [SemVer](https://semver.org): patch for fixes, minor for new tokens/palettes (additive),
major if you rename or remove a token key. The token contract is the public API — treat renames as
breaking. Note each release in `<PackageReleaseNotes>`.

## CI note

Gate releases on the WCAG check so a palette edit can't ship a sub-AA regression:

```bash
dotnet run --project src/Palette.Sample -- --verify   # exits non-zero on any sub-AA text pair
```
