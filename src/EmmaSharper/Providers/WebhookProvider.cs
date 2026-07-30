using System.Collections.Generic;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <inheritdoc/>
    internal class WebhookProvider : IEmmaWebhookProvider
    {
        private readonly IEmmaApiAdapter apiAdapter;

        /// <inheritdoc cref="object.Object"/>
        public WebhookProvider(IEmmaApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Webhook>> GetWebhooks()
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/webhooks"
            };

            return await apiAdapter.MakeRequest<List<Webhook>>(request);
        }

        /// <inheritdoc/>
        public async Task<Webhook> GetWebhookById(string webhookId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/webhooks/{webhookId}"
            };
            request.AddUrlSegment("webhookId", webhookId);

            return await apiAdapter.MakeRequest<Webhook>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<WebhookEvents>> GetWebhookEvents()
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/webhooks/events"
            };

            return await apiAdapter.MakeRequest<List<WebhookEvents>>(request);
        }

        /// <inheritdoc/>
        public async Task<int> CreateWebhook(CreateWebhook webhook)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/webhooks",
            };
            request.AddJsonBody(webhook);

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<int> UpdateWebhook(string webhookId, UpdateWebhook webhook)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/webhooks/{webhookId}"
            };
            request.AddUrlSegment("webhookId", webhookId);
            request.AddJsonBody(webhook);

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteWebhookById(string webhookId)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/webhooks/{webhookId}"
            };
            request.AddUrlSegment("webhookId", webhookId);

            return await apiAdapter.MakeRequest<bool>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAllWebhooks()
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/webhooks"
            };

            return await apiAdapter.MakeRequest<bool>(request);
        }
    }
}
