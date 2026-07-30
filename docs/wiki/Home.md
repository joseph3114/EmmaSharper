# EmmaSharper

A .NET client for the [Emma (Marigold) API](https://api.myemma.com/).

**Targets** `netstandard2.0`, `net8.0`, `net10.0` — so .NET Framework 4.6.2+ works too.
**Dependencies** two on the modern targets, both `Microsoft.Extensions.*`.

> **The version is not a .NET version.** `8.0.0` is the next major after `7.0.1` under
> [SemVer](https://semver.org/) — it is not "the .NET 8 build". One package supports every target
> above. Check the target frameworks, not the major.

```cmd
dotnet add package EmmaSharper
```

---

## Start here

| Page | For |
|---|---|
| **[Getting Started](Getting-Started)** | Install, register with DI, make a first call |
| **[Enterprise and Multiple Accounts](Enterprise-and-Multiple-Accounts)** | One credential pair addressing many subaccounts |
| **[Paging](Paging)** | Emma's ranges are inclusive, and that trips people up |
| **[Rate Limiting and Resilience](Rate-Limiting-and-Resilience)** | Emma throttles with **403**, not just 429 |
| **[Error Handling](Error-Handling)** | Typed exceptions instead of string-matching |
| **[Recipes](Recipes)** | Whole tasks, taken from a production sync |
| **[Emma API Notes](Emma-API-Notes)** | Behaviour of the API itself that the docs don't mention |

Upgrading from 7.x? See the migration table in the
[changelog](https://github.com/joseph3114/EmmaSharper/blob/master/CHANGELOG.md).

## Provenance

This is the maintained continuation of
[`kylegregory/EmmaSharp`](https://github.com/kylegregory/EmmaSharp) (last shipped 2019) by way of
[`BinaryPatrick/EmmaSharper`](https://github.com/BinaryPatrick/EmmaSharper).

Several bugs still shown as **open** on those repositories are fixed here — the int32 overflow on
Emma ids, multi-account support, .NET Core support. Each one is recorded with its cause and fix in
[Upstream issues fixed in this fork](https://github.com/joseph3114/EmmaSharper/blob/master/docs/upstream-fixes.md).

## Providers

| Interface | Covers |
|---|---|
| `IEmmaAccountProvider` | The account's users |
| `IEmmaAutomationProvider` | Automation workflows |
| `IEmmaEnterpriseProvider` | Subaccounts of an enterprise account |
| `IEmmaFieldsProvider` | Custom member fields |
| `IEmmaGroupProvider` | Groups and bulk group membership |
| `IEmmaMailingProvider` | Mailings, their HTML, recipients |
| `IEmmaMemberProvider` | Members, statuses, bulk imports |
| `IEmmaResponseProvider` | Mailing response data |
| `IEmmaSearchProvider` | Saved searches |
| `IEmmaSignupFormProvider` | Sign-up forms |
| `IEmmaSubscriptionProvider` | Subscriptions |
| `IEmmaWebhookProvider` | Webhooks |

Every method is asynchronous and takes a trailing `CancellationToken`.

## Reporting bugs

Use [this repository's issue tracker](https://github.com/joseph3114/EmmaSharper/issues). The
upstream repositories are not monitored.
