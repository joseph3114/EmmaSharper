using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class DeleteMembers
    {
        /// <summary>
        /// An array of member ids to delete.
        /// </summary>
        [JsonPropertyName("member_ids")]
        public List<long> MemberIds { get; set; }
    }
}
