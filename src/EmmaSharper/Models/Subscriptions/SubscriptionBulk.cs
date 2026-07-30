using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class SubscriptionBulk
    {
        [JsonPropertyName("member_ids")]
        public List<long>? MemberIds { get; set; }

    }
}
