# Changelog

All notable changes to this project are documented here.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project
adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [8.0.0] - unreleased

First release since 2023. Removes both third-party dependencies and drops every out-of-support
target framework. See [Upstream issues fixed in this fork](docs/upstream-fixes.md) for the bugs
this closes against `kylegregory/EmmaSharp` and `BinaryPatrick/EmmaSharper`.

### Security

- **Removed Newtonsoft.Json 9.0.1**, which carries a known high-severity advisory
  ([GHSA-5crp-9r3c-p9vr](https://github.com/advisories/GHSA-5crp-9r3c-p9vr)). The open-ended range
  `[9.0.1,]` offered no protection — NuGet resolves to the *lowest* version satisfying a range, so
  every consumer received the vulnerable floor.
- **Member email addresses are no longer written to logs.** The adapter logged the resolved request
  path; `GetMemberByEmail` resolves to `/{account}/members/email/someone@example.com`. Only the
  unresolved template is logged now, with the account id as a separate structured field.
- Removed a static constructor that set `ServicePointManager.SecurityProtocol`, mutating TLS
  settings for the entire host process from inside a library.

### Added

- `IEmmaAccountScopeFactory` — address many Emma subaccounts with one credential pair, without
  registering a service container per account. Closes
  [BinaryPatrick/EmmaSharper#6](https://github.com/BinaryPatrick/EmmaSharper/issues/6).
- `CancellationToken` on every provider method.
- `EmmaRateLimitException`, carrying `RetryAfter`. Emma throttles with **403** as well as 429.
- `EmmaOptions.Timeout` (default 30 seconds).
- `AddEmmaApiProviders` returns `IHttpClientBuilder`, so a resilience handler can be attached.
- Options validation — missing credentials or a non-absolute `BaseUrl` now fail fast.
- CI, CodeQL scanning, and Dependabot. The existing workflows targeted a branch named `main`, which
  does not exist in this repository, and had never run.

### Changed

- **Target frameworks are now `netstandard2.0`, `net8.0` and `net10.0`.** `netcoreapp3.1`, `net5.0`,
  `net6.0` and `net7.0` are all out of support and have been dropped. `netstandard2.0` keeps .NET
  Framework 4.6.2+ consumers working and closes
  [kylegregory/EmmaSharp#44](https://github.com/kylegregory/EmmaSharp/issues/44); the test suite
  runs on `net472` so that target is verified rather than merely compiled. Note the netstandard2.0
  leg additionally needs the `System.Text.Json` package, since none of it is in-box there.
- **Transport is `HttpClient` + `IHttpClientFactory`**, replacing RestSharp. The client is pooled
  rather than constructed per request.
- **Serialization is System.Text.Json**, replacing Newtonsoft.Json.
- Dependencies reduced from six packages to two, both `Microsoft.Extensions.*`, resolved per target
  framework so a net10.0 app no longer has Extensions 5.x/6.x pulled in transitively.
- `AddEmmaApiProviders(IConfiguration)` binds an `"Emma"` section by default rather than the
  configuration root. Pass `sectionName: null` for the previous behaviour.
- `EmmaOptions` is registered through the options pattern instead of as a transient that re-ran its
  configure delegate on every resolution.

### Fixed

- Paging requested **501** records for a 500-record page; Emma's range is inclusive
  (`end = start + 499`).
- Paging underflowed: `end - 500` was computed on a `uint`, wrapping to roughly 4.29 billion when
  `end` was below 500 and no `start` was supplied.
- Integer overflow on Emma ids. `Link.LinkId`, `ResponseClicks.LinkId`,
  `ResponseSignups.ReferingMemberId`, `WebhookEvents.WebhookEventId` and both `SignupFormId`
  properties widened from `int?` to `long?`. Closes
  [BinaryPatrick/EmmaSharper#5](https://github.com/BinaryPatrick/EmmaSharper/issues/5).
- Group and member id collections widened from `int` to `long` on `AddMembersToGroup`,
  `RemoveMembersFromGroup`, `AddMemberToGroups`, `RemoveMemberFromGroups` and the `GroupIds`
  properties. Closes [kylegregory/EmmaSharp#48](https://github.com/kylegregory/EmmaSharp/issues/48).
- The `net6.0` dependency block pinned `Microsoft.Extensions.Configuration` to `[5.0,6)` by
  copy-paste.
- The README's dependency-injection sample did not compile.

### Removed

- **BREAKING** — RestSharp and Newtonsoft.Json.
- **BREAKING** — `EmmaException.Response` (RestSharp's `IRestResponse`). Replaced by typed
  `StatusCode`, `ResponseBody`, `Method` and `Resource` properties.
- **BREAKING** — `IEmmaApiAdapter` and `IEmmaRestClientFactory` are no longer public.

### Migrating from 7.x

| 7.x | 8.0.0 |
|---|---|
| `catch (EmmaException ex) { ex.Response.StatusCode }` | `ex.StatusCode` |
| `ex.Message.Contains("403")` | `catch (EmmaRateLimitException ex)` |
| `IMemberProvider` | `IEmmaMemberProvider` *(unchanged from 7.x; note the `Emma` prefix)* |
| Credentials at the root of `appsettings.json` | Under an `"Emma"` section, or pass `sectionName: null` |
| `int` member and group ids | `long` |
| `await members.GetMemberCount()` | `await members.GetMemberCount(cancellationToken: ct)` |

---

## [7.0.1] - 2023-06-22

Retarget and version bump; added `net7.0`. Published by
[BinaryPatrick](https://github.com/BinaryPatrick).

## [1.6.0] - 2022-09-28

## [1.0.1] - 2021-11-03

First release of this fork, targeting .NET Core 3.1 / .NET 5.

## Earlier

Released as [`EmmaSharp`](https://www.nuget.org/packages/EmmaSharp/) by
[kylegregory](https://github.com/kylegregory), through 1.3.0 (2019-06-05).

[8.0.0]: https://github.com/joseph3114/EmmaSharper/releases/tag/v8.0.0
[7.0.1]: https://www.nuget.org/packages/EmmaSharper/7.0.1
[1.6.0]: https://www.nuget.org/packages/EmmaSharper/1.6.0
[1.0.1]: https://www.nuget.org/packages/EmmaSharper/1.0.1
