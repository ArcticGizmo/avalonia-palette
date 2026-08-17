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

## Option C — nuget.org via Trusted Publishing  ← chosen

Public, and **keyless**: instead of storing a long-lived API key, GitHub Actions mints a
short-lived (~1 hour) key from an OIDC token at push time. There is no secret to leak or rotate.
The ready-made workflow is at [`.github/workflows/publish.yml`](../.github/workflows/publish.yml).

**Irreversible-ish**: the id `ArcticGizmo.Avalonia.Palette` is claimed by your account on first
publish and a version can only be unlisted, not deleted.

### One-time setup

1. **Account** — sign in at <https://www.nuget.org> with your Microsoft or GitHub account, and
   enable 2FA if prompted.
2. **Register the trusted publishing policy** — Account → **Trusted Publishing** → new policy:
   | Field | Value |
   |---|---|
   | Repository Owner | `ArcticGizmo` |
   | Repository | `avalonia-pallete` |
   | Workflow File | `publish.yml` *(file name only — no `.github/workflows/` path)* |
   | Environment | *(leave empty — the workflow doesn't use one)* |

   > If you don't see **Trusted Publishing**, it's still rolling out to accounts — use the
   > API-key fallback below meanwhile.
3. **GitHub repo secret** — add `NUGET_USER` = your nuget.org **profile name** (not your email).
   The workflow passes it to the `NuGet/login` action.
4. You do **not** pre-create the package page; the first successful push creates it and assigns
   ownership to your account.

> **Private-repo note:** a new policy is "pending activation" for 7 days until a first publish
> locks it to your repo/owner IDs (prevents repo-name resurrection attacks). Public repos activate
> immediately.

### Publish

```bash
# bump <Version> in the csproj if you want, then tag and push:
git tag v0.1.0
git push origin v0.1.0
```

The workflow runs the WCAG gate, packs with the tag's version, exchanges the OIDC token for a
temporary key, and pushes the package + symbols. Watch it under the repo's **Actions** tab.
Installable a few minutes later with `dotnet add package ArcticGizmo.Avalonia.Palette`.

### API-key fallback (if Trusted Publishing isn't available to your account yet)

Create an API key at **Account → API Keys** scoped to glob `ArcticGizmo.*`, then:

```bash
dotnet pack src/Palette.Theming -c Release -o artifacts
dotnet nuget push artifacts/ArcticGizmo.Avalonia.Palette.0.1.0.nupkg \
  --source https://api.nuget.org/v3/index.json --api-key <NUGET_API_KEY>
```

Sources: [Trusted Publishing on nuget.org (Microsoft Learn)](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing) ·
[.NET Blog announcement](https://devblogs.microsoft.com/dotnet/enhanced-security-is-here-with-the-new-trust-publishing-on-nuget-org/)

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
