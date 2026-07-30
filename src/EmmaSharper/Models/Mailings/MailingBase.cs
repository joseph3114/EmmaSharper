using System;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    public class MailingBase
    {
        [JsonPropertyName("mailing_type")]
        public MailingType MailingType { get; set; }

        [JsonPropertyName("mailing_id")]
        public long? MailingId { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("send_started")]
        public DateTime? SendStarted { get; set; }

        [JsonPropertyName("signup_form_id")]
        public long? SignupFormId { get; set; }

        [JsonPropertyName("recipient_count")]
        public int? RecipientCount { get; set; }

        [JsonPropertyName("parent_mailing_id")]
        public long? ParentMailingId { get; set; }

        [JsonPropertyName("account_id")]
        public long? AccountId { get; set; }

        [JsonPropertyName("mailing_status")]
        public MailingStatus MailingStatus { get; set; }

        [JsonPropertyName("sender")]
        public string? Sender { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("send_finished")]
        public DateTime? SendFinished { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("send_at")]
        public DateTime? SendAt { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("archived_ts")]
        public DateTime? ArchivedTimestamp { get; set; }
    }
}
