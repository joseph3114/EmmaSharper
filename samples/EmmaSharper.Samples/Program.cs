// Top-level statements live in the global namespace, so unlike the sample classes - which sit in
// EmmaSharper.Samples and pick this up from the enclosing namespace - this needs to be explicit.
using EmmaSharper;
using EmmaSharper.Samples;

// Runnable counterparts to the wiki. Because this project is in the solution and built by CI,
// every pattern the documentation shows is compiled - a rename in the library breaks the build
// here rather than silently making the docs wrong.
//
//   dotnet run --project samples/EmmaSharper.Samples -- <sample>
//
// Credentials come from the environment so nothing sensitive lands in source:
//   EMMA_ACCOUNT_ID, EMMA_PUBLIC_KEY, EMMA_SECRET_KEY

Dictionary<string, (string Description, Func<EmmaCredentials, CancellationToken, Task> Run)> samples = new(StringComparer.OrdinalIgnoreCase)
{
    ["quickstart"] = ("Register the providers and count members", QuickStart.RunAsync),
    ["enterprise"] = ("Walk every subaccount of an enterprise account", EnterpriseSweep.RunAsync),
    ["paging"] = ("Page through members 500 at a time", PagingMembers.RunAsync),
    ["errors"] = ("Handle Emma's error and throttle responses", ErrorHandling.RunAsync),
    ["resilience"] = ("Attach a resilience pipeline with Emma's retry rules", ResiliencePipeline.RunAsync),
};

string? requested = args.FirstOrDefault();

if (requested is null || requested is "-h" or "--help" or "help")
{
    Console.WriteLine("EmmaSharper samples");
    Console.WriteLine();
    Console.WriteLine("  dotnet run --project samples/EmmaSharper.Samples -- <sample>");
    Console.WriteLine();
    foreach ((string name, (string description, _)) in samples)
    {
        Console.WriteLine($"  {name,-12} {description}");
    }
    Console.WriteLine();
    Console.WriteLine("Set EMMA_ACCOUNT_ID, EMMA_PUBLIC_KEY and EMMA_SECRET_KEY before running.");
    return 0;
}

if (!samples.TryGetValue(requested, out (string Description, Func<EmmaCredentials, CancellationToken, Task> Run) sample))
{
    Console.Error.WriteLine($"Unknown sample '{requested}'. Run with --help to list them.");
    return 2;
}

if (!EmmaCredentials.TryLoadFromEnvironment(out EmmaCredentials? credentials, out string? missing))
{
    Console.Error.WriteLine($"Missing configuration: {missing}");
    Console.Error.WriteLine();
    Console.Error.WriteLine("The samples make real calls against Emma, so they need real credentials:");
    Console.Error.WriteLine("  setx EMMA_ACCOUNT_ID  \"...\"      (or export, on Unix)");
    Console.Error.WriteLine("  setx EMMA_PUBLIC_KEY  \"...\"");
    Console.Error.WriteLine("  setx EMMA_SECRET_KEY  \"...\"");
    return 3;
}

// Ctrl+C cancels rather than kills, which is the point of the CancellationToken plumbing.
using CancellationTokenSource cts = new();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    Console.WriteLine();
    Console.WriteLine("Cancelling...");
    cts.Cancel();
};

try
{
    await sample.Run(credentials!, cts.Token);
    return 0;
}
catch (OperationCanceledException)
{
    Console.WriteLine("Cancelled.");
    return 130;
}
catch (EmmaException ex)
{
    Console.Error.WriteLine($"Emma returned {(int)ex.StatusCode} for {ex.Method?.Method} {ex.Resource}");
    if (!string.IsNullOrWhiteSpace(ex.ResponseBody))
    {
        Console.Error.WriteLine(ex.ResponseBody);
    }
    return 1;
}
