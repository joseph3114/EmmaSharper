using System;

namespace EmmaSharper
{
    /// <summary>Emma configuration options.</summary>
    public class EmmaOptions
    {
        private const string BASE_URL = "https://api.e2ma.net";

        /// <summary>Represents the default Emma API endpoint.</summary>
        /// <remarks><i>Default is <see href="https://api.e2ma.net"/></i></remarks>
        public string BaseUrl { get; set; } = BASE_URL;

        /// <summary>
        /// Default Emma account identifier.
        /// </summary>
        /// <remarks>
        /// Optional when every call supplies its own account id - see the account scoping
        /// overloads, which let one credential pair address many subaccounts.
        /// </remarks>
        public string? AccountId { get; set; }

        /// <summary>Emma public key.</summary>
        public string? PublicKey { get; set; }

        /// <summary>Emma private key.</summary>
        public string? SecretKey { get; set; }

        /// <summary>Per-request timeout.</summary>
        /// <remarks>
        /// Applied to the underlying <see cref="System.Net.Http.HttpClient"/>. Note that when a
        /// resilience handler is attached, its own per-attempt timeout may be the tighter of the
        /// two - the standard handler defaults to 10 seconds per attempt, which is not enough for
        /// a 500-record page.
        /// </remarks>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
