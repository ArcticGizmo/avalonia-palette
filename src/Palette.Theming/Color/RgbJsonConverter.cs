using System.Text.Json;
using System.Text.Json.Serialization;

namespace Palette.Theming.Color;

/// <summary>Serialises <see cref="Rgb"/> as a compact <c>"#RRGGBB"</c> hex string.</summary>
public sealed class RgbJsonConverter : JsonConverter<Rgb>
{
    public override Rgb Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        Rgb.FromHex(reader.GetString() ?? throw new JsonException("Expected a hex colour string."));

    public override void Write(Utf8JsonWriter writer, Rgb value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToHex());
}
