using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    public class ResponseSharesBase
    {
        [JsonPropertyName("network")]
        public string? Network { get; set; }

        [JsonPropertyName("share_clicks")]
        public int? ShareClicks { get; set; }
    }
    public class ResponseShares : ResponseSharesBase
    {
        [JsonPropertyName("fields")]
        public Dictionary<string, object>? Fields { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }

        [JsonPropertyName("member_id")]
        public long? MemberId { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("member_since")]
        public DateTime? MemberSince { get; set; }

        [JsonPropertyName("email_domain")]
        public string? EmailDomain { get; set; }

        [JsonPropertyName("email_user")]
        public string? EmailUser { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("member_status_id")]
        public MemberStatusShort MemberStatusId { get; set; }
    }

    public class ResponseSharesOverview : ResponseSharesBase
    {
        [JsonPropertyName("share_count")]
        public int? ShareCount { get; set; }
    }
}
