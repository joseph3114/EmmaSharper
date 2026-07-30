# Emma API Notes

Behaviour of the Emma API itself, learned by hitting it. Emma's
[documentation](https://api.myemma.com/) is a page of links with one page per endpoint and does
not mention most of this. Recorded here so the next person doesn't have to rediscover it.

---

## 403 means rate-limited

Not "forbidden". Emma throttles with `403` as well as the conventional `429`.

A client written against normal expectations treats the 403 as an auth failure and stops. The
correct response is to back off and retry. See
[Rate Limiting and Resilience](Rate-Limiting-and-Resilience).

A genuine credentials failure *also* returns 403, and the two are not distinguishable without
inspecting the response body.

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

## Enum values appear without warning

Emma adds new status and type values without notice. A client that throws on an unrecognised enum
value breaks the day that happens.

Every enum here has an `Unknown` member, and deserialization falls back to it rather than
throwing. Check for `Unknown` if an unrecognised value matters to your logic.

Because `Unknown` has no wire representation, it cannot be used as a filter — passing it throws.

---

## Retired subaccounts still hold contacts

For anything counting billable contacts, `status=active` undercounts. Retired and
pending-retirement subaccounts can still contain members. `ListSubaccounts` includes every status
by default for this reason.

---

## Subaccount payloads vary

`enterprise/subaccounts` returns different fields depending on plan, and Emma publishes no schema
for it. `Subaccount.AdditionalData` captures whatever is not modelled — plan and quota fields have
been seen there and are exactly what a billing tool wants.

---

## There is no merge endpoint

Duplicate members cannot be merged. But `PUT /{accountId}/members/{memberId}` **can change an
email address in place**, and the `member_id` survives, so mailing history stays attached. That is
the only route to collapsing duplicate aliases. `POST` cannot do this, because it identifies
members *by* email. See [Recipes](Recipes).

---

## Some documented endpoints are broken

Emma's own issue tracker has long-standing reports that creating and updating **saved searches**
returns `{"error": "'unicode' object has no attribute 'pop'"}` — a server-side fault, not a client
one. If a documented endpoint returns something structurally bizarre, suspect Emma before
suspecting your payload.
