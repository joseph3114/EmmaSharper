using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>Parameters to add a batch members to an audience</summary>
    public class AddMembers
    {
        /// <inheritdoc cref="object.Object"/>
        public AddMembers()
        {
            Members = new List<MemberBulk>();
            GroupIds = new List<long>();
        }

        /// <summary>
        /// Email address of member to add or update
        /// </summary>
        [JsonPropertyName("members")]
        public List<MemberBulk> Members { get; set; }

        /// <summary>
        /// Names and values of user-defined fields to update
        /// </summary>
        [JsonPropertyName("source_filename")]
        public string? SourceFileName { get; set; }

        /// <summary>
        /// Optional. Add imported members to this list of groups.
        /// </summary>
        [JsonPropertyName("group_ids")]
        public List<long> GroupIds { get; set; }

        /// <summary>
        /// Optional. Fires related field change auto-responders when set to true.
        /// </summary>
        [JsonPropertyName("automate_field_changes")]
        public bool? AutomateFieldChanges { get; set; }

        /// <summary>
        /// Optional. Only add new members, ignore existing members.
        /// </summary>        
        [JsonPropertyName("add_only")]
        public bool? AddOnly { get; set; }
    }
}
