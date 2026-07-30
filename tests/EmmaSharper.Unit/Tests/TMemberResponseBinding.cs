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
    /// <summary>
    /// Binds a recorded <c>members</c> response - a <b>bare array</b>, unlike the object-wrapped
    /// enterprise and accounts endpoints.
    /// </summary>
    /// <remarks>
    /// <see cref="Member"/> is the most-used model in the library and carries a dozen mapped
    /// properties. A typo in any <c>[JsonPropertyName]</c> fails silently - the property simply
    /// stays null - so asserting the whole shape is the only way to catch it.
    /// </remarks>
    public class TMemberResponseBinding
    {
        private const string BaseUrl = "https://api.example.test";

        /// <summary>
        /// Deliberately mixes date formats: Emma writes its "@D:" prefix in places and plain ISO
        /// in others, and both must parse. The third member carries a status this library does not
        /// model, to exercise the Unknown fall-back.
        /// </summary>
        private const string MembersJson = """
        [
          {
            "member_id": 2169469051,
            "account_id": 1001,
            "email": "ada@example.edu",
            "status": "active",
            "member_status_id": "a",
            "member_since": "@D:2021-03-04T09:15:00",
            "last_modified_at": "2026-07-28T14:02:11",
            "confirmed_opt_in": "@D:2021-03-05T10:00:00",
            "deleted_at": null,
            "bounce_count": 0,
            "plaintext_preferred": false,
            "email_error": null,
            "fields": { "first_name": "Ada", "department": "Computing" }
          },
          {
            "member_id": 2169469052,
            "email": "grace@example.edu",
            "status": "opt-out",
            "member_status_id": "o",
            "member_since": "2019-11-01T00:00:00",
            "bounce_count": 3,
            "plaintext_preferred": true,
            "email_error": "550 mailbox unavailable"
          },
          {
            "member_id": 2169469053,
            "email": "future@example.edu",
            "status": "something_emma_added_later",
            "member_status_id": "a",
            "member_since": "2024-01-01T00:00:00"
          }
        ]
        """;

        private static async Task<List<Member>> ListAsync(string json)
        {
            StubHttpMessageHandler handler = StubHttpMessageHandler.Returning(HttpStatusCode.OK, json);
            HttpClient client = new(handler) { BaseAddress = new Uri(BaseUrl) };

            EmmaOptions options = new()
            {
                BaseUrl = BaseUrl,
                AccountId = "1001",
                PublicKey = "public-key",
                SecretKey = "secret-key",
            };

            EmmaApiAdapter adapter = new(client, Options.Create(options), NullLogger<EmmaApiAdapter>.Instance);
            IEmmaMemberProvider members = new MemberProvider(adapter);

            return new List<Member>(await members.ListMembers());
        }

        [Fact]
        public async Task BareArray_BindsWithoutAnEnvelope()
        {
            List<Member> members = await ListAsync(MembersJson);

            members.Should().HaveCount(3);
        }

        [Fact]
        public async Task EveryMappedProperty_Binds()
        {
            Member first = (await ListAsync(MembersJson))[0];

            first.MemberId.Should().Be(2169469051);   // exceeds Int32.MaxValue
            first.AccountId.Should().Be(1001);
            first.Email.Should().Be("ada@example.edu");
            first.BounceCount.Should().Be(0);
            first.PlaintextPreferred.Should().BeFalse();
            first.EmailError.Should().BeNull();
            first.DeletedAt.Should().BeNull();
            first.Fields.Should().ContainKey("first_name");
        }

        [Fact]
        public async Task BothStatusEnums_BindFromTheirDifferentWireFormats()
        {
            // `status` spells the value out; `member_status_id` uses a single letter. Same object.
            List<Member> members = await ListAsync(MembersJson);

            members[0].Status.Should().Be(MemberStatus.Active);
            members[0].MemberStatusId.Should().Be(MemberStatusShort.Active);
            members[1].Status.Should().Be(MemberStatus.Optout);
            members[1].MemberStatusId.Should().Be(MemberStatusShort.Optout);
        }

        [Fact]
        public async Task AStatusEmmaAddsLater_FallsBackToUnknownRatherThanThrowing()
        {
            List<Member> members = await ListAsync(MembersJson);

            members[2].Status.Should().Be(MemberStatus.Unknown);
            members[2].Email.Should().Be("future@example.edu");
        }

        [Fact]
        public async Task PrefixedAndPlainDates_BothParse()
        {
            List<Member> members = await ListAsync(MembersJson);

            members[0].ConfirmedOptIn.Should().Be(new DateTime(2021, 3, 5, 10, 0, 0));   // "@D:" prefixed
            members[0].LastModifiedAt.Should().Be(new DateTime(2026, 7, 28, 14, 2, 11));  // plain ISO
        }

        [Fact]
        public async Task NonNullableDate_UsesTheConverterToo()
        {
            // MemberSince is DateTime, not DateTime?. A JsonConverter<DateTime?> alone would not
            // apply to it, which is why the date converter is a JsonConverterFactory.
            List<Member> members = await ListAsync(MembersJson);

            members[0].MemberSince.Should().Be(new DateTime(2021, 3, 4, 9, 15, 0));
            members[1].MemberSince.Should().Be(new DateTime(2019, 11, 1, 0, 0, 0));
        }

        [Fact]
        public async Task AbsentProperties_StayNullRatherThanFailing()
        {
            // Emma omits fields freely, which is why the models are nullable.
            Member sparse = (await ListAsync(MembersJson))[2];

            sparse.AccountId.Should().BeNull();
            sparse.Fields.Should().BeNull();
            sparse.ConfirmedOptIn.Should().BeNull();
            sparse.BounceCount.Should().BeNull();
        }

        [Fact]
        public async Task EmptyArray_ReturnsEmptyNotNull()
        {
            List<Member> members = await ListAsync("[]");

            members.Should().NotBeNull().And.BeEmpty();
        }
    }
}
