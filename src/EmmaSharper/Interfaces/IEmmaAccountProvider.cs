using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EmmaSharper
{
    /// <summary>Account-level endpoints for the account currently in scope.</summary>
    public interface IEmmaAccountProvider
    {
        /// <summary>Lists the users with access to this account.</summary>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IReadOnlyList<AccountUser>> ListUsers(CancellationToken cancellationToken = default);
    }
}
