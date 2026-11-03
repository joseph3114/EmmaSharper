using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmmaSharper.Internals
{
    /// <summary>
    /// Handles the date formats Emma writes: its prefixed form ("@D:2014-11-26T11:40:55"), plain
    /// ISO 8601, and the display form some endpoints use ("Sep 25, 2024").
    /// </summary>
    /// <remarks>
    /// This is a factory rather than a plain converter because the attribute is applied to both
    /// <see cref="DateTime"/> and <see cref="Nullable{T}"/> properties. Newtonsoft's non-generic
    /// JsonConverter covered both from one class; System.Text.Json matches on the exact type, so a
    /// JsonConverter&lt;DateTime?&gt; alone would silently skip the five non-nullable sites.
    /// <para>
    /// Reading never throws on content: a value in none of these formats, or a token that is not a
    /// string at all, reads as null (or <c>default</c> for a non-nullable property). A timestamp is
    /// never worth failing a whole list response over.
    /// </para>
    /// </remarks>
    internal sealed class EmmaDateJsonConverter : JsonConverterFactory
    {
        internal const string Prefix = "@D:";

        /// <summary>
        /// The display form <c>accounts/users</c> sends for <c>create_ts</c> and
        /// <c>last_login_attempt</c>. Matched exactly before the general parse, so the result does
        /// not depend on how lenient <see cref="DateTime.TryParse(string, IFormatProvider, DateTimeStyles, out DateTime)"/>
        /// happens to be on a given runtime. "d" accepts one or two digits.
        /// </summary>
        private static readonly string[] DisplayFormats = { "MMM d, yyyy", "MMMM d, yyyy" };

        public override bool CanConvert(Type typeToConvert)
            => typeToConvert == typeof(DateTime) || typeToConvert == typeof(DateTime?);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            => typeToConvert == typeof(DateTime?)
                ? new NullableConverter()
                : new NonNullableConverter();

        /// <summary>Strips the "@D:" prefix and parses. Returns null when absent or unparseable.</summary>
        private static DateTime? ReadCore(ref Utf8JsonReader reader)
        {
            if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
            {
                // Consume the whole value; returning without it leaves the reader mid-structure
                // and System.Text.Json throws.
                reader.Skip();
                return null;
            }

            string? raw = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            raw = raw!.Trim();
            if (raw.StartsWith(Prefix, StringComparison.Ordinal))
            {
                raw = raw.Substring(Prefix.Length);
            }

            // InvariantCulture is deliberate: this is a wire format, not user-facing text.
            // The Newtonsoft version used the ambient culture, which could vary by host.
            if (DateTime.TryParseExact(raw, DisplayFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime display))
            {
                return display;
            }

            return DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed)
                ? parsed
                : null;
        }

        private static void WriteCore(Utf8JsonWriter writer, DateTime value)
            => writer.WriteStringValue(string.Concat(Prefix, value.ToString("s", CultureInfo.InvariantCulture)));

        private sealed class NullableConverter : JsonConverter<DateTime?>
        {
            public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                => ReadCore(ref reader);

            public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
            {
                if (value is null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    WriteCore(writer, value.Value);
                }
            }
        }

        private sealed class NonNullableConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                => ReadCore(ref reader) ?? default;

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
                => WriteCore(writer, value);
        }
    }
}
