using System;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class MailingDetails : MailingBase
    {

        [JsonPropertyName("cancel_by_user_id")]
        public long? CancelByUserId { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("cancel_ts")]
        public DateTime? CancelTimestamp { get; set; }

        [JsonPropertyName("month")]
        public int? Month { get; set; }

        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("failure_ts")]
        public DateTime? FailureTimestamp { get; set; }

        [JsonPropertyName("reply_to")]
        public string? ReplyTo { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("start_or_finished")]
        public DateTime? StartedOrFinished { get; set; }

        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("created_ts")]
        public DateTime? CreatedTimestamp { get; set; }

        [JsonPropertyName("plaintext_only")]
        public bool PlaintextOnly { get; set; }

        [JsonPropertyName("failure_message")]
        public string? FailureMessage { get; set; }

        [JsonPropertyName("datacenter")]
        public string? Datacenter { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("purged_at")]
        public DateTime? PurgedAt { get; set; }
    }
}
