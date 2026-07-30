using System;
using System.Collections.Generic;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class Member
    {
        [JsonPropertyName("status")]
        public MemberStatus Status { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("confirmed_opt_in")]
        public DateTime? ConfirmedOptIn { get; set; }

        [JsonPropertyName("account_id")]
        public long? AccountId { get; set; }

        [JsonPropertyName("fields")]
        public Dictionary<string, object> Fields { get; set; }

        [JsonPropertyName("member_id")]
        public long? MemberId { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("last_modified_at")]
        public DateTime? LastModifiedAt { get; set; }

        // Updated to ..Short for Groups Methods.
        [JsonPropertyName("member_status_id")]
        public MemberStatusShort MemberStatusId { get; set; }

        [JsonPropertyName("plaintext_preferred")]
        public bool PlaintextPreferred { get; set; }

        [JsonPropertyName("email_error")]
        public string EmailError { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("member_since")]
        public DateTime MemberSince { get; set; }

        [JsonPropertyName("bounce_count")]
        public int? BounceCount { get; set; }

        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
