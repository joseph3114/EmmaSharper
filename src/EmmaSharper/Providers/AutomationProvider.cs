using System.Collections.Generic;
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
        public async Task<IEnumerable<Workflow>> GetWorkflows()
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/automation/workflows"
            };

            return await apiAdapter.MakeRequest<List<Workflow>>(request);
        }

        /// <inheritdoc/>
        public async Task<Workflow> GetWorkflowById(string workflowId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/automation/workflows/{workflowId}"
            };
            request.AddUrlSegment("workflowId", workflowId);

            return await apiAdapter.MakeRequest<Workflow>(request);
        }

        /// <inheritdoc/>
        public async Task<WorkflowCount> GetWorkflowCounts()
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/automation/counts"
            };

            return await apiAdapter.MakeRequest<WorkflowCount>(request);
        }
    }
}
