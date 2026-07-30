# Emma API Notes

Behaviour of the Emma API **observed while building and running against it**. Emma's
[documentation](https://api.myemma.com/) is a page of links with one page per endpoint and does
not mention most of this.

Everything here is something we have actually seen. Where Emma's intent or contractual behaviour
is unknown, it is left out rather than guessed at — so treat this as field notes, not a
specification.

---

## 403 has been observed as a throttle response

Under sustained load, Emma has been seen returning `403 Forbidden` where `429` would be expected,
with the request succeeding on retry after a backoff. A client that treats every 403 as an
authentication failure will stop when it should have waited.

A genuine credentials failure also returns 403, and the two are not distinguishable without
inspecting the response body — so this is a heuristic, not a contract. See
[Rate Limiting and Resilience](Rate-Limiting-and-Resilience) for how the library classifies it and
how to opt out.

---

## Response shapes are inconsistent

Three different shapes across endpoints, and assuming any one of them universally breaks the
others:

| Endpoint | Shape |
|---|---|
| `enterprise/subaccounts` | object-wrapped — `{"subaccounts": [...]}` |
| `accounts/users` | object-wrapped — `{"users": [...]}` |
| `members` | **bare array** — `[ {...}, {...} ]` |
| `members?count=true` | **bare integer** — `2431`, not even a JSON object |

A generic "unwrap the envelope" helper will work on half the API and fail on the other half. The
library handles each envelope where it actually occurs.

---

## Dates sometimes carry a `@D:` prefix

```
"created_at": "@D:2014-11-26T11:40:55"
```

Not universal — some fields are plain ISO 8601. The library's date converter strips the prefix if
present and parses either form, using invariant culture. If you parse Emma dates yourself, handle
both.

Note there is no timezone in that format.

---

## Ids exceed Int32

Mailing ids, member ids and link ids have all grown past `Int32.MaxValue`. The classic symptom:

```
JSON integer 2169469051 is too large or small for an Int32
```

Every id in this library is `long`. If you parse Emma ids yourself, use a 64-bit type.

---

## Paging ranges are inclusive

`start` and `end` are record indices, not page numbers, and both ends are included. A 500-record
page is `0` to `499`. Maximum page size is 500. See [Paging](Paging).

---

## Unrecognised enum values deserialize to `Unknown`

Every enum in this library has an `Unknown` member. A value the library does not model
deserializes to it rather than throwing, so an unfamiliar status does not fail the whole response.

Check for `Unknown` if that distinction matters to your logic. Because it has no wire
representation, it cannot be used as a filter — passing it throws.

---

## Counting by status

`enterprise/subaccounts` takes a multi-valued `status` parameter. The accepted values are
`active`, `trial`, `pending_retirement` and `retired`. `ListSubaccounts` sends all four unless you
narrow it:

```csharp
await enterprise.ListSubaccounts(SubaccountStatusFilter.Active, ct);
await enterprise.ListSubaccounts(
    SubaccountStatusFilter.Active | SubaccountStatusFilter.Trial, ct);
```

Member status is a separate axis, filtered on the member call. `count=true` returns the total
without fetching the records:

```csharp
int active = await scope.Members.GetMemberCount(status: MemberStatusShort.Active, cancellationToken: ct);
int optout = await scope.Members.GetMemberCount(status: MemberStatusShort.Optout, cancellationToken: ct);
int error  = await scope.Members.GetMemberCount(status: MemberStatusShort.Error, cancellationToken: ct);
int all    = await scope.Members.GetMemberCount(cancellationToken: ct);
```

Deleted members are a third, independent flag — `includeDeleted: true` — not a member status.

---

## Unmodelled fields are preserved

`Subaccount` and `AccountUser` carry a `[JsonExtensionData]` dictionary, so any property Emma
returns that this library does not model is available rather than discarded:

```csharp
if (sub.AdditionalData?.TryGetValue("contact_limit", out JsonElement limit) == true)
{
    int contactLimit = limit.GetInt32();
}
```

If you find a field consistently present on your account, please
[open an issue](https://github.com/joseph3114/EmmaSharper/issues) so it can be typed.
