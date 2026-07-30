using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EmmaSharper.Samples;

/// <summary>
/// Attach a resilience pipeline that understands Emma's throttling, rather than catching
/// <see cref="EmmaRateLimitException"/> by hand.
/// </summary>
/// <remarks>
/// The library deliberately does not depend on Microsoft.Extensions.Http.Resilience - it requires
/// net8.0+, which would exclude the .NET Framework consumers netstandard2.0 exists for. It hands
/// back the IHttpClientBuilder instead, and this is what the consumer does with it.
/// </remarks>
internal static class ResiliencePipeline
{
    internal static async Task RunAsync(EmmaCredentials credentials, CancellationToken ct)
    {
        ServiceCollection services = new();
        services.AddLogging(b => b.AddSimpleConsole(o => o.SingleLine = true).SetMinimumLevel(LogLevel.Information));

        services.AddEmmaApiProviders(options =>
                {
                    options.AccountId = credentials.AccountId;
                    options.PublicKey = credentials.PublicKey;
                    options.SecretKey = credentials.SecretKey;
                })
                .AddStandardResilienceHandler(options =>
                {
                    // Emma throttles with 403 as well as 429; the default predicate treats a 403
                    // as a permanent auth failure and gives up.
                    options.Retry.ShouldHandle = args => ValueTask.FromResult(
                        EmmaRetryDefaults.IsTransient(args.Outcome.Result)
                        || EmmaRetryDefaults.IsTransient(args.Outcome.Exception));

                    options.Retry.MaxRetryAttempts = 3;

                    // The default is 10 seconds per attempt, which is not enough to fetch a
                    // 500-record member page - it would time out, retry, and time out again.
                    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
                    options.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(2);

                    // The circuit breaker's sampling window must exceed the attempt timeout.
                    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
                });

        await using ServiceProvider provider = services.BuildServiceProvider();
        IEmmaMemberProvider members = provider.GetRequiredService<IEmmaMemberProvider>();

        Console.WriteLine("Resilience pipeline attached: retries on 403/408/429/5xx, 30s per attempt.");
        Console.WriteLine();

        int count = await members.GetMemberCount(status: MemberStatusShort.Active, cancellationToken: ct);
        Console.WriteLine($"{count:N0} active members");
        Console.WriteLine();
        Console.WriteLine("Any throttling was absorbed by the pipeline rather than surfacing here.");
    }
}
