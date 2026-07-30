using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class WebhookEvents
    {
        [JsonPropertyName("event_name")]
        public string? EventName { get; set; }

        // int64 for consistency with every other Emma id - see #5.
        [JsonPropertyName("webhook_event_id")]
        public long? WebhookEventId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
