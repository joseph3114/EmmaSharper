using System;
using System.Runtime.Serialization;

namespace EmmaSharper
{
    /// <summary>Which subaccount lifecycle states to include when listing.</summary>
    /// <remarks>
    /// A quota or billing tool generally wants <see cref="All"/>: retired and pending-retirement
    /// subaccounts can still hold billable contacts, so excluding them undercounts.
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

        /// <summary>Retired subaccounts. These may still hold billable contacts.</summary>
        [EnumMember(Value = "retired")]
        Retired = 8,

        /// <summary>Every lifecycle state.</summary>
        All = Active | Trial | PendingRetirement | Retired,
    }
}
