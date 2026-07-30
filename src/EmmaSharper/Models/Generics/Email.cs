using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class Email
    {
        [JsonPropertyName("email")]
        public string Value { get; set; }
    }
}
