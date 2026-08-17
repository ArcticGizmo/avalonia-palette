using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Palette.Theming;

namespace Palette.Sample.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private bool _suppressApply;

    public ObservableCollection<object> NavItems { get; } = new();
    public ObservableCollection<PaletteChoiceViewModel> Palettes { get; } =
        new(PaletteCatalog.All.Select(p => new PaletteChoiceViewModel(p)));

    private readonly List<PageViewModel> _pages;

    [ObservableProperty] private PageViewModel _currentPage;
    [ObservableProperty] private PaletteChoiceViewModel? _selectedPalette;
    [ObservableProperty] private string _currentPaletteName = "";
    [ObservableProperty] private bool _currentPassesAa;

    public MainWindowViewModel()
    {
        _pages =
        [
            new OverviewViewModel(),
            new LayoutViewModel(),
            new EditorViewModel(),
            new EditorLiveViewModel(),
            new DiffViewModel(),
            new ControlsViewModel(),
            new PalettesViewModel(),
        ];

        NavItems.Add(new NavHeaderViewModel("Explore"));
        foreach (var p in _pages.Take(2)) NavItems.Add(p);
        NavItems.Add(new NavHeaderViewModel("Editing surfaces"));
        foreach (var p in _pages.Skip(2).Take(3)) NavItems.Add(p);
        NavItems.Add(new NavHeaderViewModel("Components"));
        foreach (var p in _pages.Skip(5)) NavItems.Add(p);

        _currentPage = _pages[0];
        _currentPage.IsActive = true;

        ThemeManager.Current.PaletteChanged += (_, p) => OnPaletteChanged(p);
        OnPaletteChanged(ThemeManager.Current.CurrentPalette);
    }

    [RelayCommand]
    private void Navigate(PageViewModel page)
    {
        if (ReferenceEquals(page, CurrentPage)) return;
        CurrentPage.IsActive = false;
        CurrentPage = page;
        CurrentPage.IsActive = true;
    }

    /// <summary>Jump to the light/dark sibling of the active palette family.</summary>
    [RelayCommand]
    private void ToggleVariant()
    {
        var current = ThemeManager.Current.CurrentPalette;
        var sibling = PaletteCatalog.Family(current.Family)
            .FirstOrDefault(p => p.IsDark != current.IsDark);
        if (sibling is not null) ThemeManager.Current.Apply(sibling);
    }

    partial void OnSelectedPaletteChanged(PaletteChoiceViewModel? value)
    {
        if (_suppressApply || value is null) return;
        ThemeManager.Current.Apply(value.Palette);
    }

    private void OnPaletteChanged(PaletteDefinition p)
    {
        CurrentPaletteName = $"{p.Name} · {(p.IsDark ? "Dark" : "Light")}";
        CurrentPassesAa = ContrastReport.For(p).AllTextPassesAa;

        _suppressApply = true;
        SelectedPalette = Palettes.FirstOrDefault(x => x.Id == p.Id);
        _suppressApply = false;
    }
}
