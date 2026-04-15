using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BulkBatcher.Models
{
    /// <summary>
    /// Deserializes <see cref="int?"/> from JSON numbers, null, or quoted integers (e.g. CSV → dynamic → JSON).
    /// </summary>
    public sealed class NullableInt32JsonConverter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.Number:
                    if (reader.TryGetInt32(out var i))
                        return i;
                    throw new JsonException($"JSON number is outside the range of Int32 at path.");
                case JsonTokenType.String:
                    var s = reader.GetString();
                    if (string.IsNullOrWhiteSpace(s))
                        return null;
                    if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                        return parsed;
                    throw new JsonException($"Unable to convert \"{s}\" to Int32.");
                default:
                    throw new JsonException($"Unexpected token {reader.TokenType} when deserializing Int32.");
            }
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue(value.Value);
            else
                writer.WriteNullValue();
        }
    }
}
