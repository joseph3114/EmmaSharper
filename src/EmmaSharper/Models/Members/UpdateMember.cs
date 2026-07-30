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
        /// <remarks>
        /// Nullable so it is omitted when you are not changing the status. As a non-nullable enum
        /// it defaulted to <see cref="MemberStatusShort.Unknown"/> and was serialized on every
        /// request, sending <c>"status_to":"Unknown"</c> - not a status Emma accepts - to callers
        /// who only wanted to change an email address or a custom field.
        /// </remarks>
        [JsonPropertyName("status_to")]
        public MemberStatusShort? StatusTo { get; set; }

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
