using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>
    /// Remove multiple members from groups.
    /// </summary>
    public class RemoveMemberGroups
    {
        /// <summary>
        /// Member ids to remove from the given groups.
        /// </summary>
        [JsonPropertyName("member_ids")]
        public List<long>? MemberIds { get; set; }

        /// <summary>
        /// Group ids from which to remove the given members.
        /// </summary>
        [JsonPropertyName("group_ids")]
        public List<long>? GroupIds { get; set; }
    }
}
