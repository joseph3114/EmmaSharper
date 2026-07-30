using System;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class Workflow
    {
        [JsonPropertyName("workflow_id")]
        public string WorkflowId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("status")]
        public WorkflowStatus Status { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}

