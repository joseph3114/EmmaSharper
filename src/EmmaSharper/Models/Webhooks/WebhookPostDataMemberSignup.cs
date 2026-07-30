using System;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class WebhookPostDataMemberSignup
    {
        [JsonPropertyName("signup_form_id")]
        public string SignupFormId { get; set; }

        [JsonPropertyName("account_id")]
        public string AccountId { get; set; }

        [JsonPropertyName("member_id")]
        public string MemberId { get; set; }

        [JsonPropertyName("mailing_id")]
        public long MailingId { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

    }
}
