using Microsoft.Extensions.DependencyInjection;

namespace EmmaSharper.Samples;

/// <summary>
/// Page through members. Ranges are inclusive record indices, so a 500-record page is 0 to 499.
/// </summary>
internal static class PagingMembers
{
    private const int PageSize = 500;

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

        int expected = await members.GetMemberCount(status: MemberStatusShort.Active, cancellationToken: ct);
        Console.WriteLine($"{expected:N0} active members to walk, {PageSize} at a time");
        Console.WriteLine();

        int seen = 0;
        uint start = 0;

        while (true)
        {
            List<Member> page = (await members.ListMembers(
                start: start,
                end: start + PageSize - 1,          // inclusive: 0..499, then 500..999
                status: MemberStatusShort.Active,
                // Only the email and id are wanted here. On a large account this is the single
                // biggest throughput difference available.
                fields: MemberFieldSelection.ExcludeCustomFields,
                cancellationToken: ct)).ToList();

            seen += page.Count;
            Console.WriteLine($"  {start,8}-{start + PageSize - 1,-8} {page.Count,4} records   (running total {seen:N0})");

            // A short page means the end. Do not use the count as a loop bound - members can be
            // added or removed while you page.
            if (page.Count < PageSize)
            {
                break;
            }

            start += PageSize;
        }

        Console.WriteLine();
        Console.WriteLine($"walked {seen:N0} members; the count reported {expected:N0}");
    }
}
