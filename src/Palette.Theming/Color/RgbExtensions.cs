using Avalonia.Media;

namespace Palette.Theming.Color;

/// <summary>Bridges the framework-agnostic <see cref="Rgb"/> to Avalonia's media types.</summary>
public static class RgbExtensions
{
    /// <summary>Opaque Avalonia <see cref="global::Avalonia.Media.Color"/>.</summary>
    public static global::Avalonia.Media.Color ToColor(this Rgb c) =>
        global::Avalonia.Media.Color.FromRgb(c.R, c.G, c.B);

    /// <summary>Avalonia colour with an explicit alpha byte (0–255).</summary>
    public static global::Avalonia.Media.Color ToColor(this Rgb c, byte alpha) =>
        global::Avalonia.Media.Color.FromArgb(alpha, c.R, c.G, c.B);

    /// <summary>A fresh opaque <see cref="SolidColorBrush"/>.</summary>
    public static SolidColorBrush ToBrush(this Rgb c) => new(c.ToColor());
}
