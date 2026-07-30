using System.Runtime.Serialization;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    [JsonConverter(typeof(StringEnumJsonConverter))]
    public enum DeliveryType
    {
        Unknown,

        [EnumMember(Value = "all")]
        All,

        [EnumMember(Value = "delivered")]
        Delivered,

        [EnumMember(Value = "bounced")]
        Bounced,

        [EnumMember(Value = "hard")]
        Hard,

        [EnumMember(Value = "soft")]
        Soft,
    }
}
