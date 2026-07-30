using System;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    public class MailingInfo : MailingDetails
    {
        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }
}
