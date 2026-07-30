using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>
    /// 
    /// </summary>
    public class Webhook : WebhookBase
    {
        /// <summary>
        /// The Id of the webhook
        /// </summary>
        [JsonPropertyName("webhook_id")]
        public long? WebhookId { get; set; }

        /// <summary>
        /// The ID associated with the webhook account
        /// </summary>
        [JsonPropertyName("account_id")]
        public long? AccountId { get; set; }
    }
}
