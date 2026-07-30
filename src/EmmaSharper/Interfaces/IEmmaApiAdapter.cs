using System.Threading;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <summary>
    /// Sends a described Emma API call and materializes the response.
    /// </summary>
    /// <remarks>
    /// Internal as of 8.0.0. It was public in 7.x, but it takes a transport-shaped request type
    /// and is an implementation detail of the providers - exposing it forced the HTTP library into
    /// this library's public surface. Consumers should depend on the provider interfaces.
    /// </remarks>
    internal interface IEmmaApiAdapter
    {
        /// <summary>Executes <paramref name="request"/> and deserializes the response body.</summary>
        /// <typeparam name="T">The type to bind the response to.</typeparam>
        /// <param name="request">The call to make.</param>
        /// <param name="start">Inclusive start index for paged endpoints.</param>
        /// <param name="end">Inclusive end index for paged endpoints.</param>
        /// <param name="accountId">Overrides the configured account for this call only.</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<T?> MakeRequest<T>(
            EmmaRequest request,
            uint? start = null,
            uint? end = null,
            string? accountId = null,
            CancellationToken cancellationToken = default);
    }
}
