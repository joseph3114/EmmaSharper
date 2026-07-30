# Security policy

## Supported versions

| Version | Supported |
|---|---|
| 8.0.x | ✅ |
| 7.0.x and earlier | ❌ |

Only the latest 8.0.x release receives fixes. Earlier versions are deprecated on nuget.org.

**If you are on 7.0.1 or earlier, you have a known vulnerability today.** Those versions depend on
Newtonsoft.Json `[9.0.1,]`, and NuGet resolves a range to its *lowest* satisfying version — so every
consumer receives 9.0.1, which carries
[GHSA-5crp-9r3c-p9vr](https://github.com/advisories/GHSA-5crp-9r3c-p9vr) (high severity). 8.0.0
removes Newtonsoft.Json entirely.

## Reporting a vulnerability

**Please do not open a public issue for a security problem.**

Use GitHub's private vulnerability reporting instead:
[**Report a vulnerability**](https://github.com/joseph3114/EmmaSharper/security/advisories/new).
That opens a private thread visible only to the maintainer, and it can be turned into a published
advisory with a CVE once a fix is out.

Useful things to include, roughly in order of usefulness:

- The package version and target framework you are on.
- What an attacker can actually do, and what access they need to do it.
- A minimal reproduction if you have one.

### What to expect

This is a volunteer project with a single maintainer, so responses are best-effort rather than
contractual. Reports are acknowledged as soon as they are seen. If a report is valid, the fix ships
as a patch release and the advisory is published crediting you, unless you would rather not be
named.

If you do not hear back within a couple of weeks, it is reasonable to assume the notification was
missed — a nudge on the same private thread is welcome.

## Scope

This library is an HTTP client for a third-party API. Two boundaries worth naming:

- **Vulnerabilities in the Emma API itself are not in scope here.** Report those to
  [Emma (Marigold)](https://myemma.com/). This project is not affiliated with them.
- **Credential handling is the consumer's responsibility.** The library accepts a public and secret
  key and sends them to Emma over HTTPS. It does not persist them, and it does not log them. How
  they are stored and supplied is up to the calling application.

One thing the library does do on your behalf: it deliberately logs the *unresolved* request template
rather than the resolved path, because resolved paths for member lookups embed email addresses.
Regressions of that behaviour are in scope and are covered by a test.
