using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EmmaSharper.Adapters;
using EmmaSharper.Unit.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace EmmaSharper.Unit.Tests
{
    public class TEmmaEnterpriseProvider
    {
        private const string BaseUrl = "https://api.example.test";
        private const string AccountId = "enterprise-1";

        /// <summary>
        /// A recorded-shape response. Note the object envelope - the members endpoints return
        /// bare arrays instead, and assuming either shape universally breaks the other.
        /// </summary>
        private const string SubaccountsJson = """
        {
          "subaccounts": [
            { "account_id": "1001", "account_name": "College of Computing", "status": "active",
              "plan_name": "Pro", "contact_limit": 25000 },
            { "account_id": "1002", "account_name": "Alumni Relations", "status": "retired" }
          ]
        }
        """;

        private static (IEmmaEnterpriseProvider Provider, StubHttpMessageHandler Handler) Create(string json)
        {
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(HttpStatusCode.OK, json);
            HttpClient client = new(handler) { BaseAddress = new Uri(BaseUrl) };

            EmmaOptions options = new()
            {
                BaseUrl = BaseUrl,
                AccountId = AccountId,
                PublicKey = "public-key",
                SecretKey = "secret-key",
            };

            EmmaApiAdapter adapter = new(client, Options.Create(options), NullLogger<EmmaApiAdapter>.Instance);
            return (new EnterpriseProvider(adapter), handler);
        }

        [Fact]
        public async Task ListSubaccounts_UnwrapsTheObjectEnvelope()
        {
            (IEmmaEnterpriseProvider provider, _) = Create(SubaccountsJson);

            IReadOnlyList<Subaccount> result = await provider.ListSubaccounts();

            result.Should().HaveCount(2);
            result[0].AccountId.Should().Be("1001");
            result[0].AccountName.Should().Be("College of Computing");
            result[1].Status.Should().Be("retired");
        }

        [Fact]
        public async Task ListSubaccounts_CapturesUnmappedFields()
        {
            // Emma does not publish a full schema here, and plan/quota fields are exactly what a
            // quota tool wants. They must survive rather than being silently dropped.
            (IEmmaEnterpriseProvider provider, _) = Create(SubaccountsJson);

            IReadOnlyList<Subaccount> result = await provider.ListSubaccounts();

            result[0].AdditionalData.Should().ContainKey("contact_limit");
            result[0].AdditionalData!["contact_limit"].GetInt32().Should().Be(25000);
            result[0].AdditionalData.Should().ContainKey("plan_name");
        }

        [Fact]
        public async Task ListSubaccounts_HitsTheEnterpriseEndpoint()
        {
            (IEmmaEnterpriseProvider provider, StubHttpMessageHandler handler) = Create(SubaccountsJson);

            await provider.ListSubaccounts();

            handler.LastUri.Should().StartWith($"{BaseUrl}/{AccountId}/enterprise/subaccounts");
        }

        [Fact]
        public async Task ListSubaccounts_DefaultsToEveryStatus()
        {
            // Retired subaccounts can still hold billable contacts, so excluding them undercounts.
            (IEmmaEnterpriseProvider provider, StubHttpMessageHandler handler) = Create(SubaccountsJson);

            await provider.ListSubaccounts();

            Uri.UnescapeDataString(handler.LastUri)
               .Should().Contain("status=active,trial,pending_retirement,retired");
        }

        [Theory]
        [InlineData(SubaccountStatusFilter.Active, "status=active")]
        [InlineData(SubaccountStatusFilter.Retired, "status=retired")]
        [InlineData(SubaccountStatusFilter.Active | SubaccountStatusFilter.Trial, "status=active,trial")]
        public async Task ListSubaccounts_RendersTheStatusFlags(SubaccountStatusFilter filter, string expected)
        {
            (IEmmaEnterpriseProvider provider, StubHttpMessageHandler handler) = Create(SubaccountsJson);

            await provider.ListSubaccounts(filter);

            Uri.UnescapeDataString(handler.LastUri).Should().Contain(expected);
        }

        [Fact]
        public async Task ListSubaccounts_WithNoStatusSelected_Throws()
        {
            (IEmmaEnterpriseProvider provider, _) = Create(SubaccountsJson);

            Func<Task> act = () => provider.ListSubaccounts(default);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task ListSubaccounts_EmptyEnvelope_ReturnsEmptyRatherThanNull()
        {
            (IEmmaEnterpriseProvider provider, _) = Create("{}");

            IReadOnlyList<Subaccount> result = await provider.ListSubaccounts();

            result.Should().NotBeNull().And.BeEmpty();
        }
    }
}
