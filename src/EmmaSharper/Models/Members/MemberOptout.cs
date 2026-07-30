using System;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    public class MemberOptout
    {
        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("mailing_id")]
        public long? MailingId { get; set; }
    }
}
