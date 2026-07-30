using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    [JsonConverter(typeof(StringEnumJsonConverter))]
    public enum DeliveryTypeShort
    {
        Unknown,

        [EnumMember(Value = "d")]
        Delivered,

        [EnumMember(Value = "h")]
        Hard,

        [EnumMember(Value = "s")]
        Soft,
    }
}
