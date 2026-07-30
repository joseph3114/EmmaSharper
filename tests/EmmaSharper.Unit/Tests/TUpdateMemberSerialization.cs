using System.Text.Json;
using EmmaSharper.Internals;
using FluentAssertions;
using Xunit;

namespace EmmaSharper.Unit.Tests
{
    /// <summary>
    /// <see cref="UpdateMember"/> is a partial-update body, so anything the caller did not set
    /// must not appear on the wire.
    /// </summary>
    public class TUpdateMemberSerialization
    {
        private static string Serialize(UpdateMember member)
            => JsonSerializer.Serialize(member, EmmaJson.Options);

        [Fact]
        public void ChangingOnlyTheEmail_DoesNotSendAStatus()
        {
            // Regression: StatusTo used to be a non-nullable enum, so it defaulted to Unknown and
            // was serialized on every request - sending "status_to":"Unknown", which Emma does
            // not accept, on a call that only meant to rename a member.
            string json = Serialize(new UpdateMember { MemberEmail = "john.smith@example.edu" });

            json.Should().Contain("\"email\":\"john.smith@example.edu\"");
            json.Should().NotContain("status_to");
            json.Should().NotContain("Unknown");
        }

        [Fact]
        public void SettingAStatus_SendsItsWireCode()
        {
            string json = Serialize(new UpdateMember { StatusTo = MemberStatusShort.Optout });

            json.Should().Contain("\"status_to\":\"o\"");
        }

        [Fact]
        public void UnsetFields_AreOmittedEntirely()
        {
            string json = Serialize(new UpdateMember());

            json.Should().NotContain("email");
            json.Should().NotContain("status_to");
            json.Should().NotContain("fields");
        }
    }
}
