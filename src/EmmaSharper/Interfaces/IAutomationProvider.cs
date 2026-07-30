using System.Threading;
﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmmaSharper
{
    /// <summary>Provides access to automation APIs</summary>
    public interface IEmmaAutomationProvider
    {
        /// <summary>Gets detailed information about a single workflow</summary>
        /// <param name="workflowId">The ID of the Workflow to return.</param>
        /// <returns>A single workflow if one exists</returns> 
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Workflow?> GetWorkflowById(string workflowId, CancellationToken cancellationToken = default);

        /// <summary>Gets a count of this account’s automation workflows.</summary>
        /// <returns>A count of automation workflows in the given account.</returns>
        Task<WorkflowCount?> GetWorkflowCounts(CancellationToken cancellationToken = default);

        /// <summary>Gets a list of this account’s automation workflows.</summary>
        /// <returns>A list of automation workflows in the given account.</returns>
        Task<IEnumerable<Workflow>> GetWorkflows(CancellationToken cancellationToken = default);
    }
}