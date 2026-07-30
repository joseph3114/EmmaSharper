using System.Threading;
﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmmaSharper
{
    /// <summary>Provides access to webhooks</summary>
    public interface IEmmaWebhookProvider
    {
        /// <summary>Create an new webhook</summary>
        /// <param name="webhook">The webhook to be created.</param>
        /// <returns>The ID of the newly created webhook.</returns>@Html.Raw(breadcrumb.Item3)
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> CreateWebhook(CreateWebhook webhook, CancellationToken cancellationToken = default);

        /// <summary>Delete all webhooks registered for an account</summary>
        /// <returns>True if the webhook deleted successfully.</returns>
        Task<bool> DeleteAllWebhooks(CancellationToken cancellationToken = default);

        /// <summary>Deletes an existing webhook</summary>
        /// <param name="webhookId">The ID of the Webhook to delete.</param>
        /// <returns>True if the webhook deleted successfully.</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> DeleteWebhookById(string webhookId, CancellationToken cancellationToken = default);

        /// <summary>Get information for a specific webhook belonging to a specific account</summary>
        /// <param name="webhookId">The ID of the Webhook to return.</param>
        /// <returns>Details for a single webhook</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Webhook> GetWebhookById(string webhookId, CancellationToken cancellationToken = default);

        /// <summary>Get a listing of all event types that are available for webhooks</summary>
        /// <returns>A list of event types and descriptions</returns>
        Task<IEnumerable<WebhookEvents>> GetWebhookEvents(CancellationToken cancellationToken = default);

        /// <summary>Get a basic listing of all webhooks associated with an account</summary>
        /// <returns>A list of webhooks that belong to the given account.</returns>
        Task<IEnumerable<Webhook>> GetWebhooks(CancellationToken cancellationToken = default);

        /// <summary>Update an existing webhook</summary>
        /// <param name="webhookId">The ID of the Webhook to update.</param>
        /// <param name="webhook">The webhook parameters to be updated.</param>
        /// <returns>The id of the updated webhook, or False if the update failed.</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> UpdateWebhook(string webhookId, UpdateWebhook webhook, CancellationToken cancellationToken = default);
    }
}