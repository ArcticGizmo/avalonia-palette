using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ArcticGizmo.Avalonia.Palette.Color;

namespace ArcticGizmo.Avalonia.Palette;

/// <summary>
/// Serialises a <see cref="PaletteDefinition"/> to/from JSON so custom palettes can be saved,
/// shared and imported. Colours are written as <c>#RRGGBB</c> hex; the variant as a string.
/// </summary>
public static class PaletteCodec
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new RgbJsonConverter(),
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    /// <summary>Serialise a single palette to indented JSON.</summary>
    public static string ToJson(PaletteDefinition palette) =>
        JsonSerializer.Serialize(palette, Options);

    /// <summary>Parse a single palette from JSON. Throws on malformed input.</summary>
    public static PaletteDefinition FromJson(string json) =>
        JsonSerializer.Deserialize<PaletteDefinition>(json, Options)
        ?? throw new JsonException("Palette JSON deserialised to null.");

    /// <summary>Serialise a list of palettes (used by the custom-palette store).</summary>
    public static string ToJsonList(IEnumerable<PaletteDefinition> palettes) =>
        JsonSerializer.Serialize(palettes, Options);

    /// <summary>Parse a list of palettes. Returns empty on null/empty input.</summary>
    public static IReadOnlyList<PaletteDefinition> FromJsonList(string json) =>
        string.IsNullOrWhiteSpace(json)
            ? Array.Empty<PaletteDefinition>()
            : JsonSerializer.Deserialize<List<PaletteDefinition>>(json, Options) ?? new();

    // ── Compact share codes ──────────────────────────────────────────────
    //
    // A single-line, copy/paste- and QR-friendly encoding of one palette: the JSON above,
    // gzip-compressed, base64url-encoded, and tagged with a version prefix so the format can
    // evolve. Round-trips through the same JSON schema, so anything ToJson can carry, a share
    // code carries too.

    private const string ShareCodePrefix = "pal1:";

    private static readonly JsonSerializerOptions CompactOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new RgbJsonConverter(),
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    /// <summary>
    /// Encode a palette as a compact, single-line share code (e.g. <c>pal1:H4sIAAAA…</c>) suitable
    /// for copy/paste or a QR code. Reverse with <see cref="FromShareCode"/>.
    /// </summary>
    public static string ToShareCode(PaletteDefinition palette)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(palette, CompactOptions);
        using var buffer = new MemoryStream();
        using (var gz = new GZipStream(buffer, CompressionLevel.Optimal, leaveOpen: true))
            gz.Write(json, 0, json.Length);
        return ShareCodePrefix + ToBase64Url(buffer.ToArray());
    }

    /// <summary>
    /// Decode a share code produced by <see cref="ToShareCode"/>. Throws
    /// <see cref="FormatException"/> if the code is missing its prefix or otherwise malformed.
    /// </summary>
    public static PaletteDefinition FromShareCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new FormatException("Share code was empty.");

        var s = code.Trim();
        if (!s.StartsWith(ShareCodePrefix, StringComparison.Ordinal))
            throw new FormatException($"Share code must start with '{ShareCodePrefix}'.");

        try
        {
            var compressed = FromBase64Url(s[ShareCodePrefix.Length..]);
            using var input = new MemoryStream(compressed);
            using var gz = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            gz.CopyTo(output);
            var json = Encoding.UTF8.GetString(output.ToArray());
            return FromJson(json);
        }
        catch (Exception ex) when (ex is not FormatException)
        {
            throw new FormatException("Share code was not a valid palette.", ex);
        }
    }

    private static string ToBase64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] FromBase64Url(string s)
    {
        var b64 = s.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(b64.PadRight((b64.Length + 3) / 4 * 4, '='));
    }
}
