using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class UpdateMailing : MailingInfo
    {
        [JsonPropertyName("plaintext")]
        public string? Plaintext { get; set; }

        [JsonPropertyName("html_body")]
        public string? HtmlBody { get; set; }
    }
}
