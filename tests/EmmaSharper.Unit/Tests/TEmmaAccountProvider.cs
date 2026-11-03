using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EmmaSharper.Adapters;
using EmmaSharper.Unit.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace EmmaSharper.Unit.Tests
{
    public class TEmmaAccountProvider
    {
        private const string BaseUrl = "https://api.example.test";
        private const string AccountId = "1001";

        /// <summary>
        /// Recorded shape (values fabricated). Emma's <c>user_id</c> is a login name, not a number,
        /// and the two timestamps are display dates ("Sep 25, 2024"), not ISO. 8.0.x modelled
        /// both wrongly and threw on every real response. <c>accounts</c> and
        /// <c>user_group_list</c> are present but unmodelled; their element shape here is a
        /// placeholder.
        /// </summary>
        private const string UsersJson = """
        {
          "users": [
            { "user_id": "u-alovelace@example.edu", "email": "staff@example.edu",
              "user_name_first": "Ada", "user_name_last": "Lovelace",
              "role": "Account Administrator",
              "create_ts": "Sep 25, 2024", "last_login_attempt": "Oct 2, 2026",
              "accounts": [1001, 1002], "user_group_list": [] },
            { "user_id": "u-other@example.edu", "email": "other@example.edu", "role": "Viewer",
              "create_ts": null, "last_login_attempt": null }
          ]
        }
        """;

        /// <summary>
        /// The shape this library originally assumed. Kept so that, should an older account or a
        /// future API version send a numeric id, it still binds.
        /// </summary>
        private const string LegacyNumericUsersJson = """
        {
          "users": [
            { "user_id": 42, "email": "staff@example.edu",
              "create_ts": "@D:2021-03-04T09:15:00", "last_login_attempt": "2026-07-28T14:02:11" }
          ]
        }
        """;

        private static (IEmmaAccountProvider Provider, StubHttpMessageHandler Handler) Create(string json)
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
            return (new AccountProvider(adapter), handler);
        }

        [Fact]
        public async Task ListUsers_UnwrapsTheObjectEnvelope()
        {
            (IEmmaAccountProvider provider, _) = Create(UsersJson);

            IReadOnlyList<AccountUser> result = await provider.ListUsers();

            result.Should().HaveCount(2);
            result[0].UserId.Should().Be("u-alovelace@example.edu");
            result[0].Email.Should().Be("staff@example.edu");
            result[0].FirstName.Should().Be("Ada");
            result[0].LastName.Should().Be("Lovelace");
            result[0].Role.Should().Be("Account Administrator");
            result[1].UserId.Should().Be("u-other@example.edu");
        }

        [Fact]
        public async Task ListUsers_ParsesEmmaDisplayDates()
        {
            (IEmmaAccountProvider provider, _) = Create(UsersJson);

            IReadOnlyList<AccountUser> result = await provider.ListUsers();

            // Typed as dates, not strings - a user grid has to be sortable by last login.
            result[0].CreatedAt.Should().Be(new DateTime(2024, 9, 25));
            result[0].LastLoginAttempt.Should().Be(new DateTime(2026, 10, 2));
        }

        [Fact]
        public async Task ListUsers_UnmodelledArraysSurviveInAdditionalData()
        {
            (IEmmaAccountProvider provider, _) = Create(UsersJson);

            IReadOnlyList<AccountUser> result = await provider.ListUsers();

            result[0].AdditionalData.Should().ContainKeys("accounts", "user_group_list");
            result[0].AdditionalData!["accounts"].ValueKind.Should().Be(JsonValueKind.Array);
            result[0].AdditionalData!["accounts"].GetArrayLength().Should().Be(2);
            result[0].AdditionalData!["user_group_list"].GetArrayLength().Should().Be(0);
            // Mapped properties must not leak into the overflow dictionary.
            result[0].AdditionalData.Should().NotContainKeys("user_id", "create_ts");
        }

        [Fact]
        public async Task ListUsers_LegacyNumericIdAndIsoDates_StillBind()
        {
            (IEmmaAccountProvider provider, _) = Create(LegacyNumericUsersJson);

            IReadOnlyList<AccountUser> result = await provider.ListUsers();

            result[0].UserId.Should().Be("42");
            result[0].CreatedAt.Should().Be(new DateTime(2021, 3, 4, 9, 15, 0));
            result[0].LastLoginAttempt.Should().Be(new DateTime(2026, 7, 28, 14, 2, 11));
        }

        [Fact]
        public async Task ListUsers_ToleratesNullTimestamps()
        {
            (IEmmaAccountProvider provider, _) = Create(UsersJson);

            IReadOnlyList<AccountUser> result = await provider.ListUsers();

            result[1].CreatedAt.Should().BeNull();
            result[1].LastLoginAttempt.Should().BeNull();
            result[1].FirstName.Should().BeNull();
        }

        [Fact]
        public async Task ListUsers_HitsTheAccountsUsersEndpoint()
        {
            (IEmmaAccountProvider provider, StubHttpMessageHandler handler) = Create(UsersJson);

            await provider.ListUsers();

            handler.LastUri.Should().Be($"{BaseUrl}/{AccountId}/accounts/users");
        }

        [Fact]
        public async Task ListUsers_EmptyEnvelope_ReturnsEmptyRatherThanNull()
        {
            (IEmmaAccountProvider provider, _) = Create("{}");

            IReadOnlyList<AccountUser> result = await provider.ListUsers();

            result.Should().NotBeNull().And.BeEmpty();
        }
    }
}
