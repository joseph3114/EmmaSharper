using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <inheritdoc/>
    internal class AccountProvider : IEmmaAccountProvider
    {
        private readonly IEmmaApiAdapter apiAdapter;

        /// <inheritdoc cref="object.Object"/>
        public AccountProvider(IEmmaApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<AccountUser>> ListUsers(CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/accounts/users"
            };

            // Object-wrapped: {"users": [...]}.
            AccountUserEnvelope? envelope = await apiAdapter
                .MakeRequest<AccountUserEnvelope>(request, cancellationToken: cancellationToken);

            return envelope?.Users ?? new List<AccountUser>();
        }
    }
}
