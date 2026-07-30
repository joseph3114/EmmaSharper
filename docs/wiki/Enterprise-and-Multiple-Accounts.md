# Enterprise and Multiple Accounts

An Emma **enterprise** account owns subaccounts — one per department, campus, brand, whatever the
organisation divides on. You authenticate once with the enterprise credentials and then address
each subaccount by id.

## Discover the subaccounts

```csharp
public sealed class QuotaSweep(
    IEmmaEnterpriseProvider enterprise,
    IEmmaAccountScopeFactory scopeFactory,
    ILogger<QuotaSweep> logger)
{
    public async Task RunAsync(CancellationToken ct)
    {
        IReadOnlyList<Subaccount> subaccounts = await enterprise.ListSubaccounts(cancellationToken: ct);

        foreach (Subaccount sub in subaccounts)
        {
            IEmmaAccountScope scope = scopeFactory.ForAccount(sub.AccountId!);

            int active = await scope.Members.GetMemberCount(
                status: MemberStatusShort.Active,
                cancellationToken: ct);

            logger.LogInformation("{Name} ({Id}): {Active} active", sub.AccountName, sub.AccountId, active);
        }
    }
}
```

> `ListSubaccounts` defaults to **every** status, not just active. Retired and pending-retirement
> subaccounts can still hold billable contacts, so filtering them out undercounts. Narrow it
> deliberately if that is what you want:
>
> ```csharp
> await enterprise.ListSubaccounts(SubaccountStatusFilter.Active | SubaccountStatusFilter.Trial, ct);
> ```

## What a scope is

`ForAccount(id)` returns the full provider set bound to that account. It reuses the same
credentials and the **same pooled `HttpClient`** — only the account segment of the request path
changes. Creating one is a handful of field assignments; it opens no connection.

```csharp
IEmmaAccountScope scope = scopeFactory.ForAccount("1234");

scope.AccountId      // "1234"
scope.Members        // IEmmaMemberProvider, targeting 1234
scope.Groups         // IEmmaGroupProvider, targeting 1234
scope.Account        // IEmmaAccountProvider, targeting 1234
// ...every provider, same shape as the injected ones
```

Scopes are independent, so making 47 of them is fine.

## Running in parallel

Scopes share the pooled client, which is safe to use concurrently. Bound the parallelism — Emma
rate-limits, and more concurrency mostly buys you more 403s:

```csharp
await Parallel.ForEachAsync(
    subaccounts,
    new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = ct },
    async (sub, token) =>
    {
        IEmmaAccountScope scope = scopeFactory.ForAccount(sub.AccountId!);
        await ProcessAsync(scope, token);
    });
```

A degree of 4 is a reasonable starting point for a sweep of tens of subaccounts. Raise it only
with [resilience configured](Rate-Limiting-and-Resilience), and watch for throttling.

> If you resolve scoped services inside the loop — an `IServiceScope`, a `DbContext` — create a
> **new scope per task**. `DbContext` is not thread-safe, and sharing one across parallel tasks
> produces *"A second operation was started on this context instance."*

## Users on a subaccount

```csharp
IReadOnlyList<AccountUser> users = await scope.Account.ListUsers(ct);

foreach (AccountUser user in users)
{
    // CreatedAt and LastLoginAttempt are DateTime?, not string - so this sorts.
    Console.WriteLine($"{user.Email} {user.Role} last seen {user.LastLoginAttempt:d}");
}
```

## Fields Emma returns that aren't modelled

`Subaccount` and `AccountUser` both carry `AdditionalData`. Emma publishes no complete schema for
these endpoints and the payload varies by plan, so anything unmapped is captured rather than
dropped:

```csharp
if (sub.AdditionalData?.TryGetValue("contact_limit", out JsonElement limit) == true)
{
    int contactLimit = limit.GetInt32();
}
```

If you find a field that is consistently present, please
[open an issue](https://github.com/joseph3114/EmmaSharper/issues) so it can be modelled properly.
