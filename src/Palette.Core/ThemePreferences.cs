using System.Text.Json;

namespace ArcticGizmo.Avalonia.Palette;

/// <summary>
/// Optional per-user persistence for the chosen palette. Stored as a tiny JSON file under the
/// user's application-data folder, namespaced by app so multiple apps that use this template
/// don't collide.
/// <para>
/// Usage (in <c>App.OnFrameworkInitializationCompleted</c>):
/// <code>
/// var prefs = new ThemePreferences("MyApp");
/// ThemeManager.Initialize(this, prefs.LoadOrDefault());
/// ThemeManager.Current.PaletteChanged += (_, p) => prefs.Save(p.Id);
/// </code>
/// </para>
/// </summary>
public sealed class ThemePreferences
{
    private readonly string _path;

    /// <param name="appName">Folder name under %APPDATA% / ~/.config to store the preference in.</param>
    public ThemePreferences(string appName = "AvaloniaPalette")
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            appName);
        _path = Path.Combine(dir, "theme.json");
    }

    private sealed record Stored(string PaletteId);

    /// <summary>The saved palette id, or null if none/unreadable.</summary>
    public string? LoadPaletteId()
    {
        try
        {
            if (!File.Exists(_path)) return null;
            var stored = JsonSerializer.Deserialize<Stored>(File.ReadAllText(_path));
            return stored?.PaletteId;
        }
        catch
        {
            return null; // corrupt/locked file → fall back to default
        }
    }

    /// <summary>
    /// The saved palette, or <see cref="PaletteCatalog.Default"/> if none is saved/known.
    /// Resolves against <see cref="PaletteRegistry"/>, so a saved <em>custom</em> palette is
    /// restored too — load custom palettes (e.g. via <see cref="CustomPaletteStore"/>) first.
    /// </summary>
    public PaletteDefinition LoadOrDefault()
    {
        var id = LoadPaletteId();
        if (id is null) return PaletteCatalog.Default;
        return PaletteRegistry.Instance.Find(id) ?? PaletteCatalog.Default;
    }

    /// <summary>Persist the chosen palette id. Best-effort; failures are swallowed.</summary>
    public void Save(string paletteId)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, JsonSerializer.Serialize(new Stored(paletteId)));
        }
        catch
        {
            // Persisting a UI preference must never crash the app.
        }
    }
}
