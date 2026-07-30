using System.Text.Json.Serialization;

namespace EmmaSharper
{

    public class SubscriptionNew
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
