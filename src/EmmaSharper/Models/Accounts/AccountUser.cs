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
        /// <summary>The user's Emma id - a login name, not a number.</summary>
        /// <remarks>
        /// A string since 9.0.0. Emma sends a sign-in name here (shaped like an email address,
        /// though not necessarily the same as <see cref="Email"/>), so the 8.x <c>long?</c> threw
        /// on every real response. A numeric id is still accepted and kept as its digits.
        /// </remarks>
        [JsonConverter(typeof(EmmaIdStringJsonConverter))]
        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

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
        /// cannot sort a user list by it, which is the obvious thing to want. Emma sends a display
        /// date here ("Sep 25, 2024"), which has no time part, so the time is midnight.
        /// </remarks>
        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("create_ts")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>The user's most recent sign-in attempt.</summary>
        [JsonConverter(typeof(EmmaDateJsonConverter))]
        [JsonPropertyName("last_login_attempt")]
        public DateTime? LastLoginAttempt { get; set; }

        /// <summary>Any additional fields Emma returns for an account user.</summary>
        /// <remarks>
        /// Includes the <c>accounts</c> and <c>user_group_list</c> arrays. Their element shape is
        /// not documented and has not been confirmed against a live response, so they are left
        /// here rather than modelled with a guessed type that could break the whole list.
        /// </remarks>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalData { get; set; }
    }
}
