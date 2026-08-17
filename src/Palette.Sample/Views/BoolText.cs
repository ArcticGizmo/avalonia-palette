using System.Globalization;
using Avalonia.Data.Converters;

namespace Palette.Sample.Views;

/// <summary>Small value converters used by the shell.</summary>
public static class BoolText
{
    /// <summary>true → "WCAG AA ✓", false → "Contrast: check".</summary>
    public static readonly IValueConverter AaPass =
        new FuncValueConverter<bool, string>(ok => ok ? "WCAG AA ✓" : "Contrast: check");
}
