using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EmmaSharper.Samples;

/// <summary>
/// Discover every subaccount of an enterprise account and count its active members, using one
/// credential pair throughout.
/// </summary>
internal static class EnterpriseSweep
{
    /// <summary>Emma rate-limits, so more concurrency mostly buys more 403s.</summary>
    private const int MaxParallelism = 4;

    internal static async Task RunAsync(EmmaCredentials credentials, CancellationToken ct)
    {
        ServiceCollection services = new();
        services.AddLogging(b => b.AddSimpleConsole(o => o.SingleLine = true).SetMinimumLevel(LogLevel.Warning));
        services.AddEmmaApiProviders(options =>
        {
            options.AccountId = credentials.AccountId;
            options.PublicKey = credentials.PublicKey;
            options.SecretKey = credentials.SecretKey;
        });

        await using ServiceProvider provider = services.BuildServiceProvider();

        IEmmaEnterpriseProvider enterprise = provider.GetRequiredService<IEmmaEnterpriseProvider>();
        IEmmaAccountScopeFactory scopeFactory = provider.GetRequiredService<IEmmaAccountScopeFactory>();

        IReadOnlyList<Subaccount> subaccounts = await enterprise.ListSubaccounts(cancellationToken: ct);
        Console.WriteLine($"{subaccounts.Count} subaccounts");
        Console.WriteLine();

        ConcurrentBag<(string Name, string Id, int Active)> results = [];

        await Parallel.ForEachAsync(
            subaccounts,
            new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism, CancellationToken = ct },
            async (sub, token) =>
            {
                if (sub.AccountId is null)
                {
                    return;
                }

                // A scope reuses the same credentials and the same pooled HttpClient; only the
                // account segment of the path changes.
                IEmmaAccountScope scope = scopeFactory.ForAccount(sub.AccountId);

                int active = await scope.Members.GetMemberCount(
                    status: MemberStatusShort.Active,
                    cancellationToken: token);

                results.Add((sub.AccountName ?? "(unnamed)", sub.AccountId, active));
            });

        foreach ((string name, string id, int active) in results.OrderByDescending(r => r.Active))
        {
            Console.WriteLine($"{active,10:N0}  {name}  [{id}]");
        }

        Console.WriteLine();
        Console.WriteLine($"{results.Sum(r => r.Active):N0} active members across {results.Count} subaccounts");
    }
}
