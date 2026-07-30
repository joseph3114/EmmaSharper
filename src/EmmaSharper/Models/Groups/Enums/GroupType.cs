using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    [JsonConverter(typeof(StringEnumJsonConverter))]
    public enum GroupType
    {
        Unknown,

        [EnumMember(Value = "g")]
        Group,

        [EnumMember(Value = "t")]
        Test,

        [EnumMember(Value = "h")]
        Hidden,

        [EnumMember(Value = "all")]
        All,
    }
}
