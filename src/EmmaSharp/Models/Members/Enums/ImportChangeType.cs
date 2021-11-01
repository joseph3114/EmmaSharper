﻿using System.Runtime.Serialization;
using EmmaSharp.Internals;
using Newtonsoft.Json;

namespace EmmaSharp
{
    [JsonConverter(typeof(StringEnumJsonConverter))]
    public enum ImportChangeType
    {
        Unknown,

        [EnumMember(Value = "a")]
        Added,

        [EnumMember(Value = "c")]
        Confirmed,

        [EnumMember(Value = "d")]
        Deleted,

        [EnumMember(Value = "n")]
        Undeleted,

        [EnumMember(Value = "u")]
        Updated,

        [EnumMember(Value = "r")]
        UpdateRejected,

        [EnumMember(Value = "s")]
        SignedUp,

        [EnumMember(Value = "t")]
        StatusShifted,
    }
}