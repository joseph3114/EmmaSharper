using System.Threading;
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <inheritdoc/>
    internal class SubscriptionProvider : IEmmaSubscriptionProvider
    {
        private readonly IEmmaApiAdapter apiAdapter;

        /// <inheritdoc cref="object.Object"/>
        public SubscriptionProvider(IEmmaApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Subscription>> GetAccountSubscriptions(bool includeDeletedOnly = false, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/subscriptions"
            };

            if (includeDeletedOnly)
            {
                request.AddParameter("deleted_only", includeDeletedOnly);
            }

            if (includeDeleted)
            {
                request.AddParameter("include_deleted", includeDeleted);
            }

            return await apiAdapter.MakeRequest<List<Subscription>>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Subscription> GetAccountSubscription(string subscription_id, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/subscriptions/{subscriptionId}"
            };
            request.AddUrlSegment("subscriptionId", subscription_id);

            return await apiAdapter.MakeRequest<Subscription>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SubscriptionMembers>> GetSubscriptionMembers(string subscription_id, uint start = 0, uint end = 500, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/subscriptions/{subscriptionId}/members"
            };
            request.AddUrlSegment("subscriptionId", subscription_id);

            return await apiAdapter.MakeRequest<List<SubscriptionMembers>>(request, start, end, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SubscriptionMembers>> GetOptOutSubscriptionMembers(string subscription_id, uint start = 0, uint end = 500, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/subscriptions/{subscriptionId}/optouts"
            };
            request.AddUrlSegment("subscriptionId", subscription_id);

            return await apiAdapter.MakeRequest<List<SubscriptionMembers>>(request, start, end, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Subscription> PostNewSubscription(SubscriptionNew subscription, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/subscriptions",
            };
            request.AddJsonBody(subscription);

            return await apiAdapter.MakeRequest<Subscription>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> PostBulkMemberSubscriptions(SubscriptionBulk memberIds, string subscription_id, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/subscriptions/{subscriptionId}/members/bulk"
            };

            request.AddUrlSegment("subscriptionId", subscription_id);
            request.AddJsonBody(memberIds);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> PostBulkImportSubscriptions(SubscriptionImportBulk importId, string subscription_id, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/subscriptions/{subscriptionId}/members/bulk"
            };

            request.AddUrlSegment("subscriptionId", subscription_id);
            request.AddJsonBody(importId);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }


        /// <inheritdoc/>
        public async Task<Subscription> EditSubscription(SubscriptionNew subscription, string subscription_id, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/subscriptions/{subscriptionId}"
            };

            request.AddUrlSegment("subscriptionId", subscription_id);
            request.AddJsonBody(subscription);

            return await apiAdapter.MakeRequest<Subscription>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Subscription> DeleteSubscription(string subscription_id, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/subscriptions/{subscriptionId}"
            };
            request.AddUrlSegment("subscriptionId", subscription_id);

            return await apiAdapter.MakeRequest<Subscription>(request, cancellationToken: cancellationToken);
        }
    }
}
