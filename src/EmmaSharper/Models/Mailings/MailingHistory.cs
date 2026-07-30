using System;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class MailingHistory
    {
        [JsonPropertyName("mailing_type")]
        public MailingType MailingType { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("clicked")]
        public DateTime? Clicked { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("opened")]
        public DateTime? Opened { get; set; }

        [JsonPropertyName("mailing_id")]
        public long? MailingId { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("delivery_ts")]
        public DateTime? DelieveryTimestamp { get; set; }

        [JsonPropertyName("delivery_type")]
        public DeliveryTypeShort DelieveryType { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("forwarded")]
        public DateTime? Forwarded { get; set; }

        [JsonPropertyName("parent_mailing_id")]
        public long? ParentMailingId { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("shared")]
        public DateTime? Shared { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("account_id")]
        public long? AccountId { get; set; }
    }
}
