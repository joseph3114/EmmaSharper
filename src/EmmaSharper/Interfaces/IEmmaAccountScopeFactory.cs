namespace EmmaSharper
{
    /// <summary>
    /// Creates provider sets bound to a specific Emma account, using the configured credentials.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emma enterprise accounts authenticate once and then address many subaccounts. A scope
    /// reuses the configured credentials and the same pooled HttpClient, changing only the
    /// account segment of the request path, so creating one per subaccount is cheap.
    /// </para>
    /// <example>
    /// <code>
    /// foreach (Subaccount sub in await enterprise.ListSubaccounts(cancellationToken: ct))
    /// {
    ///     IEmmaAccountScope scope = scopeFactory.ForAccount(sub.AccountId!);
    ///     int active = await scope.Members.GetMemberCount(cancellationToken: ct);
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public interface IEmmaAccountScopeFactory
    {
        /// <summary>Returns providers that address <paramref name="accountId"/>.</summary>
        /// <param name="accountId">The Emma account or subaccount id to target.</param>
        /// <remarks>Cheap - no new HTTP client or handler is created.</remarks>
        IEmmaAccountScope ForAccount(string accountId);
    }
}
