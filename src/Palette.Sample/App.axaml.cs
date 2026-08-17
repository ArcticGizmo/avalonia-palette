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
        // Register the themed brush set and apply the starting palette before any view is
        // built, so {StaticResource}/{DynamicResource} lookups resolve on first paint.
        ThemeManager.Initialize(this, PaletteCatalog.Default);

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
