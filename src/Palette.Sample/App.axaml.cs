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
        // Restore the user's last palette (or the default), register the themed brush set, and
        // persist any future swap. Doing this before any view is built means
        // {StaticResource}/{DynamicResource} lookups resolve on first paint.
        var prefs = new ThemePreferences("AvaloniaPalette.Sample");
        ThemeManager.Initialize(this, prefs.LoadOrDefault());
        ThemeManager.Current.PaletteChanged += (_, p) => prefs.Save(p.Id);

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
