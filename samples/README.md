# Samples

Runnable counterparts to the [wiki](https://github.com/joseph3114/EmmaSharper/wiki).

These exist for two reasons. The obvious one is that a working program is easier to learn from
than a snippet. The less obvious one matters more: **this project is in the solution and built by
CI**, so every pattern the documentation shows is compiled. A rename in the library fails the
build here rather than quietly leaving the wiki wrong.

## Running

The samples make real calls, so they need real credentials. They come from the environment so
nothing sensitive ends up in source control:

```bash
export EMMA_ACCOUNT_ID="..."      # setx on Windows
export EMMA_PUBLIC_KEY="..."
export EMMA_SECRET_KEY="..."
```

```bash
dotnet run --project samples/EmmaSharper.Samples -- --help
dotnet run --project samples/EmmaSharper.Samples -- quickstart
```

`--help` and the missing-credentials path both work without any configuration, so you can check
the project runs before wiring anything up. `Ctrl+C` cancels rather than kills, which is the
`CancellationToken` plumbing doing its job.

| Sample | Shows | Wiki page |
|---|---|---|
| `quickstart` | DI registration, counting members by status | [Getting Started](https://github.com/joseph3114/EmmaSharper/wiki/Getting-Started) |
| `enterprise` | `IEmmaAccountScopeFactory` across every subaccount, at DOP 4 | [Enterprise and Multiple Accounts](https://github.com/joseph3114/EmmaSharper/wiki/Enterprise-and-Multiple-Accounts) |
| `paging` | Inclusive 500-record windows, `exclude_fields` | [Paging](https://github.com/joseph3114/EmmaSharper/wiki/Paging) |
| `errors` | Typed exceptions, translating 404 to null, retry classification | [Error Handling](https://github.com/joseph3114/EmmaSharper/wiki/Error-Handling) |
| `resilience` | `AddStandardResilienceHandler` with `EmmaRetryDefaults` | [Rate Limiting and Resilience](https://github.com/joseph3114/EmmaSharper/wiki/Rate-Limiting-and-Resilience) |

## Note on the resilience sample

It references `Microsoft.Extensions.Http.Resilience`, which the library itself does not. That
package requires net8.0+, and taking a dependency on it would exclude the .NET Framework consumers
`netstandard2.0` exists for. The library returns the `IHttpClientBuilder` instead and lets the
consumer attach a pipeline — this sample is what that looks like, compiled rather than described.

## Which account do the samples hit?

Whatever `EMMA_ACCOUNT_ID` points at. `enterprise` additionally expects that account to be an
**enterprise** account with subaccounts; against an ordinary account, `ListSubaccounts` will come
back empty rather than fail.

The samples only read. Nothing here creates, updates or deletes.
