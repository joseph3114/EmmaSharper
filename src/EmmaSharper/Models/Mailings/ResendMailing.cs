using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>
    /// Send a prior mailing to additional recipients. A new mailing will be created that inherits its content from the original.
    /// </summary>
    public class ResendMailing
    {
        /// <summary>
        /// An array of email addresses to which the new mailing should be sent.
        /// </summary>
        [JsonPropertyName("recipient_emails")]
        public List<string> RecipientEmails { get; set; }

        /// <summary>
        /// A list of email addresses that heads up notification emails will be sent to.
        /// </summary>
        [JsonPropertyName("heads_up_emails")]
        public List<string> HeadsUpEmails { get; set; }

        /// <summary>
        /// An array of member groups to which the new mailing should be sent.
        /// </summary>
        [JsonPropertyName("recipient_groups")]
        public List<string> RecipientGroups { get; set; }

        /// <summary>
        /// A list of searches that this mailing should be sent to.
        /// </summary>
        [JsonPropertyName("recipient_searches")]
        public List<string> RecipientSearches { get; set; }

        /// <summary>
        /// The message sender. If this is not supplied, the sender of the original mailing will be used.
        /// </summary>
        [JsonPropertyName("sender")]
        public string Sender { get; set; }
    }
}
