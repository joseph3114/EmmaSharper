using System;
using System.Globalization;
using System.Text.Json;
using EmmaSharper.Internals;
using FluentAssertions;
using Xunit;

namespace EmmaSharper.Unit.Tests
{
    /// <summary>
    /// Emma is inconsistent about how it writes ids and dates, and one mismatched token fails the
    /// whole list response, not just the one field. These bind real models through the library's
    /// own <see cref="EmmaJson.Options"/>, so they exercise exactly what a provider call does.
    /// </summary>
    public class TEmmaJsonTolerance
    {
        private static T Read<T>(string json) => JsonSerializer.Deserialize<T>(json, EmmaJson.Options)!;

        // ---- Ids: number or string into string? ------------------------------------------------

        [Theory]
        [InlineData("1001", "1001")]
        [InlineData("\"1001\"", "1001")]
        [InlineData("\"u-someone@example.edu\"", "u-someone@example.edu")]
        // Beyond double precision: kept as written, not round-tripped through a number.
        [InlineData("9007199254740993", "9007199254740993")]
        [InlineData("null", null)]
        public void Id_AcceptsNumberOrString(string token, string? expected)
        {
            Subaccount result = Read<Subaccount>($$"""{ "account_id": {{token}} }""");

            result.AccountId.Should().Be(expected);
        }

        [Theory]
        [InlineData("true")]
        [InlineData("{ \"id\": 1 }")]
        [InlineData("[1001]")]
        public void Id_RejectsNonScalarTokens(string token)
        {
            // An object where an id belongs means the model is wrong; surface it, don't guess.
            Action act = () => Read<Subaccount>($$"""{ "account_id": {{token}} }""");

            act.Should().Throw<JsonException>();
        }

        [Fact]
        public void Id_WritesAsAString()
        {
            Subaccount subaccount = new() { AccountId = "1001" };

            string json = JsonSerializer.Serialize(subaccount, EmmaJson.Options);

            json.Should().Contain("\"account_id\":\"1001\"");
        }

        [Fact]
        public void Workflow_IdAcceptsNumberOrString()
        {
            Read<Workflow>("""{ "workflow_id": 77 }""").WorkflowId.Should().Be("77");
            Read<Workflow>("""{ "workflow_id": "wf-77" }""").WorkflowId.Should().Be("wf-77");
        }

        [Fact]
        public void WebhookSignup_IdsAcceptNumbers()
        {
            const string json = """
            { "signup_form_id": 5001, "account_id": 1001, "member_id": 2169469051,
              "mailing_id": 3001, "timestamp": "@D:2026-07-28T14:02:11" }
            """;

            WebhookPostDataMemberSignup result = Read<WebhookPostDataMemberSignup>(json);

            result.SignupFormId.Should().Be("5001");
            result.AccountId.Should().Be("1001");
            result.MemberId.Should().Be("2169469051");
            result.MailingId.Should().Be(3001);
            result.Timestamp.Should().Be(new DateTime(2026, 7, 28, 14, 2, 11));
        }

        // ---- Dates: "@D:", ISO and Emma's display form -------------------------------------------

        [Theory]
        [InlineData("Sep 25, 2024", 2024, 9, 25, 0, 0, 0)]
        [InlineData("Sep 5, 2024", 2024, 9, 5, 0, 0, 0)]
        [InlineData("September 5, 2024", 2024, 9, 5, 0, 0, 0)]
        [InlineData(" Sep 25, 2024 ", 2024, 9, 25, 0, 0, 0)]
        [InlineData("@D:2021-03-04T09:15:00", 2021, 3, 4, 9, 15, 0)]
        [InlineData("2026-07-28T14:02:11", 2026, 7, 28, 14, 2, 11)]
        public void Date_ParsesEveryEmmaFormat(string raw, int y, int mo, int d, int h, int mi, int s)
        {
            AccountUser result = Read<AccountUser>($$"""{ "create_ts": "{{raw}}" }""");

            result.CreatedAt.Should().Be(new DateTime(y, mo, d, h, mi, s));
        }

        [Theory]
        [InlineData("\"not a date\"")]
        [InlineData("\"\"")]
        [InlineData("1727222400")]
        [InlineData("null")]
        public void Date_UnreadableValues_AreNullRatherThanThrowing(string token)
        {
            AccountUser result = Read<AccountUser>($$"""{ "create_ts": {{token}}, "email": "a@example.edu" }""");

            result.CreatedAt.Should().BeNull();
            result.Email.Should().Be("a@example.edu");
        }

        [Fact]
        public void Date_StructuredValue_IsSkippedWithoutDesyncingTheReader()
        {
            // Returning without consuming the object would leave the reader inside it, and
            // System.Text.Json would throw for the whole payload.
            const string json = """
            { "create_ts": { "date": "Sep 25, 2024" }, "last_login_attempt": ["x"],
              "email": "a@example.edu" }
            """;

            AccountUser result = Read<AccountUser>(json);

            result.CreatedAt.Should().BeNull();
            result.LastLoginAttempt.Should().BeNull();
            result.Email.Should().Be("a@example.edu");
        }

        [Theory]
        [InlineData("de-DE")]
        [InlineData("fr-FR")]
        [InlineData("ja-JP")]
        public void Date_DisplayForm_DoesNotDependOnTheHostCulture(string culture)
        {
            // "Sep" is not a month abbreviation in these cultures. A wire format must parse the
            // same on every server.
            CultureInfo original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo(culture);

                AccountUser result = Read<AccountUser>("""{ "create_ts": "Sep 25, 2024" }""");

                result.CreatedAt.Should().Be(new DateTime(2024, 9, 25));
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        [Fact]
        public void Date_NonNullableProperty_ParsesDisplayFormOrFallsBackToDefault()
        {
            // Workflow's dates are non-nullable DateTime - the factory's other converter.
            Workflow parsed = Read<Workflow>("""{ "created_at": "Sep 25, 2024" }""");
            Workflow unreadable = Read<Workflow>("""{ "created_at": "not a date" }""");

            parsed.CreatedAt.Should().Be(new DateTime(2024, 9, 25));
            unreadable.CreatedAt.Should().Be(default);
        }
    }
}
