using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <summary>A user with access to an Emma account.</summary>
    public class AccountUser
    {
        /// <summary>The user's Emma id.</summary>
        [JsonPropertyName("user_id")]
        public long? UserId { get; set; }

        /// <summary>The user's email address.</summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>Given name.</summary>
        [JsonPropertyName("user_name_first")]
        public string? FirstName { get; set; }

        /// <summary>Family name.</summary>
        [JsonPropertyName("user_name_last")]
        public string? LastName { get; set; }

        /// <summary>The user's role on this account.</summary>
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        /// <summary>When the user was created.</summary>
        /// <remarks>
        /// Typed as a date rather than a string on purpose. Consumers that keep this as text
        /// cannot sort a user list by it, which is the obvious thing to want.
        /// </remarks>
        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("create_ts")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>The user's most recent sign-in attempt.</summary>
        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("last_login_attempt")]
        public DateTime? LastLoginAttempt { get; set; }

        /// <summary>Any additional fields Emma returns for an account user.</summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalData { get; set; }
    }
}
