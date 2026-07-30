using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>
    /// Update a single member’s information.
    /// </summary>
    public class UpdateMember
    {
        /// <summary>
        /// A new email address for the member.
        /// </summary>
        [JsonPropertyName("email")]
        public string? MemberEmail { get; set; }

        /// <summary>
        /// A new status for the member. Accepts one of ‘a’ (active), ‘e’ (error), ‘o’ (opt-out).
        /// </summary>
        [JsonPropertyName("status_to")]
        public MemberStatusShort StatusTo { get; set; }

        /// <summary>
        /// An array of fields with associated values for this member
        /// </summary>
        [JsonPropertyName("fields")]
        public Dictionary<string, object>? Fields { get; set; }

        /// <summary>
        /// Optional. Fires related field change autoresponders when set to true.
        /// </summary>
        [JsonPropertyName("field_triggers")]
        public bool FieldTriggers { get; set; }
    }
}
