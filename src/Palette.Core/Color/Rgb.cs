namespace ArcticGizmo.Avalonia.Palette.Color;

/// <summary>
/// A UI-framework-agnostic 8-bit-per-channel RGB colour.
/// Kept free of any Avalonia dependency so the colour maths and WCAG
/// utilities can be unit-tested and reused outside a running UI.
/// Conversions to <c>Avalonia.Media.Color</c> live in <c>RgbExtensions</c>
/// (in the Avalonia-facing package).
/// </summary>
public readonly record struct Rgb(byte R, byte G, byte B)
{
    /// <summary>Parse a <c>#RGB</c>, <c>#RRGGBB</c> or <c>#AARRGGBB</c> hex string (alpha ignored).</summary>
    public static Rgb FromHex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
            throw new ArgumentException("Hex colour string was empty.", nameof(hex));

        var s = hex.Trim();
        if (s[0] == '#') s = s[1..];

        switch (s.Length)
        {
            case 3: // #RGB
                return new Rgb(
                    (byte)(Convert.ToInt32(new string(s[0], 2), 16)),
                    (byte)(Convert.ToInt32(new string(s[1], 2), 16)),
                    (byte)(Convert.ToInt32(new string(s[2], 2), 16)));
            case 6: // #RRGGBB
                return new Rgb(
                    Convert.ToByte(s.Substring(0, 2), 16),
                    Convert.ToByte(s.Substring(2, 2), 16),
                    Convert.ToByte(s.Substring(4, 2), 16));
            case 8: // #AARRGGBB — drop the alpha, we only model opaque tokens
                return new Rgb(
                    Convert.ToByte(s.Substring(2, 2), 16),
                    Convert.ToByte(s.Substring(4, 2), 16),
                    Convert.ToByte(s.Substring(6, 2), 16));
            default:
                throw new FormatException($"'{hex}' is not a recognised hex colour.");
        }
    }

    /// <summary>Render as an uppercase <c>#RRGGBB</c> string.</summary>
    public string ToHex() => $"#{R:X2}{G:X2}{B:X2}";

    public override string ToString() => ToHex();

    /// <summary>
    /// Linearly blend this colour with <paramref name="other"/>.
    /// <paramref name="t"/> = 0 returns this colour, 1 returns <paramref name="other"/>.
    /// Used to synthesise hover/tint tokens from a small seed set.
    /// </summary>
    public Rgb MixWith(Rgb other, double t)
    {
        t = Math.Clamp(t, 0d, 1d);
        return new Rgb(
            Lerp(R, other.R, t),
            Lerp(G, other.G, t),
            Lerp(B, other.B, t));
    }

    /// <summary>
    /// Simulate painting a translucent layer of <paramref name="over"/> at opacity
    /// <paramref name="alpha"/> on top of this (opaque) colour, returning the resulting
    /// opaque colour. This is how the palette derives solid "tint" tokens (e.g. a 12%
    /// accent wash for an active nav row) without introducing real transparency.
    /// </summary>
    public Rgb OverlayedBy(Rgb over, double alpha) => MixWith(over, Math.Clamp(alpha, 0d, 1d));

    private static byte Lerp(byte a, byte b, double t) =>
        (byte)Math.Round(a + (b - a) * t, MidpointRounding.AwayFromZero);
}
