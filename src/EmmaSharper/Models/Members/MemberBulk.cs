using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>
    /// Used to add new members or update existing members in bulk.
    /// </summary>
    public class MemberBulk
    {
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
    }
}
