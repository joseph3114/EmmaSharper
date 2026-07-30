# Error Handling

Every non-success response raises `EmmaException`, which carries typed detail:

```csharp
try
{
    Member? member = await members.GetMemberByEmail("someone@example.edu", cancellationToken: ct);
}
catch (EmmaRateLimitException ex)          // must come first - it derives from EmmaException
{
    logger.LogWarning("Throttled; Emma suggested {Wait}", ex.RetryAfter);
}
catch (EmmaException ex)
{
    logger.LogError("Emma {Status} on {Method} {Resource}: {Body}",
        ex.StatusCode, ex.Method, ex.Resource, ex.ResponseBody);
}
```

| Property | |
|---|---|
| `StatusCode` | The `HttpStatusCode` Emma returned |
| `ResponseBody` | Raw body, which usually carries Emma's own error text |
| `Method` | The HTTP verb |
| `Resource` | The request path **template**, e.g. `/{accountId}/members/email/{memberEmail}` |

## Why `Resource` is a template

It deliberately does **not** contain the substituted values. Resolved paths embed member email
addresses, and this property is overwhelmingly used for logging — so putting real addresses in it
would write PII into everyone's application logs by default.

The library's own debug logging follows the same rule: it emits the unresolved template plus the
account id, never the resolved path.

## Branch on the status, not the message

```csharp
catch (EmmaException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
```

`Message` is for humans and its wording is not a contract. Use `StatusCode`, or catch
`EmmaRateLimitException` when you mean throttling.

## Not found

Emma returns `404` for a member or mailing that does not exist, which surfaces as an
`EmmaException` with `StatusCode == HttpStatusCode.NotFound` rather than a null result:

```csharp
try
{
    return await members.GetMemberByEmail(email, cancellationToken: ct);
}
catch (EmmaException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    return null;
}
```

Separately, single-object methods return `T?` because a success response with an empty body
produces null. Collection methods never return null — they return an empty collection, so you can
`foreach` unconditionally.

## Configuration errors surface at startup

Missing credentials or a malformed `BaseUrl` throw `OptionsValidationException` when the options
are first resolved, rather than becoming a confusing `401` on the first call.
