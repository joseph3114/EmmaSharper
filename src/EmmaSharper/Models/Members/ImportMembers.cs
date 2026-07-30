using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class ImportMembers
    {
        [JsonPropertyName("member_id")]
        public long? MemberId { get; set; }

        [JsonPropertyName("change_type")]
        public ImportChangeType ChangeType { get; set; }

        [JsonPropertyName("member_status_id")]
        public MemberStatusShort MemberStatusId { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
