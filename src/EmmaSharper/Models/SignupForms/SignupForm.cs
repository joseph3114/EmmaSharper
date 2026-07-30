using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class SignupForm
    {
        // int64 for consistency with every other Emma id - see #5.
        [JsonPropertyName("id")]
        public long? SignupFormId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
