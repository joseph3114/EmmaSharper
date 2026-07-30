using System;
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
    public class TEmmaMemberFilters
    {
        private const string BaseUrl = "https://api.example.test";
        private const string AccountId = "1001";
        private const string ActiveFilter = """filter=["member_status_id","eq","a"]""";

        private static (IEmmaMemberProvider Provider, StubHttpMessageHandler Handler) Create(string json = "0")
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
            return (new MemberProvider(adapter), handler);
        }

        private static string Decoded(StubHttpMessageHandler handler) => Uri.UnescapeDataString(handler.LastUri);

        [Fact]
        public async Task GetMemberCount_WithoutStatus_SendsNoFilter()
        {
            (IEmmaMemberProvider provider, StubHttpMessageHandler handler) = Create();

            await provider.GetMemberCount();

            Decoded(handler).Should().Contain("count=true").And.NotContain("filter=");
        }

        [Fact]
        public async Task GetMemberCount_WithStatus_SendsEmmaFilterExpression()
        {
            (IEmmaMemberProvider provider, StubHttpMessageHandler handler) = Create();

            await provider.GetMemberCount(status: MemberStatusShort.Active);

            Decoded(handler).Should().Contain(ActiveFilter);
        }

        [Theory]
        [InlineData(MemberStatusShort.Active, "a")]
        [InlineData(MemberStatusShort.Optout, "o")]
        [InlineData(MemberStatusShort.Error, "e")]
        [InlineData(MemberStatusShort.Forwarded, "f")]
        public async Task GetMemberCount_MapsEachStatusToItsWireCode(MemberStatusShort status, string code)
        {
            (IEmmaMemberProvider provider, StubHttpMessageHandler handler) = Create();

            await provider.GetMemberCount(status: status);

            Decoded(handler).Should().Contain($"""["member_status_id","eq","{code}"]""");
        }

        [Fact]
        public async Task GetMemberCount_WithRawFilter_PassesItThroughUnchanged()
        {
            // The escape hatch: Emma's filter grammar is richer than the typed overload models.
            (IEmmaMemberProvider provider, StubHttpMessageHandler handler) = Create();
            const string raw = """["member_since","gt","2026-01-01"]""";

            await provider.GetMemberCount(rawFilter: raw);

            Decoded(handler).Should().Contain(raw);
        }

        [Fact]
        public async Task GetMemberCount_WithBothStatusAndRawFilter_Throws()
        {
            // Emma accepts one filter expression, so silently dropping one would be worse.
            (IEmmaMemberProvider provider, _) = Create();

            Func<Task> act = () => provider.GetMemberCount(
                status: MemberStatusShort.Active,
                rawFilter: """["x","eq","y"]""");

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetMemberCount_WithUnknownStatus_Throws()
        {
            // Unknown is the deserialization fall-back for statuses Emma has added and this
            // library does not model. It has no wire representation, so it cannot be a filter.
            (IEmmaMemberProvider provider, _) = Create();

            Func<Task> act = () => provider.GetMemberCount(status: MemberStatusShort.Unknown);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task ListMembers_ByDefault_DoesNotExcludeFields()
        {
            (IEmmaMemberProvider provider, StubHttpMessageHandler handler) = Create("[]");

            await provider.ListMembers();

            Decoded(handler).Should().NotContain("exclude_fields");
        }

        [Fact]
        public async Task ListMembers_ExcludeCustomFields_SendsExcludeFields()
        {
            (IEmmaMemberProvider provider, StubHttpMessageHandler handler) = Create("[]");

            await provider.ListMembers(fields: MemberFieldSelection.ExcludeCustomFields);

            Decoded(handler).Should().Contain("exclude_fields=1");
        }

        [Fact]
        public async Task ListMembers_TheEnterpriseSweepShape_SendsEveryParameter()
        {
            // Exactly the call a quota sync makes: active members only, no custom fields,
            // one inclusive 500-record page.
            (IEmmaMemberProvider provider, StubHttpMessageHandler handler) = Create("[]");

            await provider.ListMembers(
                start: 0,
                end: 499,
                status: MemberStatusShort.Active,
                fields: MemberFieldSelection.ExcludeCustomFields);

            string uri = Decoded(handler);
            uri.Should().Contain(ActiveFilter);
            uri.Should().Contain("exclude_fields=1");
            uri.Should().Contain("start=0");
            uri.Should().Contain("end=499");
        }
    }
}
