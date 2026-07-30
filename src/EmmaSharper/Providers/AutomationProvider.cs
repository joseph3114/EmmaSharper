using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <inheritdoc/>
    internal class AutomationProvider : IEmmaAutomationProvider
    {
        private readonly IEmmaApiAdapter apiAdapter;

        /// <inheritdoc cref="object.Object"/>
        public AutomationProvider(IEmmaApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Workflow>> GetWorkflows(CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/automation/workflows"
            };

            return await apiAdapter.MakeRequest<List<Workflow>>(request, cancellationToken: cancellationToken) ?? new List<Workflow>();
        }

        /// <inheritdoc/>
        public async Task<Workflow?> GetWorkflowById(string workflowId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/automation/workflows/{workflowId}"
            };
            request.AddUrlSegment("workflowId", workflowId);

            return await apiAdapter.MakeRequest<Workflow>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<WorkflowCount?> GetWorkflowCounts(CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/automation/counts"
            };

            return await apiAdapter.MakeRequest<WorkflowCount>(request, cancellationToken: cancellationToken);
        }
    }
}
