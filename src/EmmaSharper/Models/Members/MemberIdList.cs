using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class MemberIdList
    {
        [JsonPropertyName("member_ids")]
        public List<long>? MemberIds { get; set; }
    }
}
