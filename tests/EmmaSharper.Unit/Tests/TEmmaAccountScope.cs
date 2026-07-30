using System.Net;
using System.Threading.Tasks;
using EmmaSharper.Unit.Fakes;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EmmaSharper.Unit.Tests
{
    /// <summary>
    /// Covers the enterprise case from BinaryPatrick/EmmaSharper#6 - one credential pair
    /// addressing many subaccounts.
    /// </summary>
    public class TEmmaAccountScope
    {
        private const string DefaultAccount = "default-account";

        private static (ServiceProvider Provider, StubHttpMessageHandler Handler) Build()
        {
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(HttpStatusCode.OK, "0");

            ServiceCollection services = new();
            services.AddLogging();
            services.AddEmmaApiProviders(options =>
                    {
                        options.BaseUrl = "https://api.example.test";
                        options.AccountId = DefaultAccount;
                        options.PublicKey = "public-key";
                        options.SecretKey = "secret-key";
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => handler);

            return (services.BuildServiceProvider(), handler);
        }

        [Fact]
        public async Task DefaultProvider_UsesConfiguredAccount()
        {
            (ServiceProvider provider, StubHttpMessageHandler handler) = Build();
            using ServiceProvider _ = provider;

            await provider.GetRequiredService<IEmmaMemberProvider>().GetMemberCount();

            handler.LastUri.Should().Contain($"/{DefaultAccount}/members");
        }

        [Fact]
        public async Task ForAccount_RetargetsCallsAtTheScopedAccount()
        {
            (ServiceProvider provider, StubHttpMessageHandler handler) = Build();
            using ServiceProvider _ = provider;

            IEmmaAccountScope scope = provider
                .GetRequiredService<IEmmaAccountScopeFactory>()
                .ForAccount("subaccount-7");

            await scope.Members.GetMemberCount();

            handler.LastUri.Should().Contain("/subaccount-7/members");
            handler.LastUri.Should().NotContain(DefaultAccount);
        }

        [Fact]
        public void ForAccount_ExposesTheAccountItTargets()
        {
            (ServiceProvider provider, _) = Build();
            using ServiceProvider _p = provider;

            IEmmaAccountScope scope = provider
                .GetRequiredService<IEmmaAccountScopeFactory>()
                .ForAccount("subaccount-7");

            scope.AccountId.Should().Be("subaccount-7");
            scope.Members.Should().NotBeNull();
            scope.Groups.Should().NotBeNull();
        }

        [Fact]
        public async Task Scopes_AreIndependentOfEachOther()
        {
            // The ~47-subaccount sweep creates a scope per account and drives them concurrently.
            (ServiceProvider provider, StubHttpMessageHandler handler) = Build();
            using ServiceProvider _ = provider;

            IEmmaAccountScopeFactory factory = provider.GetRequiredService<IEmmaAccountScopeFactory>();

            await factory.ForAccount("acct-a").Members.GetMemberCount();
            string first = handler.LastUri;

            await factory.ForAccount("acct-b").Members.GetMemberCount();

            first.Should().Contain("/acct-a/");
            handler.LastUri.Should().Contain("/acct-b/");
        }
    }
}
