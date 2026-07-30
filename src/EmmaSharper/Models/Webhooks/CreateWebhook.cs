using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>
    /// Properties associated with creating webhooks
    /// </summary>
    public class CreateWebhook : WebhookBase
    {
        /// <summary>
        /// The public_key to use for authentication. Note: this can also be spelled “user_id” but this is deprecated.
        /// </summary>
        [JsonPropertyName("public_key")]
        public string PublicKey { get; set; }
    }
}
