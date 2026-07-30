using System;
using System.Net;
using System.Net.Http;

namespace EmmaSharper
{
    /// <summary>Thrown when the Emma API returns a non-success status code.</summary>
    /// <remarks>
    /// The 7.x version exposed RestSharp's IRestResponse as a public field, which put a third-party
    /// type in this library's public surface and forced callers to string-match on
    /// <see cref="Exception.Message"/> to discover the status code. The details are now typed.
    /// </remarks>
    public class EmmaException : Exception
    {
        /// <summary>Creates an exception describing a failed Emma API call.</summary>
        /// <param name="statusCode">The HTTP status returned by Emma.</param>
        /// <param name="responseBody">The raw response body, if one was read.</param>
        /// <param name="method">The HTTP verb used.</param>
        /// <param name="resource">The resolved request path.</param>
        public EmmaException(
            HttpStatusCode statusCode,
            string? responseBody = null,
            HttpMethod? method = null,
            string? resource = null)
            : base(BuildMessage(statusCode, responseBody, method, resource))
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
            Method = method;
            Resource = resource;
        }

        /// <summary>The HTTP status code Emma returned.</summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>The raw response body, which usually carries Emma's own error text.</summary>
        public string? ResponseBody { get; }

        /// <summary>The HTTP verb of the failing request.</summary>
        public HttpMethod? Method { get; }

        /// <summary>The resolved request path of the failing request.</summary>
        public string? Resource { get; }

        private static string BuildMessage(
            HttpStatusCode statusCode,
            string? responseBody,
            HttpMethod? method,
            string? resource)
        {
            string call = method is null && resource is null
                ? string.Empty
                : $" for {method?.Method ?? "?"} {resource ?? "?"}";

            string body = string.IsNullOrWhiteSpace(responseBody)
                ? string.Empty
                : $" with body:{Environment.NewLine}{responseBody}";

            return $"Emma returned {(int)statusCode} ({statusCode}){call}{body}";
        }
    }
}
