using System.Text.Json.Serialization;

namespace EmmaSharper
{

    public class SubscriptionMembers
    {
        [JsonPropertyName("member_id")]
        public long MemberId { get; set; }
    }
}
