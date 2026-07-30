using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class Link
    {
        [JsonPropertyName("link_order")]
        public int? LinkOrder { get; set; }

        [JsonPropertyName("Link_name")]
        public string LinkName { get; set; }

        [JsonPropertyName("unique_clicks")]
        public int? UniqueClicks { get; set; }

        [JsonPropertyName("plaintext")]
        public bool Plaintext { get; set; }

        [JsonPropertyName("link_target")]
        public string LinkTarget { get; set; }

        [JsonPropertyName("total_clicks")]
        public int? TotalClicks { get; set; }

        // int64: Emma link ids exceed Int32.MaxValue. Closes binarypatrick/EmmaSharper#5.
        [JsonPropertyName("link_id")]
        public long? LinkId { get; set; }
    }
}
