using System.Runtime.Serialization;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    [JsonConverter(typeof(StringEnumJsonConverter))]
    public enum PersonalizationType
    {
        Unknown,

        [EnumMember(Value = "all")]
        All,

        [EnumMember(Value = "html")]
        Html,

        [EnumMember(Value = "plaintext")]
        PlainText,

        [EnumMember(Value = "subject")]
        Subject,
    }
}
