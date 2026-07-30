using System.Text.Json.Serialization;

namespace EmmaSharper
{
    /// <summary>
    /// The class representing the returned properties when signing up a member.
    /// </summary>
    public class MemberSignup
    {
        /// <summary>
        /// The status of the member. The short status code will be returned as Active, Error, or Optout.
        /// </summary>
        [JsonPropertyName("status")]
        public MemberStatusShort Status { get; set; }

        /// <summary>
        /// The member id of the member.
        /// </summary>
        [JsonPropertyName("member_id")]
        public long? MemberId { get; set; }
    }
}
