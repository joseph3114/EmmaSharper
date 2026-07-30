using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmmaSharper.Internals
{
    /// <summary>Handles Emma's prefixed date format, e.g. "@D:2014-11-26T11:40:55".</summary>
    /// <remarks>
    /// This is a factory rather than a plain converter because the attribute is applied to both
    /// <see cref="DateTime"/> and <see cref="Nullable{T}"/> properties. Newtonsoft's non-generic
    /// JsonConverter covered both from one class; System.Text.Json matches on the exact type, so a
    /// JsonConverter&lt;DateTime?&gt; alone would silently skip the five non-nullable sites.
    /// </remarks>
    internal sealed class EmmaDateJsonConverter : JsonConverterFactory
    {
        internal const string Prefix = "@D:";

        public override bool CanConvert(Type typeToConvert)
            => typeToConvert == typeof(DateTime) || typeToConvert == typeof(DateTime?);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            => typeToConvert == typeof(DateTime?)
                ? new NullableConverter()
                : new NonNullableConverter();

        /// <summary>Strips the "@D:" prefix and parses. Returns null when absent or unparseable.</summary>
        private static DateTime? ReadCore(ref Utf8JsonReader reader)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            string? raw = reader.TokenType == JsonTokenType.String ? reader.GetString() : null;
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            if (raw!.StartsWith(Prefix, StringComparison.Ordinal))
            {
                raw = raw.Substring(Prefix.Length);
            }

            // InvariantCulture is deliberate: this is a wire format, not user-facing text.
            // The Newtonsoft version used the ambient culture, which could vary by host.
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
