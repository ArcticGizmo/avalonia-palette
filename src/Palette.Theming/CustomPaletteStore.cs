namespace Palette.Theming;

/// <summary>
/// Persists user-defined palettes to a JSON file under the user's application-data folder, and
/// loads them back into a <see cref="PaletteRegistry"/>. Best-effort: IO/parse failures are
/// swallowed so a corrupt file can never stop the app from starting.
/// </summary>
public sealed class CustomPaletteStore
{
    private readonly string _path;
    private readonly PaletteRegistry _registry;

    public CustomPaletteStore(string appName = "AvaloniaPalette", PaletteRegistry? registry = null)
    {
        _registry = registry ?? PaletteRegistry.Instance;
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);
        _path = Path.Combine(dir, "custom-palettes.json");
    }

    /// <summary>Load saved custom palettes into the registry. Call once at startup.</summary>
    public void Load()
    {
        try
        {
            if (!File.Exists(_path)) return;
            var palettes = PaletteCodec.FromJsonList(File.ReadAllText(_path));
            if (palettes.Count > 0) _registry.AddRange(palettes);
        }
        catch
        {
            // Ignore a corrupt/locked file; the app still has all built-in palettes.
        }
    }

    /// <summary>Write the registry's current custom set to disk.</summary>
    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, PaletteCodec.ToJsonList(_registry.Custom));
        }
        catch
        {
            // Persisting is best-effort; never crash on it.
        }
    }

    /// <summary>Add/replace a custom palette in the registry and persist immediately.</summary>
    public void SavePalette(PaletteDefinition palette)
    {
        _registry.AddOrUpdate(palette);
        Save();
    }

    /// <summary>Remove a custom palette from the registry and persist.</summary>
    public void DeletePalette(string id)
    {
        if (_registry.Remove(id)) Save();
    }
}
