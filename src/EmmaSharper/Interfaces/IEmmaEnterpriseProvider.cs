using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EmmaSharper
{
    /// <summary>Enterprise-level endpoints, which operate across subaccounts.</summary>
    /// <remarks>
    /// Requires enterprise credentials. Pair this with
    /// <see cref="IEmmaAccountScopeFactory"/> to discover subaccounts and then address each one.
    /// </remarks>
    public interface IEmmaEnterpriseProvider
    {
        /// <summary>Lists the subaccounts belonging to this enterprise account.</summary>
        /// <param name="status">
        /// Which lifecycle states to include. Defaults to <see cref="SubaccountStatusFilter.All"/>,
        /// because retired subaccounts can still hold billable contacts.
        /// </param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IReadOnlyList<Subaccount>> ListSubaccounts(
            SubaccountStatusFilter status = SubaccountStatusFilter.All,
            CancellationToken cancellationToken = default);
    }
}
