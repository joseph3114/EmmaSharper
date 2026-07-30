using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    public class SignupMember
    {
        /// <summary>
        /// Email address of the member to sign-up.
        /// </summary>
        [JsonPropertyName("email")]
        public string MemberEmail { get; set; }

        /// <summary>
        /// An array of group ids to associate sign-up with.
        /// </summary>
        [JsonPropertyName("group_ids")]
        public List<long> GroupIds { get; set; }

        /// <summary>
        /// Optional. Names and values of user-defined fields to update.
        /// </summary>
        [JsonPropertyName("fields")]
        public Dictionary<string, object> Fields { get; set; }

        /// <summary>
        /// Optional. Indicate that this member used a particular signup form. This is important if you have custom mailings for a particular signup form and so that signup-based triggers will be fired.
        /// </summary>
        [JsonPropertyName("signup_form_id")]
        public long? SignupFormId { get; set; }

        /// <summary>
        /// Optional. Override the confirmation message subject with your own copy.
        /// </summary>
        [JsonPropertyName("opt_in_subject")]
        public string OptInSubject { get; set; }

        /// <summary>
        /// Optional. Override the confirmation message body with your own copy. Must include the following tags: [rsvp_name], [rsvp_email], [opt_in_url], [opt_out_url].
        /// </summary>
        [JsonPropertyName("opt_in_message")]
        public string OptInMessage { get; set; }

        /// <summary>
        /// Optional. Fires related field change autoresponders when set to true.
        /// </summary>
        [JsonPropertyName("field_triggers")]
        public bool FieldTriggers { get; set; }

        /// <summary>
        /// Optional. Sends the default plaintext confirmation email when set to true. NOTE: Confirmation email will be sent by default if this parameter is left out.
        /// </summary>
        [JsonPropertyName("opt_in_confirmation")]
        public bool OptInConfirmation { get; set; }
    }
}
