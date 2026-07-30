using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class WorkflowCount
    {
        [JsonPropertyName("account_id")]
        public long? AccountId { get; set; }

        [JsonPropertyName("draft")]
        public int Draft { get; set; }

        [JsonPropertyName("active")]
        public int Active { get; set; }

        [JsonPropertyName("inactive")]
        public int Inactive { get; set; }
    }
}

