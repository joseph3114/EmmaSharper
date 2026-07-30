using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EmmaSharper.Internals;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmmaSharper.Adapters
{
    /// <inheritdoc cref="IEmmaApiAdapter"/>
    internal sealed class EmmaApiAdapter : IEmmaApiAdapter
    {
        /// <summary>Emma caps a single page at 500 records.</summary>
        private const int MaxPageSize = 500;

        private readonly HttpClient httpClient;
        private readonly EmmaOptions options;
        private readonly ILogger<EmmaApiAdapter> logger;

        public EmmaApiAdapter(HttpClient httpClient, IOptions<EmmaOptions> options, ILogger<EmmaApiAdapter> logger)
        {
            this.httpClient = httpClient;
            this.options = options.Value;
            this.logger = logger;
        }

        // The 7.x adapter set ServicePointManager.SecurityProtocol from a static constructor. That
        // mutated TLS settings for the entire host process from inside a library, and has been a
        // no-op on .NET Core since 3.0 - its removal here is deliberate.

        public async Task<T?> MakeRequest<T>(
            EmmaRequest request,
            uint? start = null,
            uint? end = null,
            string? accountId = null,
            CancellationToken cancellationToken = default)
        {
            string account = ResolveAccount(accountId);
            string resource = ResolveResource(request, account);
            string uri = resource + BuildQuery(request, start, end);

            using HttpRequestMessage message = new(request.Method, uri);

            if (request.Body is not null)
            {
                string json = JsonSerializer.Serialize(request.Body, request.Body.GetType(), EmmaJson.Options);
                message.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            // Logs the UNRESOLVED template, never the substituted path. Resolved paths embed
            // member email addresses (see GetMemberByEmail), so logging them would write PII into
            // application logs. The account id is logged separately - it is a tenant identifier,
            // not a secret, and it is what you need to follow a subaccount sweep.
            logger.LogDebug(
                "Emma request {Method} {Resource} starting for account {AccountId}",
                request.Method.Method,
                request.Resource,
                account);

            using HttpResponseMessage response = await httpClient
                .SendAsync(message, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            // The CancellationToken overload is .NET 5+. On netstandard2.0 the body read is not
            // separately cancellable; SendAsync above still honours the token.
#if NETSTANDARD2_0
            string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#else
            string body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#endif

            logger.LogDebug(
                "Emma request {Method} {Resource} for account {AccountId} completed with {StatusCode}",
                request.Method.Method,
                request.Resource,
                account,
                (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                throw CreateException(response, body, request.Method, resource);
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(body, EmmaJson.Options);
        }

        /// <summary>Picks the per-call account override, falling back to the configured default.</summary>
        private string ResolveAccount(string? accountId)
            => accountId ?? options.AccountId
                ?? throw new InvalidOperationException(
                    $"No account id supplied. Set {nameof(EmmaOptions)}.{nameof(EmmaOptions.AccountId)} " +
                    "or pass an explicit account id for this call.");

        /// <summary>Substitutes <c>{accountId}</c> and any other placeholders into the path.</summary>
        private static string ResolveResource(EmmaRequest request, string account)
        {
            string resource = request.Resource.Replace("{accountId}", Uri.EscapeDataString(account));

            foreach (KeyValuePair<string, string> segment in request.Segments)
            {
                resource = resource.Replace("{" + segment.Key + "}", Uri.EscapeDataString(segment.Value));
            }

            return resource;
        }

        private static string BuildQuery(EmmaRequest request, uint? start, uint? end)
        {
            List<KeyValuePair<string, string>> query = new(request.Query);

            if (start.HasValue || end.HasValue)
            {
                (uint from, uint to) = ResolvePage(start, end);
                query.Add(new KeyValuePair<string, string>("start", from.ToString(CultureInfo.InvariantCulture)));
                query.Add(new KeyValuePair<string, string>("end", to.ToString(CultureInfo.InvariantCulture)));
            }

            if (query.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new("?");
            for (int i = 0; i < query.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append('&');
                }

                builder.Append(Uri.EscapeDataString(query[i].Key))
                       .Append('=')
                       .Append(Uri.EscapeDataString(query[i].Value));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Resolves a page window. Emma's range is <b>inclusive</b>, so a 500-record page is
        /// <c>end = start + 499</c>.
        /// </summary>
        /// <remarks>
        /// Fixes two defects in the 7.x helpers: <c>ValidateEndPage</c> returned <c>start + 500</c>,
        /// requesting 501 records, and <c>ValidateStartPage</c> computed <c>end - 500</c> on a
        /// <see cref="uint"/>, which wrapped to roughly 4.29 billion whenever <c>end</c> was below
        /// 500 and no start was supplied.
        /// </remarks>
        private static (uint Start, uint End) ResolvePage(uint? start, uint? end)
        {
            uint from = start ?? (end.HasValue && end.Value >= MaxPageSize
                ? end.Value - MaxPageSize + 1
                : 0u);

            uint to = end ?? from + MaxPageSize - 1;

            return to < from ? (from, from) : (from, to);
        }

        /// <summary>
        /// Emma throttles with <b>403</b> as well as 429, so both map to
        /// <see cref="EmmaRateLimitException"/>.
        /// </summary>
        private static EmmaException CreateException(
            HttpResponseMessage response,
            string body,
            HttpMethod method,
            string resource)
        {
            // HttpStatusCode.TooManyRequests does not exist on netstandard2.0; the cast is
            // equivalent and avoids an #if.
            const HttpStatusCode tooManyRequests = (HttpStatusCode)429;

            bool throttled = response.StatusCode == tooManyRequests
                          || response.StatusCode == HttpStatusCode.Forbidden;

            if (!throttled)
            {
                return new EmmaException(response.StatusCode, body, method, resource);
            }

            TimeSpan? retryAfter = response.Headers.RetryAfter?.Delta;

            if (retryAfter is null && response.Headers.RetryAfter?.Date is DateTimeOffset when)
            {
                TimeSpan delta = when - DateTimeOffset.UtcNow;
                retryAfter = delta > TimeSpan.Zero ? delta : TimeSpan.Zero;
            }

            return new EmmaRateLimitException(response.StatusCode, retryAfter, body, method, resource);
        }
    }
}
