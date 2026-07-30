using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EmmaSharper.Internals
{
    /// <summary>
    /// Wrappers for the Emma endpoints that return an object around their payload.
    /// </summary>
    /// <remarks>
    /// Emma is inconsistent here, and assuming either shape breaks half the API. The
    /// <c>enterprise/*</c> and <c>accounts/*</c> endpoints wrap their arrays in an object, while
    /// the <c>members</c> endpoints return a bare array — and <c>members?count=true</c> returns a
    /// bare integer that is not even a JSON object. These types exist so the envelope is handled
    /// where it actually occurs, rather than by a generic unwrap that would break elsewhere.
    /// </remarks>
    internal sealed class SubaccountEnvelope
    {
        [JsonPropertyName("subaccounts")]
        public List<Subaccount>? Subaccounts { get; set; }
    }

    /// <inheritdoc cref="SubaccountEnvelope"/>
    internal sealed class AccountUserEnvelope
    {
        [JsonPropertyName("users")]
        public List<AccountUser>? Users { get; set; }
    }
}
