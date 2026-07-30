using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class MemberAdd
    {
        [JsonPropertyName("status")]
        public MemberStatusShort Status { get; set; }

        [JsonPropertyName("member_id")]
        public long? MemberId { get; set; }

        [JsonPropertyName("added")]
        public bool Added { get; set; }
    }
}
