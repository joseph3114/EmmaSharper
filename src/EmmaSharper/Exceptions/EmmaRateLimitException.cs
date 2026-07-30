using System;
using System.Net;
using System.Net.Http;

namespace EmmaSharper
{
    /// <summary>Thrown when Emma throttles a request.</summary>
    /// <remarks>
    /// <para>
    /// Emma signals throttling with <b>403 Forbidden</b> as well as the conventional 429. That is
    /// the single most surprising behaviour in the API and the easiest thing for a consumer to get
    /// wrong - a naive client treats the 403 as an auth failure and gives up instead of backing off.
    /// Classifying it here means every consumer gets it right by default.
    /// </para>
    /// <para>
    /// Callers that want automatic retries should attach a resilience handler rather than catching
    /// this - see <c>EmmaRetryDefaults.ShouldHandle</c>.
    /// </para>
    /// </remarks>
    public sealed class EmmaRateLimitException : EmmaException
    {
        /// <summary>Creates an exception describing a throttled Emma API call.</summary>
        /// <param name="statusCode">The status Emma returned - 429, or 403 used as a throttle.</param>
        /// <param name="retryAfter">The Retry-After hint, when Emma supplied one.</param>
        /// <param name="responseBody">The raw response body, if one was read.</param>
        /// <param name="method">The HTTP verb used.</param>
        /// <param name="resource">The resolved request path.</param>
        public EmmaRateLimitException(
            HttpStatusCode statusCode,
            TimeSpan? retryAfter = null,
            string? responseBody = null,
            HttpMethod? method = null,
            string? resource = null)
            : base(statusCode, responseBody, method, resource)
        {
            RetryAfter = retryAfter;
        }

        /// <summary>How long Emma asked the caller to wait, when it said so.</summary>
        public TimeSpan? RetryAfter { get; }
    }
}
