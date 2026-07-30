using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <inheritdoc/>
    internal class EnterpriseProvider : IEmmaEnterpriseProvider
    {
        /// <summary>Every individual flag, in the order Emma documents them.</summary>
        private static readonly SubaccountStatusFilter[] IndividualStatuses =
        {
            SubaccountStatusFilter.Active,
            SubaccountStatusFilter.Trial,
            SubaccountStatusFilter.PendingRetirement,
            SubaccountStatusFilter.Retired,
        };

        private readonly IEmmaApiAdapter apiAdapter;

        /// <inheritdoc cref="object.Object"/>
        public EnterpriseProvider(IEmmaApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Subaccount>> ListSubaccounts(
            SubaccountStatusFilter status = SubaccountStatusFilter.All,
            CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/enterprise/subaccounts"
            };
            request.AddParameter("status", ToQueryValue(status));

            // Object-wrapped: {"subaccounts": [...]}, unlike the bare arrays the members
            // endpoints return.
            SubaccountEnvelope? envelope = await apiAdapter
                .MakeRequest<SubaccountEnvelope>(request, cancellationToken: cancellationToken);

            return envelope?.Subaccounts ?? new List<Subaccount>();
        }

        /// <summary>Renders the flags as Emma's comma-separated <c>status</c> parameter.</summary>
        private static string ToQueryValue(SubaccountStatusFilter status)
        {
            // Explicit Where rather than a filtering foreach - CodeQL flags the latter as
            // cs/linq/missed-where, and here the LINQ form reads better anyway.
            List<string> selected = IndividualStatuses
                .Where(flag => (status & flag) == flag)
                .AsEnumStrings()
                .ToList();

            if (selected.Count == 0)
            {
                throw new ArgumentException(
                    "At least one subaccount status must be selected.", nameof(status));
            }

            return selected.JoinWith(',');
        }
    }
}
