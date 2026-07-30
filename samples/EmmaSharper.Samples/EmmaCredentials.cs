using System.Diagnostics.CodeAnalysis;

namespace EmmaSharper.Samples;

/// <summary>Emma credentials, read from the environment so nothing sensitive is committed.</summary>
internal sealed record EmmaCredentials(string AccountId, string PublicKey, string SecretKey)
{
    internal static bool TryLoadFromEnvironment(
        [NotNullWhen(true)] out EmmaCredentials? credentials,
        [NotNullWhen(false)] out string? missing)
    {
        string?[] values =
        [
            Environment.GetEnvironmentVariable("EMMA_ACCOUNT_ID"),
            Environment.GetEnvironmentVariable("EMMA_PUBLIC_KEY"),
            Environment.GetEnvironmentVariable("EMMA_SECRET_KEY"),
        ];

        string[] names = ["EMMA_ACCOUNT_ID", "EMMA_PUBLIC_KEY", "EMMA_SECRET_KEY"];

        List<string> absent = [];
        for (int i = 0; i < values.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(values[i]))
            {
                absent.Add(names[i]);
            }
        }

        if (absent.Count > 0)
        {
            credentials = null;
            missing = string.Join(", ", absent);
            return false;
        }

        credentials = new EmmaCredentials(values[0]!, values[1]!, values[2]!);
        missing = null;
        return true;
    }
}
