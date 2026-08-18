using System.Collections.ObjectModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ArcticGizmo.Avalonia.Palette;
using ArcticGizmo.Avalonia.Palette.Color;

namespace Palette.Sample.ViewModels;

/// <summary>
/// A live theme designer: pick a base palette, tweak the key colour roles with pickers, watch the
/// whole app recolour as you go, check WCAG live (with one-click Fix), preview colour-blindness,
/// and save the result as a custom palette that then behaves like any built-in.
/// </summary>
public sealed partial class DesignerViewModel : PageViewModel
{
    public override string Title => "Theme designer";
    public override string Glyph => "✦";
    public override string Blurb => "Build a custom palette from any base — live preview, WCAG fixes, colour-blind check.";

    // Editable roles (key, label). Everything else derives from the base palette.
    private static readonly (string Key, string Label)[] RoleDefs =
    {
        ("surface", "App surface"), ("panel", "Panel"), ("sunken", "Sunken well"), ("border", "Border"),
        ("text", "Body text"), ("muted", "Muted text"),
        ("accent", "Accent"), ("onaccent", "On-accent"),
        ("editorbg", "Editor bg"), ("editorfg", "Editor text"),
        ("success", "Success"), ("warning", "Warning"), ("danger", "Danger"),
        ("keyword", "Keyword"), ("string", "String"), ("comment", "Comment"),
        ("function", "Function"), ("number", "Number"), ("type", "Type"),
    };

    private readonly Dictionary<string, Rgb> _values = new(StringComparer.Ordinal);
    private readonly CustomPaletteStore _store = new("AvaloniaPalette.Sample");
    private PaletteDefinition _base = PaletteCatalog.Default;
    private bool _suppress;

    public ObservableCollection<ColorRoleViewModel> Roles { get; } = new();
    public ObservableCollection<DesignerWcagRow> WcagRows { get; } = new();
    public ObservableCollection<PaletteChoiceViewModel> Bases { get; } = new();
    public Cvd[] CvdOptions { get; } = { Cvd.None, Cvd.Protanopia, Cvd.Deuteranopia, Cvd.Tritanopia };

    [ObservableProperty] private PaletteChoiceViewModel? _selectedBase;
    [ObservableProperty] private string _name = "My theme";
    [ObservableProperty] private bool _isDarkVariant = true;
    [ObservableProperty] private Cvd _selectedCvd = Cvd.None;
    [ObservableProperty] private string _status = "";

    public DesignerViewModel()
    {
        foreach (var (key, label) in RoleDefs)
            Roles.Add(new ColorRoleViewModel(key, label, OnRoleChanged));

        RebuildBases();
        PaletteRegistry.Instance.Changed += (_, _) => RebuildBases();

        // Start from whatever is currently applied — but DO NOT apply anything from the
        // constructor (the designer only takes over the live theme once the user edits).
        var current = ThemeManager.Current.CurrentPalette;
        LoadBase(current);
        _suppress = true;
        SelectedBase = Bases.FirstOrDefault(b => b.Id == current.Id);
        _suppress = false;

        RefreshWcag(); // show the report immediately, before any edit
    }

    private void RebuildBases()
    {
        var keepId = SelectedBase?.Id;
        Bases.Clear();
        foreach (var p in PaletteRegistry.Instance.All)
            Bases.Add(new PaletteChoiceViewModel(p));
        if (keepId is not null)
        {
            _suppress = true;
            SelectedBase = Bases.FirstOrDefault(b => b.Id == keepId);
            _suppress = false;
        }
    }

    partial void OnSelectedBaseChanged(PaletteChoiceViewModel? value)
    {
        if (_suppress || value is null) return;
        LoadBase(value.Palette);
        Preview();
    }

    partial void OnIsDarkVariantChanged(bool value) => Preview();

    partial void OnSelectedCvdChanged(Cvd value) => ThemeManager.Current.SetCvd(value);

