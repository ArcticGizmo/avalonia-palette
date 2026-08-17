using Avalonia;
using Palette.Theming;

namespace Palette.Sample;

internal static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        // Headless WCAG gate: `dotnet run -- --verify` prints a contrast report for every
        // palette and exits non-zero if any text/syntax/status/diff pair falls below AA.
        // Handy in CI so palette edits can't silently regress readability.
        if (args.Contains("--verify"))
            return VerifyPalettes.Run();

        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
