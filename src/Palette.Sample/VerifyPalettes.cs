using Palette.Theming;

namespace Palette.Sample;

/// <summary>
/// A headless check that every built-in palette meets WCAG AA on the readability-critical
/// pairings. Non-text UI borders (which only need 3:1) are excluded. Returns a process exit
/// code so it can gate CI: <c>dotnet run --project src/Palette.Sample -- --verify</c>.
/// </summary>
internal static class VerifyPalettes
{
    public static int Run()
    {
        var failures = 0;

        foreach (var p in PaletteCatalog.All)
        {
            var report = ContrastReport.For(p);
            Console.WriteLine($"=== {p.Id,-16} {p.Variant} ===");
            foreach (var c in report.Checks)
            {
                var isBorder = c.Label.Contains("border", StringComparison.OrdinalIgnoreCase);
                var below = c.Ratio < Palette.Theming.Color.Contrast.AaText;
                var mark = isBorder ? "  " : below ? "!!" : "ok";
                if (below && !isBorder) failures++;
                Console.WriteLine($"  {mark} {c.RatioText,-8} {c.Label}");
            }

            Console.WriteLine();
        }

        if (failures == 0)
        {
            Console.WriteLine("PASS — all text / syntax / status / diff pairs meet WCAG AA (>= 4.5:1).");
            return 0;
        }

        Console.WriteLine($"FAIL — {failures} pair(s) below WCAG AA.");
        return 1;
    }
}
