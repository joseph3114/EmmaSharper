using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>Copy all account members of one or more statuses into a group</summary>
    public class CopyStatus
    {
        /// <summary>‘a’ (active), ‘o’ (optout), and/or ‘e’ (error)</summary>
        [JsonPropertyName("member_status_id")]
        public List<MemberStatusShort> MemberStatusId { get; set; }
    }
}
