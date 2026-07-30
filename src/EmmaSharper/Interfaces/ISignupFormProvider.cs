using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EmmaSharper
{
    /// <summary>Provides a list of all of your signup forms</summary>
    public interface IEmmaSignupFormProvider
    {
        /// <summary>Gets a list of this account’s signup forms</summary>
        /// <returns>An array of signup forms.</returns>
        Task<IEnumerable<SignupForm>> GetSignupForms(CancellationToken cancellationToken = default);
    }
}