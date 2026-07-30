using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EmmaSharper.Samples;

/// <summary>Register the providers and make one call. The whole surface starts here.</summary>
internal static class QuickStart
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
        });

        await using ServiceProvider provider = services.BuildServiceProvider();
        IEmmaMemberProvider members = provider.GetRequiredService<IEmmaMemberProvider>();

        // Every parameter before the token is optional, so name what you pass.
        int active = await members.GetMemberCount(status: MemberStatusShort.Active, cancellationToken: ct);
        int optout = await members.GetMemberCount(status: MemberStatusShort.Optout, cancellationToken: ct);
        int total = await members.GetMemberCount(cancellationToken: ct);

        Console.WriteLine($"account {credentials.AccountId}");
        Console.WriteLine($"  active {active:N0}");
        Console.WriteLine($"  opt-out {optout:N0}");
        Console.WriteLine($"  all statuses {total:N0}");
    }
}
