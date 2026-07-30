using System;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class Field : BaseField
    {
        [JsonPropertyName("shortcut_name")]
        public string? ShortcutName { get; set; }

        [JsonPropertyName("account_id")]
        public long? AccountId { get; set; }

        [JsonPropertyName("required")]
        public bool Required { get; set; }

        [JsonPropertyName("field_id")]
        public long? FieldId { get; set; }

        [JsonPropertyName("short_display_name")]
        public string? ShortDisplayName { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("options")]
        public string[]? Options { get; set; }
    }
}
