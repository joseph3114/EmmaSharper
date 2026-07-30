using System;
using System.Net;
using System.Net.Http;

namespace EmmaSharper
{
    /// <summary>
    /// Classifies Emma responses as retryable, so consumers do not each have to rediscover how
    /// Emma signals throttling.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This library deliberately does <b>not</b> own a retry policy. Retry belongs to the
    /// consumer's resilience pipeline, which knows the surrounding budget. What the wrapper can
    /// usefully own is the <i>classification</i> — and Emma's is genuinely surprising.
    /// </para>
    /// <para>
    /// These are plain predicates over <see cref="HttpResponseMessage"/> and
    /// <see cref="Exception"/> rather than Polly types, so the library keeps working on
    /// netstandard2.0. <c>Microsoft.Extensions.Http.Resilience</c> requires net8.0 or later; if
    /// the library took a dependency on it, .NET Framework consumers would be shut out.
    /// </para>
    /// <example>
    /// Wiring it into the standard resilience handler:
    /// <code>
    /// services.AddEmmaApiProviders(configuration)
    ///         .AddStandardResilienceHandler(options =>
    ///         {
    ///             options.Retry.ShouldHandle = args =>
    ///                 ValueTask.FromResult(EmmaRetryDefaults.IsTransient(args.Outcome.Result)
    ///                                   || EmmaRetryDefaults.IsTransient(args.Outcome.Exception));
    ///
    ///             // The default 10s per attempt is not enough for a 500-record member page.
    ///             options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
    ///             options.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(2);
    ///         });
    /// </code>
    /// </example>
    /// </remarks>
    public static class EmmaRetryDefaults
    {
        /// <summary>429, which does not exist as a named value on netstandard2.0.</summary>
        private const HttpStatusCode TooManyRequests = (HttpStatusCode)429;

        /// <summary>
        /// Whether a status code should be retried.
        /// </summary>
        /// <param name="statusCode">The status Emma returned.</param>
        /// <param name="treatForbiddenAsThrottle">
        /// Whether <c>403</c> counts as throttling. Defaults to <see langword="true"/>, because
        /// Emma uses 403 for rate limiting as well as the conventional 429 — the single most
        /// surprising behaviour in the API, and the one every consumer gets wrong first.
        /// <para>
        /// The trade-off is real: a genuine credentials failure also returns 403, and cannot be
        /// told apart from a throttle without inspecting the response body, which a resilience
        /// handler is not well placed to do. Retrying a bad-credentials 403 wastes the attempt
        /// budget and then fails, which is the less damaging error of the two. Pass
        /// <see langword="false"/> if your credentials are dynamic and you would rather fail fast.
        /// </para>
        /// </param>
        public static bool IsTransient(HttpStatusCode statusCode, bool treatForbiddenAsThrottle = true)
        {
            if (treatForbiddenAsThrottle && statusCode == HttpStatusCode.Forbidden)
            {
                return true;
            }

            return statusCode == TooManyRequests
                || statusCode == HttpStatusCode.RequestTimeout
                || statusCode == HttpStatusCode.InternalServerError
                || statusCode == HttpStatusCode.BadGateway
                || statusCode == HttpStatusCode.ServiceUnavailable
                || statusCode == HttpStatusCode.GatewayTimeout;
        }

        /// <summary>Whether a response should be retried.</summary>
        /// <param name="response">The response, which may be <see langword="null"/>.</param>
        /// <param name="treatForbiddenAsThrottle">See the overload taking a status code.</param>
        public static bool IsTransient(HttpResponseMessage? response, bool treatForbiddenAsThrottle = true)
            => response is not null && IsTransient(response.StatusCode, treatForbiddenAsThrottle);

        /// <summary>Whether a failure should be retried.</summary>
        /// <param name="exception">The exception, which may be <see langword="null"/>.</param>
        /// <remarks>
        /// Covers <see cref="EmmaRateLimitException"/>, transient <see cref="EmmaException"/>
        /// statuses, transport faults, and per-attempt timeouts.
        /// </remarks>
        public static bool IsTransient(Exception? exception) => exception switch
        {
            null => false,
            EmmaRateLimitException => true,
            EmmaException emma => IsTransient(emma.StatusCode),
            HttpRequestException => true,
            TimeoutException => true,

            // HttpClient surfaces its own timeout as a cancellation. On .NET 5+ that carries an
            // inner TimeoutException, which distinguishes it from the caller cancelling
            // deliberately - and the caller's own cancellation must never be retried. On .NET
            // Framework the inner exception is absent, so a client timeout is treated as
            // non-transient there rather than risk retrying a real cancellation.
            OperationCanceledException cancelled => cancelled.InnerException is TimeoutException,

            _ => false,
        };

        /// <summary>
        /// Reads Emma's <c>Retry-After</c> hint, whether expressed as a delay or a date.
        /// </summary>
        /// <returns>How long to wait, or <see langword="null"/> if Emma did not say.</returns>
        public static TimeSpan? GetRetryAfter(HttpResponseMessage? response)
        {
            if (response?.Headers.RetryAfter is null)
            {
                return null;
            }

            if (response.Headers.RetryAfter.Delta is TimeSpan delta)
            {
                return delta;
            }

            if (response.Headers.RetryAfter.Date is DateTimeOffset when)
            {
                TimeSpan remaining = when - DateTimeOffset.UtcNow;
                return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
            }

            return null;
        }
    }
}
