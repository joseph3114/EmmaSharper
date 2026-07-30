using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <inheritdoc/>
    internal class ResponseProvider : IEmmaResponseProvider
    {
        private readonly IEmmaApiAdapter apiAdapter;

        /// <inheritdoc cref="object.Object"/>
        public ResponseProvider(IEmmaApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseSummary>> GetResponseSummary(DateRange? range = null, bool includeArchived = false)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response"
            };

            if (includeArchived)
            {
                request.AddParameter("include_archived", "1");
            }

            if (range != null)
            {
                request.AddParameter("range", range.Value.ToString());
            }

            return await apiAdapter.MakeRequest<List<ResponseSummary>>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseSummary>> GetResponseSummary(DateTime? date, bool includeArchived = false)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response"
            };

            if (includeArchived)
            {
                request.AddParameter("include_archived", "1");
            }

            if (date != null)
            {
                request.AddParameter("range", date.Value.ToString("yyyy-MM-dd"));
            }

            return await apiAdapter.MakeRequest<List<ResponseSummary>>(request);
        }

        /// <inheritdoc/>
        public async Task<Response> GetMailingResponse(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<Response>(request);
        }

        /// <inheritdoc/>
        public async Task<int> GetMailingSendsCount(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/sends"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseGeneric>> GetMailingSends(string mailingId, uint? start = null, uint? end = null)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/sends"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<ResponseGeneric>>(request, start, end);
        }

        /// <inheritdoc/>
        public async Task<int> GetMailingInProgressCount(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/in_progress"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseGeneric>> GetMailingInProgress(string mailingId, uint? start = null, uint? end = null)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/in_progress"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<ResponseGeneric>>(request, start, end);
        }

        /// <inheritdoc/>
        public async Task<int> GetMailingDelieveriesCount(string mailingId, DeliveryType result = DeliveryType.All)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/deliveries"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddParameter("count", "true");

            request.AddParameter("result", result.ToEnumString<DeliveryType>());

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseDeliveries>> GetMailingDelieveries(string mailingId, DeliveryType result = DeliveryType.All, uint? start = null, uint? end = null)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/deliveries"
            };
            request.AddUrlSegment("mailingId", mailingId);

            request.AddParameter("result", result.ToEnumString<DeliveryType>());

            return await apiAdapter.MakeRequest<List<ResponseDeliveries>>(request, start, end);
        }

        /// <inheritdoc/>
        public async Task<int> GetMailingOpensCount(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/opens"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseGeneric>> GetMailingOpens(string mailingId, uint? start = null, uint? end = null)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/opens"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<ResponseGeneric>>(request, start, end);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Link>> GetMailingLinks(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/links"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<Link>>(request);
        }

        /// <inheritdoc/>
        public async Task<int> GetMailingClicksCount(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/clicks"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseClicks>> GetMailingClicks(string mailingId, string memberId = null, string linkId = null, uint? start = null, uint? end = null)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/clicks"
            };
            request.AddUrlSegment("mailingId", mailingId);

            if (!string.IsNullOrWhiteSpace(memberId))
            {
                request.AddParameter("member_id", memberId);
            }

            if (!string.IsNullOrWhiteSpace(linkId))
            {
                request.AddParameter("link_id", linkId);
            }

            return await apiAdapter.MakeRequest<List<ResponseClicks>>(request, start, end);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseForwards>> GetMailingForwards(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/forwards"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<ResponseForwards>>(request);
        }

        /// <inheritdoc/>
        public async Task<int> GetMailingOptoutsCount(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/optouts"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseGeneric>> GetMailingOptouts(string mailingId, uint? start = null, uint? end = null)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/optouts"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<ResponseGeneric>>(request, start, end);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseSignups>> GetMailingSignups(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/signups"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<ResponseSignups>>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseShares>> GetMailingShares(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/shares"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<ResponseShares>>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseCustomerShare>> GetMailingCustomerShares(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/customer_shares"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<ResponseCustomerShare>>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseCustomerShareClicks>> GetMailingCustomerShareClicks(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/customer_share_clicks"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<ResponseCustomerShareClicks>>(request);
        }

        /// <inheritdoc/>
        public async Task<ResponseCustomerShare> GetMailingCustomerShare(string shareId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{shareId}/customer_share"
            };
            request.AddUrlSegment("shareId", shareId);

            return await apiAdapter.MakeRequest<ResponseCustomerShare>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ResponseSharesOverview>> GetMailingSharesOverview(string mailingId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/response/{mailingId}/shares/overview"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<ResponseSharesOverview>>(request);
        }
    }
}
