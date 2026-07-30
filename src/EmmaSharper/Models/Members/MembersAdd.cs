using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class MembersAdd
    {
        [JsonPropertyName("import_id")]
        public long ImportId { get; set; }
    }
}
