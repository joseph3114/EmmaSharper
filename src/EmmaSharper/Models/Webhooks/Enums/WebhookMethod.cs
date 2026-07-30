using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <summary>Webhook method enumeration</summary>
    [JsonConverter(typeof(StringEnumJsonConverter))]
    public enum WebhookMethod
    {
        Unknown,

        /// <summary>
        /// Webhook uses HTTP GET
        /// </summary>
        [EnumMember(Value = "GET")]
        Get,

        /// <summary>
        /// Webhook uses HTTP POST
        /// </summary>
        [EnumMember(Value = "POST")]
        Post,
    }
}
