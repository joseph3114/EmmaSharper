using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    [JsonConverter(typeof(StringEnumJsonConverter))]
    public enum UpdateMailingStatus
    {
        Unknown,

        [EnumMember(Value = "canceled")]
        Canceled,

        [EnumMember(Value = "paused")]
        Paused,

        [EnumMember(Value = "ready")]
        Ready,
    }
}