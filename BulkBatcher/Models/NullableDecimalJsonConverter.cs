using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BulkBatcher.Models
{
    /// <summary>
    /// Deserializes <see cref="decimal?"/> from JSON numbers, null, or quoted decimals (e.g. CSV → dynamic → JSON).
    /// </summary>
    public sealed class NullableDecimalJsonConverter : JsonConverter<decimal?>
    {
        private const NumberStyles DecimalStyles =
            NumberStyles.Number | NumberStyles.AllowCurrencySymbol;

        // InvariantCulture's currency symbol is "\u00A4", so clone its number format
        // and use "$" instead to accept values like "$1,234.56" from spreadsheets.
        private static readonly NumberFormatInfo NumberFormat = CreateNumberFormat();

        private static NumberFormatInfo CreateNumberFormat()
        {
            var format = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            format.CurrencySymbol = "$";
            return format;
        }

        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.Number:
                    if (reader.TryGetDecimal(out var d))
                        return d;
                    throw new JsonException($"JSON number is outside the range of Decimal at path.");
                case JsonTokenType.String:
                    var s = reader.GetString();
                    if (string.IsNullOrWhiteSpace(s))
                        return null;
                    if (decimal.TryParse(s, DecimalStyles, NumberFormat, out var parsed))
                        return parsed;
                    throw new JsonException($"Unable to convert \"{s}\" to Decimal.");
                default:
                    throw new JsonException($"Unexpected token {reader.TokenType} when deserializing Decimal.");
            }
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue(value.Value);
            else
                writer.WriteNullValue();
        }
    }
}
