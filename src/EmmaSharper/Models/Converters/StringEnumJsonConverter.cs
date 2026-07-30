using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmmaSharper.Internals
{
    /// <summary>
    /// Serializes enums using their <see cref="EnumMemberAttribute"/> value, falling back to an
    /// "Unknown" member when Emma returns a value this library does not yet model.
    /// </summary>
    /// <remarks>
    /// <para>
    /// System.Text.Json's built-in JsonStringEnumConverter does NOT honour [EnumMember], and its
    /// replacement (JsonStringEnumMemberNameAttribute) is .NET 9+ only while this library still
    /// targets net8.0. The mapping is therefore done here so the wire format stays identical
    /// across both target frameworks and matches what Newtonsoft produced.
    /// </para>
    /// <para>
    /// The Unknown fallback preserves the behaviour added upstream in 2019 - Emma adds enum values
    /// without notice, and a hard failure on an unrecognised status made the client brittle.
    /// </para>
    /// </remarks>
    internal sealed class StringEnumJsonConverter : JsonConverterFactory
    {
        internal const string UnknownValue = "Unknown";

        public override bool CanConvert(Type typeToConvert)
            => (Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert).IsEnum;

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type? underlying = Nullable.GetUnderlyingType(typeToConvert);
            Type enumType = underlying ?? typeToConvert;

            Type converterType = underlying is null
                ? typeof(EnumConverter<>).MakeGenericType(enumType)
                : typeof(NullableEnumConverter<>).MakeGenericType(enumType);

            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

        /// <summary>
        /// Name/value maps per enum type. Static and concurrent - the previous implementation
        /// marked instance fields [ThreadStatic], which the runtime ignores on non-static fields,
        /// so the cache was silently per-instance rather than per-thread.
        /// </summary>
        private static class Maps
        {
            private static readonly ConcurrentDictionary<Type, EnumMap> Cache = new();

            internal static EnumMap For(Type enumType) => Cache.GetOrAdd(enumType, Build);

            private static EnumMap Build(Type enumType)
            {
                Dictionary<string, object> fromValue = new(StringComparer.OrdinalIgnoreCase);
                Dictionary<object, string> toValue = new();

                foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    object member = Enum.Parse(enumType, field.Name);
                    string? enumMember = field.GetCustomAttribute<EnumMemberAttribute>()?.Value;

                    if (!string.IsNullOrEmpty(enumMember))
                    {
                        fromValue[enumMember!] = member;
                        toValue[member] = enumMember!;
                    }
                    else
                    {
                        toValue[member] = field.Name;
                    }

                    // The CLR name is always accepted on read, even when an EnumMember value exists.
                    fromValue[field.Name] = member;
                }

                return new EnumMap(fromValue, toValue);
            }
        }

        private sealed class EnumMap
        {
            internal EnumMap(Dictionary<string, object> fromValue, Dictionary<object, string> toValue)
            {
                FromValue = fromValue;
                ToValue = toValue;
            }

            internal Dictionary<string, object> FromValue { get; }

            internal Dictionary<object, string> ToValue { get; }
        }

        /// <summary>Shared read/write logic for both the nullable and non-nullable converters.</summary>
        private static bool TryRead<TEnum>(ref Utf8JsonReader reader, out TEnum result)
            where TEnum : struct, Enum
        {
            EnumMap map = Maps.For(typeof(TEnum));

            if (reader.TokenType == JsonTokenType.String)
            {
                string? text = reader.GetString();
                if (text is not null && map.FromValue.TryGetValue(text, out object? match))
                {
                    result = (TEnum)match;
                    return true;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out long numeric))
            {
                // Enum.IsDefined rather than scanning Enum.GetValues: no array allocation per
                // call, and it drops the implicit-filter loop CodeQL flags as cs/linq/missed-where.
                TEnum candidate = (TEnum)Enum.ToObject(typeof(TEnum), numeric);
                if (Enum.IsDefined(candidate))
                {
                    result = candidate;
                    return true;
                }
            }

            result = default;
            return false;
        }

        private static void WriteValue<TEnum>(Utf8JsonWriter writer, TEnum value)
            where TEnum : struct, Enum
        {
            EnumMap map = Maps.For(typeof(TEnum));
            writer.WriteStringValue(map.ToValue.TryGetValue(value, out string? text) ? text : value.ToString());
        }

        private sealed class EnumConverter<TEnum> : JsonConverter<TEnum>
            where TEnum : struct, Enum
        {
            public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (TryRead(ref reader, out TEnum value))
                {
                    return value;
                }

                if (Enum.TryParse(UnknownValue, ignoreCase: true, out TEnum unknown))
                {
                    return unknown;
                }

                throw new JsonException(
                    $"Unable to map the received value to enum {typeof(TEnum)}. " +
                    $"Consider adding '{UnknownValue}' as a fall-back member.");
            }

            public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
                => WriteValue(writer, value);
        }

        private sealed class NullableEnumConverter<TEnum> : JsonConverter<TEnum?>
            where TEnum : struct, Enum
        {
            public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return null;
                }

                // Unlike the non-nullable case, an unrecognised value is simply absent.
                return TryRead(ref reader, out TEnum value) ? value : null;
            }

            public override void Write(Utf8JsonWriter writer, TEnum? value, JsonSerializerOptions options)
            {
                if (value is null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    WriteValue(writer, value.Value);
                }
            }
        }
    }
}
