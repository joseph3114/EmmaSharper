using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmmaSharper.Internals
{
    /// <summary>Shared System.Text.Json configuration for every Emma request and response.</summary>
    internal static class EmmaJson
    {
        internal static readonly JsonSerializerOptions Options = Create();

        private static JsonSerializerOptions Create()
        {
            JsonSerializerOptions options = new()
            {
                // Every model carries an explicit [JsonPropertyName], so this is only a safety net
                // for anything added later without one. Emma is snake_case throughout.
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true,

                // Replaces the 27 per-property NullValueHandling.Ignore attributes that the
                // Newtonsoft models carried.
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

                // Emma is inconsistent about quoting numerics on some endpoints.
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };

            // Registered globally to match the previous EmmaJsonSerializer, which added the enum
            // converter to its converter collection - this covers enums that lack the attribute.
            //
            // EmmaDateJsonConverter is deliberately NOT registered globally: it is opt-in per
            // property via [JsonConverter], because only Emma's "@D:"-prefixed fields use that
            // format and applying it to every DateTime would corrupt the others.
            options.Converters.Add(new StringEnumJsonConverter());

            return options;
        }
    }
}
