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

Every subaccount status is included on purpose — retired subaccounts can still hold billable
contacts.

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

## Rename a member without losing their history

Emma has **no merge endpoint**. But `PUT /members/{id}` can change an email address in place, and
because the `member_id` survives, the member's mailing history stays attached. This is the basis
for collapsing duplicate aliases, where the same person is enrolled under several addresses and
billed for each.

```csharp
Member? alias = await scope.Members.GetMemberByEmail("j.smith@example.edu", cancellationToken: ct);
Member? canonical = await scope.Members.GetMemberByEmail("john.smith@example.edu", cancellationToken: ct);

if (alias is not null && canonical is null)
{
    // Nothing to collide with - rename in place and keep the history.
    await scope.Members.UpdateSingleMemberInformation(
        alias.MemberId!.Value.ToString(),
        new UpdateMember { MemberEmail = "john.smith@example.edu" },
        cancellationToken: ct);
}
else if (alias is not null && canonical is not null)
{
    // Both exist. Keep whichever is more engaged, archive the other.
    int aliasHistory = await scope.Members.GetMemberMailingHistoryCount(
        alias.MemberId!.Value.ToString(), ct);
    int canonicalHistory = await scope.Members.GetMemberMailingHistoryCount(
        canonical.MemberId!.Value.ToString(), ct);

    Member loser = aliasHistory > canonicalHistory ? canonical : alias;
    await scope.Members.DeleteMember(loser.MemberId!.Value.ToString(), ct);
}
```

`DeleteMember` archives rather than hard-deleting, so this is recoverable. Verify the behaviour
against a sandbox subaccount before running it across a real list.

---

## What this replaces

The sync these recipes come from hand-rolled four Emma calls. Each now has a direct equivalent:

| Hand-rolled | Library |
|---|---|
| `GET /{acct}/enterprise/subaccounts?status=active,trial,pending_retirement,retired` | `enterprise.ListSubaccounts()` |
| `GET /{sub}/members?count=true&filter=["member_status_id","eq","a"]` | `scope.Members.GetMemberCount(status: Active)` |
| `GET /{sub}/members?filter=[…]&exclude_fields=1&start=&end=` | `scope.Members.ListMembers(status: Active, fields: ExcludeCustomFields, start, end)` |
| `GET /{sub}/accounts/users` | `scope.Account.ListUsers()` |

Along with the Basic auth header, the retry loop, the `403`-means-throttled special case, the
`JObject` parsing, and four private DTOs — roughly 600 lines of the 757-line service.
