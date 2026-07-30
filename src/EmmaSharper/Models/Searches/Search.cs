using System;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    public class Search
    {
        [JsonPropertyName("search_id")]
        public long? SearchId { get; set; }

        [JsonPropertyName("optout_count")]
        public int? OptoutCount { get; set; }

        [JsonPropertyName("error_count")]
        public int? ErrorCount { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("criteria")]
        public string? Criteria { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("purged_at")]
        public DateTime? PurgedAt { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("last_run_at")]
        public DateTime? LastRunAt { get; set; }

        [JsonPropertyName("active_count")]
        public int? ActiveCount { get; set; }

        [JsonPropertyName("account_id")]
        public long? AccountId { get; set; }
    }
}
