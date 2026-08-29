using System.Text.Json;
using System.Text.Json.Serialization;

namespace VictoryCenter.BLL.Helpers;

public sealed class TrimStringJsonHelper : JsonConverter<string>
{
    public override bool HandleNull => true;

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return string.Empty;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException(
                $"Invalid token type. Expected String or Null, received: {reader.TokenType}");
        }

        return reader.GetString()!.Trim();
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        => writer.WriteStringValue(value);
}
