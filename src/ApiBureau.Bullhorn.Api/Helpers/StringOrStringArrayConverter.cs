using System.Text.Json;

namespace ApiBureau.Bullhorn.Api.Helpers;

/// <summary>
/// Reads a string or string array as text while preserving scalar string serialization.
/// </summary>
public sealed class StringOrStringArrayConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String) return reader.GetString();
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected a string or an array of strings.");

        var values = new List<string>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray) return string.Join(", ", values);
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Expected a string in the array.");

            values.Add(reader.GetString()!);
        }

        throw new JsonException("The string array is incomplete.");
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        => writer.WriteStringValue(value);
}