using System.Runtime.Serialization;
using EmmaSharper.Internals;
using System.Text.Json.Serialization;

namespace EmmaSharper
{
    [JsonConverter(typeof(StringEnumJsonConverter))]
    public enum FieldType
    {
        Unknown,

        [EnumMember(Value = "text")]
        Text,

        [EnumMember(Value = "text[]")]
        TextArray,

        [EnumMember(Value = "numeric")]
        Numeric,

        [EnumMember(Value = "boolean")]
        Boolean,

        [EnumMember(Value = "date")]
        Date,

        [EnumMember(Value = "timestamp")]
        Timestamp,
    }
}
