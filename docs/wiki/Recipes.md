# Recipes

Whole tasks rather than isolated calls. These are taken from a production quota sync that ran
against Emma with hand-rolled HTTP before this library could express the workload — roughly 47
subaccounts and 460,000 members per run.

---

## Count active members across every subaccount

The canonical enterprise sweep.

```csharp
public sealed class QuotaSweep(
    IEmmaEnterpriseProvider enterprise,
    IEmmaAccountScopeFactory scopeFactory)
{
    public async Task<Dictionary<string, int>> RunAsync(CancellationToken ct)
    {
        IReadOnlyList<Subaccount> subaccounts = await enterprise.ListSubaccounts(cancellationToken: ct);
        var counts = new ConcurrentDictionary<string, int>();

        await Parallel.ForEachAsync(
            subaccounts,
            new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = ct },
            async (sub, token) =>
            {
                IEmmaAccountScope scope = scopeFactory.ForAccount(sub.AccountId!);
                counts[sub.AccountId!] = await scope.Members.GetMemberCount(
                    status: MemberStatusShort.Active,
                    cancellationToken: token);
            });

        return counts.ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
```

`ListSubaccounts` sends all four statuses here. Pass a `SubaccountStatusFilter` to narrow it.

---

## Stream every active member of a subaccount

Identity only, so custom fields are excluded.

```csharp
static async IAsyncEnumerable<Member> StreamActiveMembersAsync(
    IEmmaAccountScope scope,
    [EnumeratorCancellation] CancellationToken ct = default)
{
    const int PageSize = 500;
    uint start = 0;

    while (true)
    {
        var page = (await scope.Members.ListMembers(
            start: start,
            end: start + PageSize - 1,
            status: MemberStatusShort.Active,
            fields: MemberFieldSelection.ExcludeCustomFields,
            cancellationToken: ct)).ToList();

        foreach (Member m in page)
        {
            yield return m;
        }

        if (page.Count < PageSize)
        {
            yield break;
        }

        start += PageSize;
    }
}
```

---

## Audit who can access each subaccount

```csharp
foreach (Subaccount sub in await enterprise.ListSubaccounts(cancellationToken: ct))
{
    IEmmaAccountScope scope = scopeFactory.ForAccount(sub.AccountId!);

    foreach (AccountUser user in await scope.Account.ListUsers(ct))
    {
        bool dormant = user.LastLoginAttempt is null
                    || user.LastLoginAttempt < DateTime.UtcNow.AddYears(-1);

        if (dormant)
        {
            logger.LogWarning("{Account}: {Email} ({Role}) last seen {Seen:d}",
                sub.AccountName, user.Email, user.Role, user.LastLoginAttempt);
        }
    }
}
```

The timestamps are `DateTime?`, so comparisons and sorting work. Keeping them as strings — which
is what a hand-rolled client tends to produce — makes a "sort by last login" grid impossible.

---

## Compare mailing engagement between two members

```csharp
Member? first = await scope.Members.GetMemberByEmail("j.smith@example.edu", cancellationToken: ct);
Member? second = await scope.Members.GetMemberByEmail("john.smith@example.edu", cancellationToken: ct);

if (first is not null && second is not null)
{
    int firstCount = await scope.Members.GetMemberMailingHistoryCount(
        first.MemberId!.Value.ToString(), ct);
    int secondCount = await scope.Members.GetMemberMailingHistoryCount(
        second.MemberId!.Value.ToString(), ct);
}
```

`GetMemberByEmail` raises an `EmmaException` with `NotFound` when no member matches — see
[Error Handling](Error-Handling) for turning that into a null.

---

## Endpoint reference

If you are porting code that calls Emma directly, these are the equivalents:

| Emma endpoint | Library |
|---|---|
| `GET /{acct}/enterprise/subaccounts?status=active,trial,pending_retirement,retired` | `enterprise.ListSubaccounts()` |
| `GET /{sub}/members?count=true&filter=["member_status_id","eq","a"]` | `scope.Members.GetMemberCount(status: Active)` |
| `GET /{sub}/members?filter=[…]&exclude_fields=1&start=&end=` | `scope.Members.ListMembers(status: Active, fields: ExcludeCustomFields, start, end)` |
| `GET /{sub}/accounts/users` | `scope.Account.ListUsers()` |
| `PUT /{sub}/members/{id}` | `scope.Members.UpdateSingleMemberInformation(id, member)` |

Basic auth, the retry loop, response parsing and the DTOs come with the library.
