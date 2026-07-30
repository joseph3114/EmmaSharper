using System;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class Group
    {
        [JsonPropertyName("active_count")]
        public int? ActiveCount { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("error_count")]
        public int? ErrorCount { get; set; }

        [JsonPropertyName("optout_count")]
        public int? OptoutCount { get; set; }

        [JsonPropertyName("group_type")]
        public GroupType GroupType { get; set; }

        [JsonPropertyName("member_group_id")]
        public long? MemberGroupId { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("purged_at")]
        public DateTime? PurgedAt { get; set; }

        [JsonPropertyName("account_id")]
        public long? AccountId { get; set; }

        [JsonPropertyName("group_name")]
        public string GroupName { get; set; }
    }
}
