using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class SubscriptionSettings
    {
        [JsonPropertyName("show_on_default_preference_form")]
        public bool ShowOnDefaultPreferenceForm { get; set; }
    }
}
