using System;

namespace EmmaSharper.Internals
{
    /// <summary>Builds Emma's <c>filter</c> query expressions.</summary>
    /// <remarks>
    /// Emma's wire format is a JSON array, e.g. <c>filter=["member_status_id","eq","a"]</c>. The
    /// typed overloads cover the common case; a raw string escape hatch keeps the fuller filter
    /// grammar reachable rather than walling it off behind the wrapper.
    /// </remarks>
    internal static class EmmaFilter
    {
        internal const string MemberStatusField = "member_status_id";

        /// <summary>Renders an equality filter.</summary>
        internal static string EqualsExpression(string field, string value)
            => "[\"" + field + "\",\"eq\",\"" + value + "\"]";

        /// <summary>
        /// Resolves a member-status filter, a caller-supplied raw expression, or neither.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Both a typed status and a raw filter were supplied, or the status was
        /// <see cref="MemberStatusShort.Unknown"/>, which has no wire representation.
        /// </exception>
        internal static string? ResolveMemberFilter(MemberStatusShort? status, string? rawFilter)
        {
            bool hasRaw = !string.IsNullOrWhiteSpace(rawFilter);

            if (hasRaw && status.HasValue)
            {
                throw new ArgumentException(
                    "Supply either a status or a raw filter, not both - Emma accepts a single " +
                    "filter expression. Fold the status into the raw filter if you need both.",
                    nameof(rawFilter));
            }

            if (hasRaw)
            {
                return rawFilter;
            }

            if (!status.HasValue)
            {
                return null;
            }

            if (status.Value == MemberStatusShort.Unknown)
            {
                throw new ArgumentException(
                    $"{nameof(MemberStatusShort)}.{nameof(MemberStatusShort.Unknown)} is a " +
                    "fall-back for values Emma has added but this library does not model yet. " +
                    "It cannot be used as a filter; pass null for no filtering.",
                    nameof(status));
            }

            return EqualsExpression(MemberStatusField, status.Value.ToEnumString());
        }
    }
}
