using System.Threading;
﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmmaSharper
{
    /// <summary>Provides access to subscriptions and subscription members</summary>
    public interface IEmmaSubscriptionProvider
    {
        /// <summary>Delete a subscription</summary>
        /// <returns>Information about the subscription, including the date and time it was deleted.</returns>
        /// <param name="subscription_id ">the id to update</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Subscription> DeleteSubscription(string subscription_id, CancellationToken cancellationToken = default);

        /// <summary>Edit a subscription's name or description</summary>
        /// <returns>Information about the updated subscription.Limited to name and description.</returns>
        /// <param name="subscription">Name and description of the subscription text to update. Visible in the Subscription Center.</param>
        /// <param name="subscription_id ">the id to update</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Subscription> EditSubscription(SubscriptionNew subscription, string subscription_id, CancellationToken cancellationToken = default);

        /// <summary>Get detailed information for a specific subscription</summary>
        /// <returns>Information about a subscription.</returns>
        /// <param name="subscription_id">URL segment for the subscription ID to query details on</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Subscription> GetAccountSubscription(string subscription_id, CancellationToken cancellationToken = default);

        /// <summary>Get a list of all subscriptions in an account</summary>
        /// <returns>A list of subscriptions in an account along with related information, including member count and subscription ID.</returns>
        /// <param name="includeDeletedOnly">true or false. Returns deleted subscriptions only. Optional, defaults to false.</param>
        /// <param name="includeDeleted">true or false. Returns deleted subscriptions along with active. Optional, defaults to false.</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Subscription>> GetAccountSubscriptions(bool includeDeletedOnly = false, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>Get a list of member IDs for members who have opted out of a specific subscription</summary>
        /// <returns>A list of member IDs.</returns>
        /// <param name="subscription_id">URL segment for the subscription ID to query details on</param>
        /// <param name="start">Pagination: start page. Defaults to first page (e.g. 0).</param>
        /// <param name="end">Pagination: end page. Defaults to first page (e.g. 500).</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<SubscriptionMembers>> GetOptOutSubscriptionMembers(string subscription_id, uint start = 0, uint end = 500, CancellationToken cancellationToken = default);

        /// <summary>Get a list of member IDs for members subscribed to a specific subscription</summary>
        /// <returns>A list of member IDs.</returns>
        /// <param name="subscription_id">URL segment for the subscription ID to query details on</param>
        /// <param name="start">Pagination: start page. Defaults to first page (e.g. 0).</param>
        /// <param name="end">Pagination: end page. Defaults to first page (e.g. 500).</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<SubscriptionMembers>> GetSubscriptionMembers(string subscription_id, uint start = 0, uint end = 500, CancellationToken cancellationToken = default);

        /// <summary>Bulk subscribe members to a subscription using the import ID of all members</summary>
        /// <returns>True if successful.</returns>
        /// <param name="importId">import ID to bulk subscribe</param>
        /// <param name="subscription_id">subscription id</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> PostBulkImportSubscriptions(SubscriptionImportBulk importId, string subscription_id, CancellationToken cancellationToken = default);

        /// <summary>Bulk subscribe members to a subscription using a list of member IDs</summary>
        /// <returns>True if successful.</returns>
        /// <param name="memberIds">List of memberIDs</param>
        /// <param name="subscription_id">subscription id</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> PostBulkMemberSubscriptions(SubscriptionBulk memberIds, string subscription_id, CancellationToken cancellationToken = default);

        /// <summary>Create a subscription</summary>
        /// <returns> Information about the created subscription, including the subscription ID.</returns>
        /// <param name="subscription">Name and description of the new subscription to create</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Subscription> PostNewSubscription(SubscriptionNew subscription, CancellationToken cancellationToken = default);
    }
}