    /// <summary>Seed the editable roles + name/variant from a palette (no preview).</summary>
    private void LoadBase(PaletteDefinition p)
    {
        _base = p;
        _suppress = true;

        _values["surface"] = p.Surface; _values["panel"] = p.SurfaceRaised;
        _values["sunken"] = p.SurfaceSunken; _values["border"] = p.Border;
        _values["text"] = p.TextPrimary; _values["muted"] = p.TextMuted;
        _values["accent"] = p.Accent; _values["onaccent"] = p.OnAccent;
        _values["editorbg"] = p.EditorBg; _values["editorfg"] = p.EditorFg;
        _values["success"] = p.Success; _values["warning"] = p.Warning; _values["danger"] = p.Danger;
        _values["keyword"] = p.Keyword; _values["string"] = p.Str; _values["comment"] = p.Comment;
        _values["function"] = p.Function; _values["number"] = p.Number; _values["type"] = p.Type;

        foreach (var role in Roles) role.Color = _values[role.Key].ToColor();
        IsDarkVariant = p.IsDark;
        if (!PaletteRegistry.Instance.IsCustom(p.Id)) Name = $"My {p.Name}";
        else Name = p.Name;

        _suppress = false;
    }

    private void OnRoleChanged(string key, Color color)
    {
        _values[key] = new Rgb(color.R, color.G, color.B);
        if (!_suppress) Preview();
    }

    /// <summary>Rebuild the working palette from the roles and apply it live.</summary>
    private void Preview()
    {
        if (_suppress) return;
        ThemeManager.Current.Apply(BuildWorking());
        RefreshWcag();
    }

    private PaletteDefinition BuildWorking()
    {
        var accent = _values["accent"];
        var hover = IsDarkVariant ? accent.MixWith(new Rgb(255, 255, 255), 0.25)
                                  : accent.MixWith(new Rgb(0, 0, 0), 0.20);

        return _base with
        {
            Id = "custom-" + Slug(Name),
            Name = string.IsNullOrWhiteSpace(Name) ? "Custom" : Name.Trim(),
            Family = string.IsNullOrWhiteSpace(Name) ? "Custom" : Name.Trim(),
            Variant = IsDarkVariant ? PaletteVariant.Dark : PaletteVariant.Light,
            Description = "Custom palette",
            Surface = _values["surface"], SurfaceRaised = _values["panel"],
            SurfaceSunken = _values["sunken"], Border = _values["border"],
            TextPrimary = _values["text"], TextMuted = _values["muted"],
            Accent = accent, AccentHover = hover, OnAccent = _values["onaccent"], Link = hover,
            EditorBg = _values["editorbg"], EditorFg = _values["editorfg"],
            Success = _values["success"], Warning = _values["warning"], Danger = _values["danger"],
            Keyword = _values["keyword"], Str = _values["string"], Comment = _values["comment"],
            Function = _values["function"], Number = _values["number"], Type = _values["type"],
        };
    }

    // Foreground role vs background role — the pairs a designer most needs to keep legible.
    private static readonly (string Label, string Fg, string Bg)[] Pairs =
    {
        ("Body text on surface", "text", "surface"),
        ("Muted on surface", "muted", "surface"),
        ("Editor text", "editorfg", "editorbg"),
        ("On-accent on accent", "onaccent", "accent"),
        ("Keyword", "keyword", "editorbg"), ("String", "string", "editorbg"),
        ("Comment", "comment", "editorbg"), ("Function", "function", "editorbg"),
        ("Number", "number", "editorbg"), ("Type", "type", "editorbg"),
        ("Success", "success", "surface"), ("Warning", "warning", "surface"),
        ("Danger", "danger", "surface"),
    };

