using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmmaSharper.Unit.Fakes
{
    /// <summary>
    /// Captures the outgoing request and returns a canned response, so adapter behaviour can be
    /// asserted without a network call.
    /// </summary>
    internal sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> responder;

        internal StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            this.responder = responder;
        }

        /// <summary>The last request that passed through the handler.</summary>
        internal HttpRequestMessage? LastRequest { get; private set; }

        /// <summary>The last request URI, as an absolute string.</summary>
        internal string LastUri => LastRequest?.RequestUri?.ToString() ?? string.Empty;

        internal static StubHttpMessageHandler Returning(
            HttpStatusCode status,
            string? body = null,
            TimeSpan? retryAfter = null)
            => new(_ =>
            {
                HttpResponseMessage response = new(status)
                {
                    Content = new StringContent(body ?? string.Empty, Encoding.UTF8, "application/json"),
                };

                if (retryAfter is not null)
                {
                    response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(retryAfter.Value);
                }

                return response;
            });

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(responder(request));
        }
    }
}
