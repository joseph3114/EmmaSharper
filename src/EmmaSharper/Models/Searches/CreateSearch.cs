using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class CreateSearch
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("criteria")]
        [JsonConverter(typeof(RawStringJsonConverter))]
        public string Criteria { get; set; }
    }
}
