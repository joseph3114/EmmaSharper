using EmmaSharper.Internals;

namespace EmmaSharper.Services
{
    /// <inheritdoc cref="IEmmaAccountScopeFactory"/>
    internal sealed class EmmaAccountScopeFactory : IEmmaAccountScopeFactory
    {
        private readonly IEmmaApiAdapter adapter;

        public EmmaAccountScopeFactory(IEmmaApiAdapter adapter)
        {
            this.adapter = adapter;
        }

        public IEmmaAccountScope ForAccount(string accountId)
            => new EmmaAccountScope(accountId, new AccountScopedApiAdapter(adapter, accountId));
    }
}
