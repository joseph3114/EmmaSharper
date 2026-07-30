using System;
using System.Threading;
using System.Threading.Tasks;

namespace EmmaSharper.Internals
{
    /// <summary>
    /// Decorates <see cref="IEmmaApiAdapter"/> so every call defaults to a fixed account id.
    /// </summary>
    /// <remarks>
    /// Scoping is applied here rather than by adding an account parameter to all 73 provider
    /// methods. The providers stay untouched, and the decorator is the only thing that knows an
    /// override is in play. An explicit per-call account id still wins, so a scope is a default
    /// rather than a hard constraint.
    /// </remarks>
    internal sealed class AccountScopedApiAdapter : IEmmaApiAdapter
    {
        private readonly IEmmaApiAdapter inner;
        private readonly string accountId;

        internal AccountScopedApiAdapter(IEmmaApiAdapter inner, string accountId)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.accountId = string.IsNullOrWhiteSpace(accountId)
                ? throw new ArgumentException("An account id is required.", nameof(accountId))
                : accountId;
        }

        public Task<T?> MakeRequest<T>(
            EmmaRequest request,
            uint? start = null,
            uint? end = null,
            string? accountId = null,
            CancellationToken cancellationToken = default)
            => inner.MakeRequest<T>(request, start, end, accountId ?? this.accountId, cancellationToken);
    }
}
