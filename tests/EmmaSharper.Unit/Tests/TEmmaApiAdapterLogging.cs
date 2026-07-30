using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EmmaSharper.Adapters;
using EmmaSharper.Internals;
using EmmaSharper.Unit.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace EmmaSharper.Unit.Tests
{
    /// <summary>
    /// Guards a privacy regression CodeQL caught during the 8.0.0 rewrite: the adapter briefly
    /// logged the <i>resolved</i> request path, which embeds member email addresses for endpoints
    /// like GetMemberByEmail, putting PII into application logs.
    /// </summary>
    public class TEmmaApiAdapterLogging
    {
        private const string MemberEmail = "student@example.edu";

        private sealed class CapturingLogger<T> : ILogger<T>
        {
            internal List<string> Messages { get; } = new();

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
                => Messages.Add(formatter(state, exception));
        }

        private static async Task<CapturingLogger<EmmaApiAdapter>> CallMemberByEmailAsync()
        {
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(HttpStatusCode.OK, "{}");
            CapturingLogger<EmmaApiAdapter> logger = new();

            EmmaOptions options = new()
            {
                BaseUrl = "https://api.example.test",
                AccountId = "account-id",
                PublicKey = "public-key",
                SecretKey = "secret-key",
            };

            HttpClient client = new(handler) { BaseAddress = new Uri(options.BaseUrl) };
            EmmaApiAdapter adapter = new(client, Options.Create(options), logger);

            EmmaRequest request = new(Method.GET) { Resource = "/{accountId}/members/email/{memberEmail}" };
            request.AddUrlSegment("memberEmail", MemberEmail);

            await adapter.MakeRequest<object>(request);

            // The email must still reach Emma - it is only the log that must not carry it.
            handler.LastUri.Should().Contain(Uri.EscapeDataString(MemberEmail));

            return logger;
        }

        [Fact]
        public async Task MakeRequest_DoesNotLogMemberEmail()
        {
            CapturingLogger<EmmaApiAdapter> logger = await CallMemberByEmailAsync();

            logger.Messages.Should().NotBeEmpty();
            logger.Messages.Should().OnlyContain(m => !m.Contains(MemberEmail, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task MakeRequest_LogsTheUnresolvedTemplateAndTheAccount()
        {
            CapturingLogger<EmmaApiAdapter> logger = await CallMemberByEmailAsync();

            // Diagnostics are preserved: you can still tell which endpoint and which subaccount.
            logger.Messages.Should().Contain(m => m.Contains("{memberEmail}", StringComparison.Ordinal));
            logger.Messages.Should().Contain(m => m.Contains("account-id", StringComparison.Ordinal));
        }
    }
}
