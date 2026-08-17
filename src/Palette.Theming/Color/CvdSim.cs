namespace Palette.Theming.Color;

/// <summary>A type of colour-vision deficiency to simulate.</summary>
public enum Cvd
{
    /// <summary>Normal vision — no transform.</summary>
    None,

    /// <summary>Red-blind (~1% of men).</summary>
    Protanopia,

    /// <summary>Green-blind (~1% of men) — the most common.</summary>
    Deuteranopia,

    /// <summary>Blue-blind (rare).</summary>
    Tritanopia
}

/// <summary>
/// Approximate colour-blindness simulation, so a palette can be checked for how it reads to users
/// with colour-vision deficiencies. Uses the widely-used HCIRN sRGB simulation matrices — good
/// enough for a design-time preview (not a clinical model).
/// </summary>
public static class CvdSim
{
    // Row-major 3x3 matrices applied to sRGB.
    private static readonly double[] Protan =
    {
        0.567, 0.433, 0.000,
        0.558, 0.442, 0.000,
        0.000, 0.242, 0.758
    };

    private static readonly double[] Deutan =
    {
        0.625, 0.375, 0.000,
        0.700, 0.300, 0.000,
        0.000, 0.300, 0.700
    };

    private static readonly double[] Tritan =
    {
        0.950, 0.050, 0.000,
        0.000, 0.433, 0.567,
        0.000, 0.475, 0.525
    };

    /// <summary>Simulate how <paramref name="c"/> appears under the given deficiency.</summary>
    public static Rgb Simulate(Rgb c, Cvd type)
    {
        if (type == Cvd.None) return c;
        var m = type switch
        {
            Cvd.Protanopia => Protan,
            Cvd.Deuteranopia => Deutan,
            _ => Tritan
        };

        double r = c.R, g = c.G, b = c.B;
        return new Rgb(
            Clamp(m[0] * r + m[1] * g + m[2] * b),
            Clamp(m[3] * r + m[4] * g + m[5] * b),
            Clamp(m[6] * r + m[7] * g + m[8] * b));
    }

    private static byte Clamp(double v) => (byte)Math.Clamp(Math.Round(v), 0, 255);
}
