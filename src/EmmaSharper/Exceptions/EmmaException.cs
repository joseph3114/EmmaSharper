using System;
using System.Net;
using System.Net.Http;

namespace EmmaSharper
{
    /// <summary>Thrown when the Emma API returns a non-success status code.</summary>
    /// <remarks>
    /// Carries the status code, response body, verb and resource as typed properties, so callers
    /// can branch on them directly. <see cref="Exception.Message"/> is for humans and its wording
    /// is not a contract - do not match on it.
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
