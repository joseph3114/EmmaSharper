using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>
    /// Validate that a mailing has valid personalization-tag syntax.
    /// </summary>
    public class MailingPersonalization
    {
        /// <summary>
        /// The html contents of the mailing.
        /// </summary>
        [JsonPropertyName("html_body")]
        public string HtmlBody { get; set; }

        /// <summary>
        /// The plaintext contents of the mailing. Unlike in create_mailing, this param is not required.
        /// </summary>
        [JsonPropertyName("plaintext")]
        public string Plaintext { get; set; }

        /// <summary>
        /// The subject of the mailing.
        /// </summary>
        [JsonPropertyName("subject")]
        public string Subject { get; set; }
    }
}
