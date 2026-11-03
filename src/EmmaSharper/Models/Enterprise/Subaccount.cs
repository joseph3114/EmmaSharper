using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <summary>A subaccount belonging to an Emma enterprise account.</summary>
    public class Subaccount
    {
        /// <summary>The subaccount's Emma account id. Use this to scope calls to it.</summary>
        /// <remarks>Emma sends this as a JSON number; it is kept as a string because that is what
        /// <see cref="IEmmaAccountScopeFactory.ForAccount(string)"/> takes.</remarks>
        /// <seealso cref="IEmmaAccountScopeFactory.ForAccount(string)"/>
        [JsonConverter(typeof(EmmaIdStringJsonConverter))]
        [JsonPropertyName("account_id")]
        public string? AccountId { get; set; }

        /// <summary>The subaccount's display name.</summary>
        [JsonPropertyName("account_name")]
        public string? AccountName { get; set; }

        /// <summary>Lifecycle status, e.g. <c>active</c>, <c>trial</c>, <c>retired</c>.</summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Any additional fields Emma returns for a subaccount.
        /// </summary>
        /// <remarks>
        /// Emma does not publish a complete schema for this endpoint, and the payload appears to
        /// vary by plan. Rather than guess at property names and silently drop the rest, anything
        /// unmapped is captured here — so plan, quota or billing fields, which are directly useful
        /// to a quota tool, are available even before they are modelled properly.
        /// </remarks>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalData { get; set; }
    }
}
