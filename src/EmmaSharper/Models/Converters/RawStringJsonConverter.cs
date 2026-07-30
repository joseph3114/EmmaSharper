using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmmaSharper.Internals
{
    /// <summary>
    /// Passes a string through as raw JSON rather than as a quoted string literal.
    /// </summary>
    /// <remarks>
    /// Used for <c>CreateSearch.Criteria</c>, where Emma expects a JSON array expression that the
    /// caller has already composed. Escaping it as a string would send the wrong body.
    /// </remarks>
    internal sealed class RawStringJsonConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            // Anything structural (the criteria expression itself) round-trips as raw JSON text.
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return document.RootElement.GetRawText();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteRawValue(value);
        }
    }
}
