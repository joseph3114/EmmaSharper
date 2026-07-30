using System;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    public class MailingTrigger : MailingDetails
    {
        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("plaintext")]
        public string? Plaintext { get; set; }

        [JsonPropertyName("html_body")]
        public string? HtmlBody { get; set; }
    }
}
