using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class MemberStatusShortList
    {
        [JsonPropertyName("member_status_id")]
        public List<MemberStatusShort>? MemberStatusId { get; set; }
    }
}
