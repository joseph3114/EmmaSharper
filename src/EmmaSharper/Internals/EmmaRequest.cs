using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace EmmaSharper.Internals
{
    /// <summary>HTTP verbs, named to match the shape provider code already uses.</summary>
    internal static class Method
    {
        internal static readonly HttpMethod GET = HttpMethod.Get;
        internal static readonly HttpMethod POST = HttpMethod.Post;
        internal static readonly HttpMethod PUT = HttpMethod.Put;
        internal static readonly HttpMethod DELETE = HttpMethod.Delete;
    }

    /// <summary>
    /// A transport-agnostic description of one Emma API call, resolved into an
    /// <see cref="HttpRequestMessage"/> by the adapter.
    /// </summary>
    /// <remarks>
    /// The surface here intentionally mirrors the small slice of RestSharp's RestRequest that the
    /// providers actually used - Resource, AddUrlSegment, AddParameter, AddJsonBody - so removing
    /// the RestSharp dependency did not require rewriting ~2,000 lines of provider code.
    /// </remarks>
    internal sealed class EmmaRequest
    {
        internal EmmaRequest()
            : this(HttpMethod.Get)
        {
        }

        internal EmmaRequest(HttpMethod method)
        {
            Method = method ?? HttpMethod.Get;
        }

        /// <summary>The HTTP verb for this call.</summary>
        internal HttpMethod Method { get; }

        /// <summary>Path template, e.g. <c>/{accountId}/members/{memberId}</c>.</summary>
        internal string Resource { get; set; } = string.Empty;

        internal Dictionary<string, string> Segments { get; } = new(StringComparer.Ordinal);

        internal List<KeyValuePair<string, string>> Query { get; } = new();

        internal object? Body { get; private set; }

        /// <summary>Substitutes a <c>{name}</c> placeholder in <see cref="Resource"/>.</summary>
        internal EmmaRequest AddUrlSegment(string name, object? value)
        {
            Segments[name] = Format(value);
            return this;
        }

        /// <summary>Appends a query-string parameter.</summary>
        internal EmmaRequest AddParameter(string name, object? value)
        {
            Query.Add(new KeyValuePair<string, string>(name, Format(value)));
            return this;
        }

        /// <summary>Sets the request body, serialized as JSON at send time.</summary>
        internal EmmaRequest AddJsonBody(object body)
        {
            Body = body;
            return this;
        }

        /// <summary>
        /// Invariant formatting, with booleans lower-cased.
        /// RestSharp emitted <c>True</c>/<c>False</c> via ToString(); Emma's documented query
        /// values are lower-case, so this is a deliberate correction rather than pure parity.
        /// </summary>
        private static string Format(object? value) => value switch
        {
            null => string.Empty,
            bool b => b ? "true" : "false",
            string s => s,
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty,
        };
    }
}
