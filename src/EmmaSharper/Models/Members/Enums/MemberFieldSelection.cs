namespace EmmaSharper
{
    /// <summary>How much of each member record to ask Emma for.</summary>
    /// <remarks>
    /// The single biggest throughput lever on a large account. Pulling every custom field for
    /// every member is the difference between a fast sync and a hostile one when the account
    /// holds hundreds of thousands of members and only the email and id are wanted.
    /// </remarks>
    public enum MemberFieldSelection
    {
        /// <summary>Return the full member record, including custom fields.</summary>
        All = 0,

        /// <summary>Omit custom fields. Maps to Emma's <c>exclude_fields=1</c>.</summary>
        ExcludeCustomFields = 1,
    }
}
