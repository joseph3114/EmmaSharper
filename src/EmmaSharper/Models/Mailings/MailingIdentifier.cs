using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>
    /// Class including just the Mailing Identifier.
    /// </summary>
    public class MailingIdentifier
    {
        /// <summary>
        /// Mailing Identifier.
        /// </summary>
        [JsonPropertyName("mailing_id")]
        public long MailingId { get; set; }
    }
}
