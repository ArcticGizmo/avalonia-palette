using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Palette.Sample.ViewModels;
using Palette.Sample.Views;
using Palette.Theming;

namespace Palette.Sample;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        // Load any user-defined palettes into the registry BEFORE restoring the preference, so a
        // saved custom palette resolves. Then register the brush set, restore the last palette,
        // and persist future swaps.
        new CustomPaletteStore("AvaloniaPalette.Sample").Load();
        var prefs = new ThemePreferences("AvaloniaPalette.Sample");
        ThemeManager.Initialize(this, prefs.LoadOrDefault());

        // Persist only palettes that exist in the registry (built-ins + saved customs). This
        // stops the designer's transient live previews from clobbering the remembered choice.
        ThemeManager.Current.PaletteChanged += (_, p) =>
        {
            if (PaletteRegistry.Instance.Find(p.Id) is not null) prefs.Save(p.Id);
        };

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
