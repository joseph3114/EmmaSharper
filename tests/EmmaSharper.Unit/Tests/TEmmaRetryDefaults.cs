using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace EmmaSharper.Unit.Tests
{
    public class TEmmaRetryDefaults
    {
        private const HttpStatusCode TooManyRequests = (HttpStatusCode)429;

        [Theory]
        [InlineData(TooManyRequests)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.GatewayTimeout)]
        public void IsTransient_RetriesTheUsualSuspects(HttpStatusCode status)
            => EmmaRetryDefaults.IsTransient(status).Should().BeTrue();

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.Conflict)]
        [InlineData(HttpStatusCode.OK)]
        public void IsTransient_DoesNotRetryClientErrors(HttpStatusCode status)
            => EmmaRetryDefaults.IsTransient(status).Should().BeFalse();

        [Fact]
        public void IsTransient_TreatsForbiddenAsThrottlingByDefault()
        {
            // The whole point of this type: Emma rate-limits with 403, not only 429.
            EmmaRetryDefaults.IsTransient(HttpStatusCode.Forbidden).Should().BeTrue();
        }

        [Fact]
        public void IsTransient_ForbiddenCanBeOptedOut()
        {
            // For callers with dynamic credentials, who would rather fail fast on a real 403.
            EmmaRetryDefaults
                .IsTransient(HttpStatusCode.Forbidden, treatForbiddenAsThrottle: false)
                .Should().BeFalse();
        }

        [Fact]
        public void IsTransient_NullResponse_IsNotTransient()
            => EmmaRetryDefaults.IsTransient((HttpResponseMessage?)null).Should().BeFalse();

        [Fact]
        public void IsTransient_ReadsTheStatusFromAResponse()
        {
            using HttpResponseMessage response = new(HttpStatusCode.ServiceUnavailable);

            EmmaRetryDefaults.IsTransient(response).Should().BeTrue();
        }

        [Fact]
        public void IsTransient_RateLimitException_IsTransient()
            => EmmaRetryDefaults.IsTransient(new EmmaRateLimitException(TooManyRequests)).Should().BeTrue();

        [Fact]
        public void IsTransient_EmmaException_FollowsItsStatus()
        {
            EmmaRetryDefaults.IsTransient(new EmmaException(HttpStatusCode.ServiceUnavailable)).Should().BeTrue();
            EmmaRetryDefaults.IsTransient(new EmmaException(HttpStatusCode.NotFound)).Should().BeFalse();
        }

        [Fact]
        public void IsTransient_TransportFaults_AreTransient()
        {
            EmmaRetryDefaults.IsTransient(new HttpRequestException("socket")).Should().BeTrue();
            EmmaRetryDefaults.IsTransient(new TimeoutException()).Should().BeTrue();
        }

        [Fact]
        public void IsTransient_CallerCancellation_IsNotRetried()
        {
            // Retrying a deliberate cancellation would defeat the point of the token.
            using CancellationTokenSource cts = new();
            cts.Cancel();

            EmmaRetryDefaults.IsTransient(new OperationCanceledException(cts.Token)).Should().BeFalse();
        }

        [Fact]
        public void IsTransient_ClientTimeoutDressedAsCancellation_IsRetried()
        {
            // HttpClient reports its own timeout as a cancellation carrying an inner
            // TimeoutException, which is what tells it apart from a real cancellation.
            Exception timedOut = new TaskCanceledException("timeout", new TimeoutException());

            EmmaRetryDefaults.IsTransient(timedOut).Should().BeTrue();
        }

        [Fact]
        public void IsTransient_NullException_IsNotTransient()
            => EmmaRetryDefaults.IsTransient((Exception?)null).Should().BeFalse();

        [Fact]
        public void GetRetryAfter_ReadsADelta()
        {
            using HttpResponseMessage response = new(TooManyRequests);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(45));

            EmmaRetryDefaults.GetRetryAfter(response).Should().Be(TimeSpan.FromSeconds(45));
        }

        [Fact]
        public void GetRetryAfter_ConvertsADateToARemainingDelay()
        {
            using HttpResponseMessage response = new(TooManyRequests);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddMinutes(1));

            TimeSpan? result = EmmaRetryDefaults.GetRetryAfter(response);

            result.Should().NotBeNull();
            result!.Value.Should().BeGreaterThan(TimeSpan.Zero).And.BeLessThanOrEqualTo(TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void GetRetryAfter_PastDate_ClampsToZeroRatherThanGoingNegative()
        {
            using HttpResponseMessage response = new(TooManyRequests);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddMinutes(-5));

            EmmaRetryDefaults.GetRetryAfter(response).Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void GetRetryAfter_WhenEmmaDidNotSay_IsNull()
        {
            using HttpResponseMessage response = new(TooManyRequests);

            EmmaRetryDefaults.GetRetryAfter(response).Should().BeNull();
            EmmaRetryDefaults.GetRetryAfter(null).Should().BeNull();
        }
    }
}
