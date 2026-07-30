# Getting Started

## Install

```cmd
dotnet add package EmmaSharper
```

## Credentials

Emma issues a **public key** and a **private key** per account, plus an **account id**. All three
come from the Emma UI under account settings. The library sends the keys as HTTP Basic auth; you
never construct the header yourself.

## Register with dependency injection

```csharp
using EmmaSharper;

builder.Services.AddEmmaApiProviders(options =>
{
    options.AccountId = "your account id";
    options.PublicKey = "your public key";
    options.SecretKey = "your secret key";
    // options.BaseUrl defaults to https://api.e2ma.net
    // options.Timeout defaults to 30 seconds
});
```

Or bind from configuration — this reads the **`Emma`** section:

```csharp
builder.Services.AddEmmaApiProviders(builder.Configuration);
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

> To bind the configuration root instead of a section — keys at the top level of
> `appsettings.json` — pass `sectionName: null`.

Keep the secret key out of source control — user secrets in development, and your platform's
secret store in production.

## Make a call

Inject whichever provider you need:

```csharp
public sealed class MemberReport(IEmmaMemberProvider members)
{
    public async Task<int> CountActiveAsync(CancellationToken ct)
        => await members.GetMemberCount(status: MemberStatusShort.Active, cancellationToken: ct);
}
```

Most parameters are optional, so name the ones you pass — including `cancellationToken`, since it
is the trailing parameter on every method.

## Validation happens early

`AddEmmaApiProviders` validates on first resolve rather than failing on the first HTTP call:

- `PublicKey` and `SecretKey` must be present
- `BaseUrl` must be an absolute URI

A missing key throws `OptionsValidationException` at startup, not a confusing `401` an hour later.

## What you get

One pooled `HttpClient` via `IHttpClientFactory`, shared across every provider and every account
scope. Providers are transient and stateless; the client and its handler are managed by the
factory, so there is no socket exhaustion and no stale DNS.

## Next

- Addressing more than one account → **[Enterprise and Multiple Accounts](Enterprise-and-Multiple-Accounts)**
- Fetching more than 500 records → **[Paging](Paging)**
- Before you run anything at volume → **[Rate Limiting and Resilience](Rate-Limiting-and-Resilience)**
