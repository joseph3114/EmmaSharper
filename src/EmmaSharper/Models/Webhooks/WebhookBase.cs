using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>
    /// Common Properties to all Webhook classes.
    /// </summary>
    public class WebhookBase
    {
        /// <summary>
        /// The name of an event to register this webhook for
        /// </summary>
        [JsonPropertyName("event")]
        public string? Event { get; set; }

        /// <summary>
        /// The URL to call when the event happens
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        /// The method to use when calling the webhook. Can be GET or POST. Defaults to POST.
        /// </summary>
        [JsonPropertyName("method")]
        public WebhookMethod Method { get; set; }
    }
}
