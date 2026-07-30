using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>Change the status for an array of members</summary>
    public class ChangeStatus
    {
        /// <summary>
        /// The array of member ids to change.
        /// </summary>
        [JsonPropertyName("member_ids")]
        public List<long>? MemberIds { get; set; }

        /// <summary>
        /// The new status for the given members. Accepts one of ‘a’ (active), ‘e’ (error), ‘o’ (optout).
        /// </summary>
        [JsonPropertyName("status_to")]
        public MemberStatusShort StatusTo { get; set; }
    }
}