using System.Collections.ObjectModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ArcticGizmo.Avalonia.Palette;
using ArcticGizmo.Avalonia.Palette.Color;

namespace Palette.Sample.ViewModels;

public sealed partial class PalettesViewModel : PageViewModel
{
    public override string Title => "Palettes & WCAG";
    public override string Glyph => "◑";
    public override string Blurb => "Every built-in palette, with a live contrast report against the current one.";

    public ObservableCollection<PaletteChoiceViewModel> Palettes { get; } = new();

    public ObservableCollection<ContrastRowViewModel> Checks { get; } = new();

    [ObservableProperty] private string _currentName = "";
    [ObservableProperty] private bool _currentPassesAa;

    public PalettesViewModel()
    {
        RebuildPalettes();
        PaletteRegistry.Instance.Changed += (_, _) => RebuildPalettes();
        ThemeManager.Current.PaletteChanged += (_, p) => Refresh(p);
        Refresh(ThemeManager.Current.CurrentPalette);
    }

    private void RebuildPalettes()
    {
        Palettes.Clear();
        foreach (var p in PaletteRegistry.Instance.All)
            Palettes.Add(new PaletteChoiceViewModel(p));
    }

    [RelayCommand]
    private void Apply(PaletteChoiceViewModel choice) => ThemeManager.Current.Apply(choice.Palette);

    private void Refresh(PaletteDefinition p)
    {
        CurrentName = $"{p.Name} · {(p.IsDark ? "Dark" : "Light")}";
        var report = ContrastReport.For(p);
        CurrentPassesAa = report.AllTextPassesAa;

        Checks.Clear();
        foreach (var c in report.Checks)
            Checks.Add(new ContrastRowViewModel(c));
    }
}

/// <summary>One row of the live WCAG report.</summary>
public sealed class ContrastRowViewModel(ContrastCheck check)
{
    public string Label => check.Label;
    public string RatioText => check.RatioText;

    public string LevelText => check.Level switch
    {
        WcagLevel.Aaa => "AAA",
        WcagLevel.Aa => "AA",
        WcagLevel.AaLarge => "AA large",
        _ => "fail"
    };

    // Non-text UI (borders) is judged at the 3:1 bar, not 4.5:1.
    private bool IsUi => Label.Contains("UI", StringComparison.OrdinalIgnoreCase);
    private bool Ok => IsUi ? check.Ratio >= Contrast.AaLargeOrUi : check.PassesAa;

    public IBrush StatusBrush => new SolidColorBrush(
        Ok ? global::Avalonia.Media.Color.FromRgb(0x4A, 0xDE, 0x80)
           : global::Avalonia.Media.Color.FromRgb(0xF8, 0x71, 0x71));
}
