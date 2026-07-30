# EmmaSharper

[![CI](https://github.com/joseph3114/EmmaSharper/actions/workflows/test.yml/badge.svg)](https://github.com/joseph3114/EmmaSharper/actions/workflows/test.yml)
[![NuGet](https://img.shields.io/nuget/v/EmmaSharper.svg)](https://www.nuget.org/packages/EmmaSharper/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.txt)

A .NET client for the [Emma (Marigold) API](https://api.myemma.com/).

> **This is the maintained continuation of [`kylegregory/EmmaSharp`](https://github.com/kylegregory/EmmaSharp)**,
> which last shipped in 2019, by way of [`BinaryPatrick/EmmaSharper`](https://github.com/BinaryPatrick/EmmaSharper).
> Several bugs still shown as open on those repositories are fixed here —
> see **[Upstream issues fixed in this fork](docs/upstream-fixes.md)**.

**Targets:** `netstandard2.0`, `net8.0`, `net10.0` — so .NET Framework 4.6.2+ works too.
**Dependencies:** two on the modern targets, both `Microsoft.Extensions.*`.

---

## Install

```cmd
dotnet add package EmmaSharper
```

## Quick start

```csharp
using EmmaSharper;

services.AddEmmaApiProviders(options =>
{
    options.AccountId = "your account id";
    options.PublicKey = "your public key";
    options.SecretKey = "your secret key";
    // options.BaseUrl defaults to https://api.e2ma.net
});
```

Or bind from configuration — this reads the `"Emma"` section:

```csharp
services.AddEmmaApiProviders(builder.Configuration);
```

```json
{
  "Emma": {
    "AccountId": "your account id",
    "PublicKey": "your public key",
    "SecretKey": "your secret key"
  }
}
```

> Pass `sectionName: null` to bind the configuration root instead, which is how 7.x behaved.

Then inject any provider:

```csharp
public sealed class MemberSync(IEmmaMemberProvider members)
{
    public async Task<int> CountActiveAsync(CancellationToken ct)
        => await members.GetMemberCount(cancellationToken: ct);
}
```

## Working with multiple accounts

Emma enterprise accounts authenticate once and then address many subaccounts. Use
`IEmmaAccountScopeFactory` rather than registering a container per account — a scope reuses the same
credentials and the same pooled `HttpClient`, changing only the account segment of the request path.

```csharp
public sealed class QuotaSweep(IEmmaAccountScopeFactory scopeFactory)
{
    public async Task RunAsync(IEnumerable<string> subaccountIds, CancellationToken ct)
    {
        foreach (string accountId in subaccountIds)
        {
            IEmmaAccountScope scope = scopeFactory.ForAccount(accountId);
            int active = await scope.Members.GetMemberCount(cancellationToken: ct);
        }
    }
}
```

## Rate limiting

**Emma signals throttling with `403 Forbidden` as well as the conventional `429`.** This is the
least obvious behaviour in the API — a naive client reads the 403 as an auth failure and gives up
instead of backing off.

Both map to `EmmaRateLimitException`, which carries `RetryAfter` when Emma supplies it:

```csharp
try
{
    await members.GetMemberCount(cancellationToken: ct);
}
catch (EmmaRateLimitException ex)
{
    await Task.Delay(ex.RetryAfter ?? TimeSpan.FromSeconds(5), ct);
}
```

`AddEmmaApiProviders` returns the `IHttpClientBuilder`, so you can attach a resilience handler
instead of catching:

```csharp
services.AddEmmaApiProviders(configuration)
        .AddStandardResilienceHandler();
```

> If you do, raise the per-attempt timeout. The standard handler defaults to 10 seconds, which is
> not enough to fetch a 500-record member page.

## Errors

Every non-success response raises `EmmaException` with typed detail — no string matching required:

```csharp
catch (EmmaException ex)
{
    logger.LogError("Emma {Status} on {Method} {Resource}: {Body}",
        ex.StatusCode, ex.Method, ex.Resource, ex.ResponseBody);
}
```

## Providers

| Interface | Covers |
|---|---|
| `IEmmaAutomationProvider` | Automation workflows |
| `IEmmaFieldsProvider` | Custom member fields, including `ClearField` to reset a single field across every member |
| `IEmmaGroupProvider` | Groups and bulk group membership |
| `IEmmaMailingProvider` | Mailings, their HTML, recipients; pausing and cancelling |
| `IEmmaMemberProvider` | Members, statuses, and bulk imports — prefer the bulk calls over looping |
| `IEmmaResponseProvider` | Mailing response data, down to who opened what |
| `IEmmaSearchProvider` | Saved searches and their matching members |
| `IEmmaSignupFormProvider` | Sign-up forms |
| `IEmmaSubscriptionProvider` | Subscriptions and subscription members |
| `IEmmaWebhookProvider` | Webhooks |

All methods are asynchronous and accept a trailing `CancellationToken`.

### Paging

Endpoints that page take `start` and `end`. Emma's range is **inclusive**, so a 500-record page is
`end = start + 499`. Omit both and you get the first page.

## Versioning

`8.0.0` is a breaking release — see the [changelog](CHANGELOG.md). The short version: RestSharp and
Newtonsoft.Json removed, `EmmaException` no longer exposes a RestSharp type, ids widened from `int`
to `long`, and every method gained a `CancellationToken`.

## Contributing

This project is not affiliated with [Emma](http://myemma.com/meet-us). Everyone working on it is a
volunteer. [Fork the repo](https://help.github.com/articles/fork-a-repo), make your changes, and
open a pull request — CI runs build, tests on both target frameworks, and CodeQL.

Emma's own API documentation is at <https://api.myemma.com/>.
