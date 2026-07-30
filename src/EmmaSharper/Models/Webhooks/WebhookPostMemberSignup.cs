using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class WebhookPostMemberSignup
    {
        [JsonPropertyName("event_name")]
        public string EventName { get; set; }

        [JsonPropertyName("resource_url")]
        public string ResourceUrl { get; set; }

        [JsonPropertyName("data")]
        public WebhookPostDataMemberSignup Data { get; set; }
    }
}
