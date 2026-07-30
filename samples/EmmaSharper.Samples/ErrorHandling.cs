using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace EmmaSharper.Samples;

/// <summary>Typed exceptions: branch on the status, never on the message text.</summary>
internal static class ErrorHandling
{
    internal static async Task RunAsync(EmmaCredentials credentials, CancellationToken ct)
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AddEmmaApiProviders(options =>
        {
            options.AccountId = credentials.AccountId;
            options.PublicKey = credentials.PublicKey;
            options.SecretKey = credentials.SecretKey;
        });

        await using ServiceProvider provider = services.BuildServiceProvider();
        IEmmaMemberProvider members = provider.GetRequiredService<IEmmaMemberProvider>();

        // An address that will not exist, to show the not-found path.
        const string Missing = "definitely-not-a-member@example.invalid";

        Member? member = await FindMemberAsync(members, Missing, ct);
        Console.WriteLine(member is null
            ? $"{Missing} -> not found (translated to null)"
            : $"{Missing} -> member {member.MemberId}");

        Console.WriteLine();
        Console.WriteLine("EmmaRetryDefaults classification:");
        foreach (HttpStatusCode status in new[]
                 {
                     HttpStatusCode.OK,
                     HttpStatusCode.BadRequest,
                     HttpStatusCode.Unauthorized,
                     HttpStatusCode.Forbidden,
                     (HttpStatusCode)429,
                     HttpStatusCode.ServiceUnavailable,
                 })
        {
            Console.WriteLine($"  {(int)status} {status,-20} transient: {EmmaRetryDefaults.IsTransient(status)}");
        }

        Console.WriteLine();
        Console.WriteLine("403 is treated as a throttle by default; opt out with treatForbiddenAsThrottle: false:");
        Console.WriteLine($"  403 -> {EmmaRetryDefaults.IsTransient(HttpStatusCode.Forbidden, treatForbiddenAsThrottle: false)}");
    }

    /// <summary>Turns Emma's 404 into a null, leaving every other failure to propagate.</summary>
    private static async Task<Member?> FindMemberAsync(
        IEmmaMemberProvider members,
        string email,
        CancellationToken ct)
    {
        try
        {
            return await members.GetMemberByEmail(email, cancellationToken: ct);
        }
        catch (EmmaRateLimitException ex)
        {
            // Must be caught before EmmaException - it derives from it.
            Console.WriteLine($"  throttled; Emma suggested waiting {ex.RetryAfter?.ToString() ?? "(unspecified)"}");
            throw;
        }
        catch (EmmaException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
