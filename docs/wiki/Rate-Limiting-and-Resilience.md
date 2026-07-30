# Rate Limiting and Resilience

## Emma throttles with 403

This is the single most surprising thing about the Emma API:

> **Emma returns `403 Forbidden` for rate limiting**, as well as the conventional `429`.

A client written against normal expectations reads that 403 as an authentication failure, decides
the credentials are wrong, and gives up — when the correct response was to wait and retry. It is
the mistake every Emma consumer makes exactly once.

The library classifies both as throttling:

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

`EmmaRateLimitException` derives from `EmmaException`, so a general `catch (EmmaException)` still
sees it — order your catch blocks accordingly.

## Prefer a resilience pipeline

Catching and sleeping works, but a pipeline is better because it understands the surrounding
budget. `AddEmmaApiProviders` returns the `IHttpClientBuilder` so you can attach one:

```csharp
services.AddEmmaApiProviders(builder.Configuration)
        .AddStandardResilienceHandler(options =>
        {
            options.Retry.ShouldHandle = args =>
                ValueTask.FromResult(EmmaRetryDefaults.IsTransient(args.Outcome.Result)
                                  || EmmaRetryDefaults.IsTransient(args.Outcome.Exception));

            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
            options.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(2);
        });
```

`AddStandardResilienceHandler` needs the
[`Microsoft.Extensions.Http.Resilience`](https://www.nuget.org/packages/Microsoft.Extensions.Http.Resilience)
package and **net8.0 or later**. The library itself does not depend on it — that would have shut
out .NET Framework consumers — which is why `EmmaRetryDefaults` exposes plain predicates instead
of Polly types.

> ⚠️ **Raise the attempt timeout.** The standard handler defaults to **10 seconds per attempt**,
> which is not enough to fetch a 500-record member page. Left alone it will time out, retry, and
> time out again, turning a slow call into a failing one.

## What counts as retryable

`EmmaRetryDefaults.IsTransient` returns true for:

| Status | |
|---|---|
| `403` | Emma's throttle signal |
| `408` | Request timeout |
| `429` | Too many requests |
| `500` `502` `503` `504` | Server-side |

and for `EmmaRateLimitException`, `HttpRequestException`, `TimeoutException`, and a client timeout
that arrives dressed as a cancellation.

### The 403 trade-off

A genuine credentials failure also returns 403, and the two cannot be told apart without reading
the response body — which a resilience handler is poorly placed to do. Retrying a bad-credentials
403 wastes the attempt budget and then fails, which is the less damaging of the two errors.

If your credentials rotate and you would rather fail fast:

```csharp
EmmaRetryDefaults.IsTransient(response, treatForbiddenAsThrottle: false)
```

### Cancellation is not uniformly retryable

Your own cancellation is never retried — that would defeat the token. But `HttpClient` reports
*its own timeout* as a cancellation too, distinguished on .NET 5+ by an inner `TimeoutException`,
and that one is retried. On .NET Framework the inner exception is absent, so a client timeout is
treated as non-transient there rather than risk retrying a real cancellation.

## Retry-After

```csharp
TimeSpan? wait = EmmaRetryDefaults.GetRetryAfter(response);
```

Handles both forms Emma may send — a delay in seconds or an absolute date — and clamps a date
already in the past to zero rather than returning a negative span.

## Practical settings for a large sweep

From a production sync over ~47 subaccounts and ~460,000 members:

- **Parallelism 4.** Higher mostly buys more 403s.
- **30-second attempt timeout.** A 500-record page does not reliably return in 10.
- **3 retries** with a modest exponential factor. Emma recovers quickly; long backoffs waste more
  time than they save.
- **Page size 500**, which is Emma's maximum — see [Paging](Paging).
