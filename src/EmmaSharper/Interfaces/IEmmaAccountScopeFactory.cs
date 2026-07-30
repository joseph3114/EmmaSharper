namespace EmmaSharper
{
    /// <summary>
    /// Creates provider sets bound to a specific Emma account, using the configured credentials.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emma enterprise accounts authenticate once and then address many subaccounts. Before 8.0.0
    /// the account id was fixed at DI registration and no provider method accepted one, so a
    /// consumer could not iterate subaccounts without registering a container per account.
    /// </para>
    /// <example>
    /// <code>
    /// foreach (Subaccount sub in await enterprise.ListSubaccountsAsync(ct))
    /// {
    ///     IEmmaAccountScope scope = scopeFactory.ForAccount(sub.AccountId);
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
