using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BulkBatcher.Models
{
    /// <summary>
    /// Converts empty or whitespace-only strings to null during deserialization
    /// so they are omitted when using JsonIgnoreCondition.WhenWritingNull.
    /// </summary>
    public class EmptyStringToNullConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