    private void RefreshWcag()
    {
        if (WcagRows.Count == 0)
            foreach (var (label, fg, bg) in Pairs)
                WcagRows.Add(new DesignerWcagRow(label, fg, bg, FixCommand));

        foreach (var row in WcagRows)
        {
            var ratio = Contrast.Ratio(_values[row.Fg], _values[row.Bg]);
            row.RatioText = $"{ratio:0.0}:1";
            row.Pass = ratio >= Contrast.AaText;
        }
    }

    [RelayCommand]
    private void Fix(DesignerWcagRow row)
    {
        var fixedFg = Contrast.AdjustToMeet(_values[row.Fg], _values[row.Bg], Contrast.AaText);
        _values[row.Fg] = fixedFg;
        var role = Roles.FirstOrDefault(r => r.Key == row.Fg);
        if (role is not null)
        {
            _suppress = true;
            role.Color = fixedFg.ToColor();
            _suppress = false;
        }
        Preview();
    }

    [RelayCommand]
    private void FixAll()
    {
        foreach (var (_, fg, bg) in Pairs)
        {
            var fixedFg = Contrast.AdjustToMeet(_values[fg], _values[bg], Contrast.AaText);
            _values[fg] = fixedFg;
            var role = Roles.FirstOrDefault(r => r.Key == fg);
            if (role is not null) { _suppress = true; role.Color = fixedFg.ToColor(); _suppress = false; }
        }
        Preview();
        Status = "Adjusted all pairs to WCAG AA.";
    }

    [RelayCommand]
    private void Save()
    {
        var working = BuildWorking();
        try
        {
            _store.SavePalette(working);
            Status = $"Saved “{working.Name}”. It's now in the palette switcher.";
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
    }

    /// <summary>JSON for the current working palette (used by the view's Export button).</summary>
    public string CurrentJson() => PaletteCodec.ToJson(BuildWorking());

    /// <summary>Load a palette from pasted JSON (used by the view's Import button).</summary>
    public void ApplyImportedJson(string json)
    {
        try
        {
            var p = PaletteCodec.FromJson(json);
            LoadBase(p);
            Preview();
            Status = $"Imported “{p.Name}”.";
        }
        catch (Exception ex)
        {
            Status = "Import failed: " + ex.Message;
        }
    }

    private static string Slug(string s)
    {
        var chars = (s ?? "").Trim().ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : '-').ToArray();
        var slug = new string(chars).Trim('-');
        while (slug.Contains("--")) slug = slug.Replace("--", "-");
        return string.IsNullOrEmpty(slug) ? "theme" : slug;
    }
}

/// <summary>One editable colour role in the designer.</summary>
public sealed partial class ColorRoleViewModel : ObservableObject
{
    private readonly Action<string, Color> _onChanged;

    public ColorRoleViewModel(string key, string label, Action<string, Color> onChanged)
    {
        Key = key;
        Label = label;
        _onChanged = onChanged;
    }

    public string Key { get; }
    public string Label { get; }

    [ObservableProperty] private Color _color;

    partial void OnColorChanged(Color value) => _onChanged(Key, value);
}

/// <summary>One live WCAG row in the designer, with a Fix command.</summary>
public sealed partial class DesignerWcagRow : ObservableObject
{
    public DesignerWcagRow(string label, string fg, string bg, IRelayCommand<DesignerWcagRow> fixCommand)
    {
        Label = label;
        Fg = fg;
        Bg = bg;
        FixCommand = fixCommand;
    }

    public string Label { get; }
    public string Fg { get; }
    public string Bg { get; }
    public IRelayCommand<DesignerWcagRow> FixCommand { get; }

    [ObservableProperty] private string _ratioText = "";
    [ObservableProperty] private bool _pass;

    public IBrush StatusBrush => Pass
        ? new SolidColorBrush(Color.FromRgb(0x4A, 0xDE, 0x80))
        : new SolidColorBrush(Color.FromRgb(0xF8, 0x71, 0x71));

    partial void OnPassChanged(bool value) => OnPropertyChanged(nameof(StatusBrush));
}
