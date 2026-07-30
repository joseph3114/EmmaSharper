using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    [JsonConverter(typeof(StringEnumJsonConverter))]
    public enum MemberStatus
    {
        Unknown,

        [EnumMember(Value = "active")]
        Active,

        [EnumMember(Value = "opt-out")]
        Optout,

        [EnumMember(Value = "error")]
        Error,

        [EnumMember(Value = "forwarded")]
        Forwarded,
    }
}
