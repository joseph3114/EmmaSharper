using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class GroupName
    {
        [JsonPropertyName("group_name")]
        public string Name { get; set; }
    }
}
