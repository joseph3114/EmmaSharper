using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

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
