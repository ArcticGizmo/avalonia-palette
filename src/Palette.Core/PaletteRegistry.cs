namespace ArcticGizmo.Avalonia.Palette;

/// <summary>
/// The runtime set of palettes available to an app: the built-ins from
/// <see cref="PaletteCatalog"/> plus any user-defined ones added at runtime. This is what UIs
/// (palette pickers, galleries) and <see cref="ThemeManager"/> resolve ids against, so a custom
/// palette behaves exactly like a built-in once added.
/// </summary>
public sealed class PaletteRegistry
{
    /// <summary>The process-wide registry.</summary>
    public static PaletteRegistry Instance { get; } = new();

    private readonly List<PaletteDefinition> _builtIns = PaletteCatalog.All.ToList();
    private readonly List<PaletteDefinition> _custom = new();

    private PaletteRegistry() { }

    /// <summary>Raised whenever the custom set changes (add / update / remove).</summary>
    public event EventHandler? Changed;

    public IReadOnlyList<PaletteDefinition> BuiltIns => _builtIns;
    public IReadOnlyList<PaletteDefinition> Custom => _custom;

    /// <summary>Built-ins followed by custom palettes, in display order.</summary>
    public IReadOnlyList<PaletteDefinition> All => _builtIns.Concat(_custom).ToList();

    /// <summary>True if the id belongs to a user-defined palette.</summary>
    public bool IsCustom(string id) => _custom.Any(p => IdEq(p.Id, id));

    /// <summary>Find a palette by id across built-ins and custom, or null.</summary>
    public PaletteDefinition? Find(string id) => All.FirstOrDefault(p => IdEq(p.Id, id));

    /// <summary>Find a palette by id, throwing if unknown.</summary>
    public PaletteDefinition ById(string id) =>
        Find(id) ?? throw new KeyNotFoundException($"No palette with id '{id}'.");

    /// <summary>
    /// Add a custom palette, or replace an existing custom one with the same id. Built-in ids are
    /// rejected so a user theme can't shadow a shipped one.
    /// </summary>
    public void AddOrUpdate(PaletteDefinition palette)
    {
        if (_builtIns.Any(p => IdEq(p.Id, palette.Id)))
            throw new InvalidOperationException($"'{palette.Id}' is a built-in palette id; choose another.");

        _custom.RemoveAll(p => IdEq(p.Id, palette.Id));
        _custom.Add(palette);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Add several custom palettes at once (e.g. on load), raising Changed once.</summary>
    public void AddRange(IEnumerable<PaletteDefinition> palettes)
    {
        foreach (var p in palettes)
        {
            if (_builtIns.Any(b => IdEq(b.Id, p.Id))) continue;
            _custom.RemoveAll(x => IdEq(x.Id, p.Id));
            _custom.Add(p);
        }
        Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Remove a custom palette. No-op for built-ins / unknown ids.</summary>
    public bool Remove(string id)
    {
        var removed = _custom.RemoveAll(p => IdEq(p.Id, id)) > 0;
        if (removed) Changed?.Invoke(this, EventArgs.Empty);
        return removed;
    }

    private static bool IdEq(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
}
