# Upstream issues fixed in this fork

EmmaSharper is the maintained continuation of [`kylegregory/EmmaSharp`](https://github.com/kylegregory/EmmaSharp)
(last release 2019) by way of [`BinaryPatrick/EmmaSharper`](https://github.com/BinaryPatrick/EmmaSharper)
(last release 2023).

Several bugs reported against those repositories are still shown as **open** there, because neither
is actively maintained. This page records which of them are resolved here, what actually caused
them, and which version carries the fix.

If you arrived from a search for one of the error messages below: the fix is in
[`EmmaSharper` on NuGet](https://www.nuget.org/packages/EmmaSharper/) **8.0.0** or later.

---

## Fixed

### `Link_ID trips an integer overflow error`
**[BinaryPatrick/EmmaSharper#5](https://github.com/BinaryPatrick/EmmaSharper/issues/5)** · reported 2024-02-27 · fixed in **8.0.0**

> When pulling in click info, link_id sometimes trips an int overflow error, due to the size of the ids being pulled in.

**Cause.** `Link.LinkId` and `ResponseClicks.LinkId` were typed `int?`. Emma link ids exceed
`Int32.MaxValue`.

**Fix.** Both widened to `long?`. Investigating the report showed it was not isolated — an
`int32 → int64` sweep had been done upstream in 2019 but missed several fields. Four more were
widened at the same time:

| Property | Note |
|---|---|
| `ResponseSignups.ReferingMemberId` | A *member* id, four lines from `MemberId` which was already `long?`. Unreported, same exposure. |
| `WebhookEvents.WebhookEventId` | |
| `SignupForm.SignupFormId` | |
| `SignupMember.SignupFormId` | |

Commit `2a164f1`. The reporter suggested `string`; `long` was chosen instead to match how every
other Emma id in the library is already typed.

---

### `RemoveMembersFromGroup api call fails when returned ids too big for int32`
**[kylegregory/EmmaSharp#48](https://github.com/kylegregory/EmmaSharp/issues/48)** · reported 2019-12-06 · fixed in **8.0.0**

```
Newtonsoft.Json.JsonReaderException: JSON integer 2169469051 is too large or small for an Int32.
Path '[0]', line 2, position 12.
   at EmmaSharp.EmmaApi.RemoveMembersFromGroup(String memberGroupId, MemberIdList memberIds)
```

**Cause.** Exactly as reported. Worth noting the *request* side had already been corrected —
`MemberIdList.MemberIds` has been `List<long>` for years — so only the **returned** member ids
overflowed, which is why the bug survived so long.

**Fix.** Return types widened to `long` on `AddMembersToGroup`, `RemoveMembersFromGroup`,
`AddMemberToGroups` and `RemoveMemberFromGroups`, along with the `GroupIds` properties on
`AddMember`, `AddMembers`, `RemoveMemberGroups` and `SignupMember`. Commit `6bab5c5`.

---

### `multiple accounts`
**[BinaryPatrick/EmmaSharper#6](https://github.com/BinaryPatrick/EmmaSharper/issues/6)** · reported 2024-10-03 · fixed in **8.0.0**

> I was just wondering if there was a way to use the api with two different accountid's? I have a need to loop through and send the same data but to two different accounts.

**Cause.** The account id was bound once at DI registration and no provider method accepted one, so
addressing a second account meant building a second service container.

**Fix.** `IEmmaAccountScopeFactory`:

```csharp
foreach (string accountId in accountIds)
{
    IEmmaAccountScope scope = scopeFactory.ForAccount(accountId);
    int active = await scope.Members.GetMemberCount(cancellationToken: ct);
}
```

A scope reuses the same credentials and the same pooled `HttpClient` — it only changes the account
segment of the request path. Commit `cbc4ae8`.

---

### `.NET Core/5/6`
**[kylegregory/EmmaSharp#55](https://github.com/kylegregory/EmmaSharp/issues/55)** · reported 2022-10-20 · fixed

Kyle's `EmmaSharp` targeted .NET Framework 4.6. This fork has been .NET Core / .NET 5+ since 1.0.1,
and as of 8.0.0 targets **net8.0** and **net10.0**.

---

### `Subscription endpoint availability?`
**[kylegregory/EmmaSharp#51](https://github.com/kylegregory/EmmaSharp/issues/51)** · reported 2020-06-17 · fixed

Emma's subscriptions endpoints are implemented — see `IEmmaSubscriptionProvider`.

---

### `Target .Net Standard 2.0`
**[kylegregory/EmmaSharp#44](https://github.com/kylegregory/EmmaSharp/issues/44)** · reported 2019-06-06 · fixed in **8.0.0**

> It would be helpful to update the library to target .net standard 2.0 for improved compatibility.

**Fix.** `netstandard2.0` is a target framework as of 8.0.0, bringing .NET Framework 4.6.2+
consumers back. This was initially scoped out and then reversed once the cost was measured rather
than assumed — compiling the source against netstandard2.0 produced only **four** API gaps, three
of which need no conditional compilation at all:

| Missing on netstandard2.0 | Resolution |
|---|---|
| `HttpStatusCode.TooManyRequests` | `(HttpStatusCode)429` |
| `string.Join(char, …)` | `string.Join(separator.ToString(), …)` |
| `Enum.IsDefined<T>(T)` | `Enum.IsDefined(typeof(T), boxed)` |
| `HttpContent.ReadAsStringAsync(CancellationToken)` | the one `#if NETSTANDARD2_0` in the codebase |

`JsonNamingPolicy.SnakeCaseLower`, `Utf8JsonWriter.WriteRawValue` and `JsonConverterFactory` all
work, because the `System.Text.Json` package backports them — so serialization behaves identically
on every target.

The test suite also runs on `net472`, so this leg is verified against a real .NET Framework runtime
rather than merely compiled.

Two caveats worth knowing: netstandard2.0 defaults to C# 7.3 and cannot parse
`<Nullable>enable</Nullable>`, so `LangVersion` is pinned to 12 for that framework only; and the
"two dependencies" figure holds for net8.0/net10.0 — the netstandard2.0 leg additionally needs
`System.Text.Json`, since none of it is in-box there.

---

## Fixed but never reported

These were found while working on the issues above.

| Defect | Detail |
|---|---|
| **Inclusive paging off-by-one** | Page bounds were computed as `start + 500`, requesting **501** records. Emma's range is inclusive, so a 500-record page ends at `start + 499`. |
| **`uint` underflow in paging** | `end - 500` was evaluated on a `uint`; if `end` was below 500 and no `start` was given it wrapped to roughly 4.29 billion. |
| **Member email addresses in logs** | The adapter logged the *resolved* request path. `GetMemberByEmail` resolves to `/{account}/members/email/someone@example.com`, so real addresses were written to application logs. Now only the unresolved template is logged. Caught by CodeQL; regression test in `TEmmaApiAdapterLogging`. |
| **Socket exhaustion** | A new HTTP client was constructed per request from a transient factory. Now pooled through `IHttpClientFactory`. |
| **Process-wide TLS mutation** | A static constructor set `ServicePointManager.SecurityProtocol`, changing TLS settings for the entire host process from inside a library — and a no-op on .NET Core since 3.0. Removed. |
| **403 treated as an auth failure** | Emma signals throttling with **403** as well as 429. Both now raise `EmmaRateLimitException` carrying `Retry-After`. |
| **Known-vulnerable dependency** | Newtonsoft.Json 9.0.1 carries [GHSA-5crp-9r3c-p9vr](https://github.com/advisories/GHSA-5crp-9r3c-p9vr). The open-ended range `[9.0.1,]` did not help: NuGet resolves to the *lowest* version in range, so every consumer received the vulnerable floor. Newtonsoft has been removed entirely. |

---

## Reporting something new

Bugs against this fork belong in [our issue tracker](https://github.com/joseph3114/EmmaSharper/issues).
The upstream repositories are not monitored.
