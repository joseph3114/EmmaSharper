using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>Parameters to add a single member to an audience. Group Ids and Field Triggers are optional</summary>
    public class AddMember
    {
        /// <inheritdoc cref="object.Object"/>
        public AddMember()
        {
            GroupIds = new List<long>();
        }

        /// <summary>
        /// Email address of member to add or update
        /// </summary>
        [JsonPropertyName("email")]
        public string MemberEmail { get; set; }

        /// <summary>
        /// Names and values of user-defined fields to update
        /// </summary>
        [JsonPropertyName("fields")]
        public Dictionary<string, object> Fields { get; set; }

        /// <summary>
        /// Optional. Add imported members to this list of groups.
        /// </summary>
        [JsonPropertyName("group_ids")]
        public List<long> GroupIds { get; set; }

        /// <summary>
        /// Optional. Fires related field change auto-responders when set to true.
        /// </summary>
        [JsonPropertyName("field_triggers")]
        public bool? FieldTriggers { get; set; }
    }
}
