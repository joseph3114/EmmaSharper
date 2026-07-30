using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    [JsonConverter(typeof(StringEnumJsonConverter))]
    public enum ImportStatus
    {
        Unknown,

        [EnumMember(Value = "o")]
        Okay,

        [EnumMember(Value = "e")]
        Error,

        [EnumMember(Value = "q")]
        Queued,
    }
}
