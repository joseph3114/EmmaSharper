using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmmaSharper.Internals
{
    /// <summary>
    /// Reads an id that Emma sends as either a JSON number or a JSON string into a <c>string</c>.
    /// </summary>
    /// <remarks>
    /// Emma is not consistent about quoting ids: <c>enterprise/subaccounts</c> sends
    /// <c>account_id</c> as a number, other endpoints quote theirs, and <c>accounts/users</c> sends a
    /// <c>user_id</c> that is not numeric at all. <see cref="JsonNumberHandling.AllowReadingFromString"/>
    /// only covers the opposite direction (a quoted number into a numeric property), so a
    /// <c>string</c> property meeting a number token would otherwise throw.
    /// <para>
    /// A number is kept exactly as written (<c>1001</c> becomes <c>"1001"</c>), so an id never loses
    /// digits or gains a decimal point. Writing always emits a JSON string.
    /// </para>
    /// </remarks>
    internal sealed class EmmaIdStringJsonConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;

                case JsonTokenType.String:
                    return reader.GetString();

                case JsonTokenType.Number:
                    // The raw token text, not a parsed value: an id is an identifier, not a quantity.
                    using (JsonDocument document = JsonDocument.ParseValue(ref reader))
                    {
                        return document.RootElement.GetRawText();
                    }

                default:
                    throw new JsonException(
                        $"Expected a string or number for an Emma id, but found {reader.TokenType}.");
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            => writer.WriteStringValue(value);
    }
}
