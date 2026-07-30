using System;
using System.Runtime.Serialization;

namespace EmmaSharper
{
    /// <summary>Which subaccount lifecycle states to include when listing.</summary>
    /// <remarks>
    /// Defaults to <see cref="All"/>. Narrowing the filter changes which subaccounts you
    /// enumerate and therefore any total computed across them, so choose deliberately rather than
    /// assuming <see cref="Active"/> is what you want.
    /// </remarks>
    [Flags]
    public enum SubaccountStatusFilter
    {
        /// <summary>Active subaccounts.</summary>
        [EnumMember(Value = "active")]
        Active = 1,

        /// <summary>Subaccounts still in trial.</summary>
        [EnumMember(Value = "trial")]
        Trial = 2,

        /// <summary>Subaccounts scheduled for retirement.</summary>
        [EnumMember(Value = "pending_retirement")]
        PendingRetirement = 4,

        /// <summary>Retired subaccounts. These may still contain member records.</summary>
        [EnumMember(Value = "retired")]
        Retired = 8,

        /// <summary>Every lifecycle state.</summary>
        All = Active | Trial | PendingRetirement | Retired,
    }
}
