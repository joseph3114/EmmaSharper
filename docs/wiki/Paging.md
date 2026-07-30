# Paging

Endpoints that return lists take `start` and `end`.

> **They are inclusive record indices, not page numbers.** A 500-record page is
> `start: 0, end: 499` — not `end: 500`.

500 is the page size in use here, and the library's paging helper assumes it: omit `end` and it
fills in `start + 499`.

```csharp
const int PageSize = 500;

var all = new List<Member>();
uint start = 0;

while (true)
{
    IEnumerable<Member> page = await members.ListMembers(
        start: start,
        end: start + PageSize - 1,      // inclusive: 0..499, then 500..999
        status: MemberStatusShort.Active,
        fields: MemberFieldSelection.ExcludeCustomFields,
        cancellationToken: ct);

    var batch = page.ToList();
    all.AddRange(batch);

    if (batch.Count < PageSize)
    {
        break;                          // short page means the end
    }

    start += PageSize;
}
```

Omit both and you get the first page.

## Know the total first

```csharp
int total = await members.GetMemberCount(status: MemberStatusShort.Active, cancellationToken: ct);
```

Useful for a progress bar, and cheap — a single call returning a bare integer. Don't rely on it as
a loop bound though: members can be added or removed while you page, so the short-page check above
is what actually terminates the loop correctly.

## Exclude custom fields

If you only need identity, say so:

```csharp
fields: MemberFieldSelection.ExcludeCustomFields
```

This maps to Emma's `exclude_fields=1`. On an account with hundreds of thousands of members and
dozens of custom fields per member, it is the single biggest throughput lever available — it is
the difference between a sync that takes minutes and one that takes an hour.

## Getting the bounds wrong

Two mistakes are easy to make when computing ranges yourself, and both are silent:

- Treating the window as a **count** rather than an inclusive end, so `start + 500` asks for 501
  records against an API whose maximum is 500.
- Computing `end - 500` on a `uint`, which **wraps** to roughly 4.29 billion when `end` is below
  500.

Pass `start` and `end` and the library handles the arithmetic.
