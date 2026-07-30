using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EmmaSharper.Adapters;
using EmmaSharper.Internals;
using EmmaSharper.Unit.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace EmmaSharper.Unit.Tests
{
    public class TEmmaApiAdapter
    {
        private const string BaseUrl = "https://api.example.test";
        private const string AccountId = "account-id";

        // HttpStatusCode.TooManyRequests does not exist on .NET Framework, which the net472
        // test leg runs on. A cast of a constant is still a constant, so this is usable in
        // an InlineData attribute.
        private const HttpStatusCode TooManyRequests = (HttpStatusCode)429;

        private static EmmaApiAdapter CreateAdapter(StubHttpMessageHandler handler, string? accountId = AccountId)
        {
            HttpClient client = new(handler) { BaseAddress = new Uri(BaseUrl) };

            EmmaOptions options = new()
            {
                BaseUrl = BaseUrl,
                AccountId = accountId,
                PublicKey = "public-key",
                SecretKey = "secret-key",
            };

            return new EmmaApiAdapter(client, Options.Create(options), NullLogger<EmmaApiAdapter>.Instance);
        }

        private static EmmaRequest Request(string resource = "/{accountId}/self")
            => new(Method.GET) { Resource = resource };

        [Fact]
        public async Task MakeRequest_DeserializesResponseBody()
        {
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(HttpStatusCode.OK, "\"Hello World\"");

            string? result = await CreateAdapter(handler).MakeRequest<string>(Request());

            result.Should().Be("Hello World");
        }

        [Fact]
        public async Task MakeRequest_SubstitutesAccountIdIntoPath()
        {
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(HttpStatusCode.OK, "\"ok\"");

            await CreateAdapter(handler).MakeRequest<string>(Request());

            handler.LastUri.Should().Be($"{BaseUrl}/{AccountId}/self");
        }

        [Fact]
        public async Task MakeRequest_ExplicitAccountId_OverridesConfiguredAccount()
        {
            // The enterprise case: one credential pair, many subaccounts.
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(HttpStatusCode.OK, "\"ok\"");

            await CreateAdapter(handler).MakeRequest<string>(Request(), accountId: "subaccount-42");

            handler.LastUri.Should().Be($"{BaseUrl}/subaccount-42/self");
        }

        [Fact]
        public async Task MakeRequest_Paging_UsesInclusiveRange()
        {
            // Emma's range is inclusive, so a 500-record page ends at 499, not 500.
            // 7.x emitted end=500 here and asked for 501 records.
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(HttpStatusCode.OK, "[]");

            await CreateAdapter(handler).MakeRequest<string[]>(Request(), start: 0);

            handler.LastUri.Should().Contain("start=0").And.Contain("end=499");
        }

        [Fact]
        public async Task MakeRequest_Paging_EndBelowPageSize_DoesNotUnderflow()
        {
            // 7.x computed `end - 500` on a uint, wrapping to ~4.29 billion.
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(HttpStatusCode.OK, "[]");

            await CreateAdapter(handler).MakeRequest<string[]>(Request(), end: 100);

            handler.LastUri.Should().Contain("start=0").And.Contain("end=100");
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        public async Task MakeRequest_NonSuccess_ThrowsEmmaExceptionCarryingStatus(HttpStatusCode status)
        {
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(status, "boom");
            EmmaApiAdapter adapter = CreateAdapter(handler);

            Func<Task> act = () => adapter.MakeRequest<string>(Request());

            EmmaException thrown = (await act.Should().ThrowAsync<EmmaException>()).Which;
            thrown.StatusCode.Should().Be(status);
            thrown.ResponseBody.Should().Be("boom");
            thrown.Should().NotBeOfType<EmmaRateLimitException>();
        }

        [Theory]
        [InlineData(TooManyRequests)]
        [InlineData(HttpStatusCode.Forbidden)]
        public async Task MakeRequest_Throttled_ThrowsRateLimitException(HttpStatusCode status)
        {
            // Emma signals throttling with 403 as well as 429 - the behaviour every consumer
            // otherwise mistakes for an auth failure.
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(status);
            EmmaApiAdapter adapter = CreateAdapter(handler);

            Func<Task> act = () => adapter.MakeRequest<string>(Request());

            await act.Should().ThrowAsync<EmmaRateLimitException>();
        }

        [Fact]
        public async Task MakeRequest_Throttled_SurfacesRetryAfter()
        {
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(
                TooManyRequests,
                retryAfter: TimeSpan.FromSeconds(30));

            EmmaApiAdapter adapter = CreateAdapter(handler);

            Func<Task> act = () => adapter.MakeRequest<string>(Request());

            EmmaRateLimitException thrown = (await act.Should().ThrowAsync<EmmaRateLimitException>()).Which;
            thrown.RetryAfter.Should().Be(TimeSpan.FromSeconds(30));
        }

        [Fact]
        public async Task MakeRequest_BareInteger_Deserializes()
        {
            // `members?count=true` returns a bare integer, not a JSON object.
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(HttpStatusCode.OK, "2431");

            int result = await CreateAdapter(handler).MakeRequest<int>(Request("/{accountId}/members"));

            result.Should().Be(2431);
        }

        [Fact]
        public async Task MakeRequest_EmptyBody_ReturnsDefault()
        {
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(HttpStatusCode.NoContent);

            string? result = await CreateAdapter(handler).MakeRequest<string>(Request());

            result.Should().BeNull();
        }

        [Fact]
        public async Task MakeRequest_WithNoAccountIdAnywhere_Throws()
        {
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(HttpStatusCode.OK, "\"ok\"");
            EmmaApiAdapter adapter = CreateAdapter(handler, accountId: null);

            Func<Task> act = () => adapter.MakeRequest<string>(Request());

            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
