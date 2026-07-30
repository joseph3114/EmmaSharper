using System.Threading;
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <inheritdoc/>
    internal class SignupFormProvider : IEmmaSignupFormProvider
    {
        private readonly IEmmaApiAdapter apiAdapter;

        /// <inheritdoc cref="object.Object"/>
        public SignupFormProvider(IEmmaApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<SignupForm>> GetSignupForms(CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/signup_forms"
            };

            return await apiAdapter.MakeRequest<List<SignupForm>>(request, cancellationToken: cancellationToken) ?? new List<SignupForm>();
        }
    }
}
