using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    [JsonConverter(typeof(StringEnumJsonConverter))]
    public enum MemberStatusShort
    {
        Unknown,

        [EnumMember(Value = "a")]
        Active,

        [EnumMember(Value = "o")]
        Optout,

        [EnumMember(Value = "e")]
        Error,

        [EnumMember(Value = "f")]
        Forwarded,
    }
}
