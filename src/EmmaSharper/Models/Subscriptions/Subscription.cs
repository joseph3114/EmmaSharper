using System;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class Subscription
    {
        [JsonPropertyName("account_id")]
        public long? AccountId { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("import_status")]
        public string? ImportStatus { get; set; }

        [JsonPropertyName("member_count")]
        public int? MemberCount { get; set; }

        [JsonPropertyName("modified_at")]
        public string? ModifiedAt { get; set; }

        [JsonPropertyName("optout_count")]
        public int? OptoutCount { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("purged_at")]
        public DateTime? PurgedAt { get; set; }

        [JsonPropertyName("settings")]
        public SubscriptionSettings? Settings { get; set; }

        [JsonPropertyName("subscription_id")]
        public long? SubscriptionId { get; set; }

        [JsonPropertyName("subscription_name")]
        public string? SubscriptionName { get; set; }

        [JsonPropertyName("subscription_order")]
        public int? SubscriptionOrder { get; set; }
    }
}
