using System.Text.Json;
using System.Text.Json.Serialization;
using Palette.Theming.Color;

namespace Palette.Theming;

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
}
