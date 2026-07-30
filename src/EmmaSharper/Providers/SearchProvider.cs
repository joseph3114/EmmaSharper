using System.Threading;
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <inheritdoc/>
    internal class SearchProvider : IEmmaSearchProvider
    {
        private readonly IEmmaApiAdapter apiAdapter;

        public SearchProvider(IEmmaApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }

        /// <inheritdoc/>
        public async Task<int> GetSearchesCount(bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/searches"
            };
            request.AddParameter("count", "true");

            if (includeDeleted)
            {
                request.AddParameter("deleted", "true");
            }

            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<List<Search>> GetSearches(bool includeDeleted = false, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/searches"
            };

            if (includeDeleted)
            {
                request.AddParameter("deleted", "true");
            }

            return await apiAdapter.MakeRequest<List<Search>>(request, start, end, cancellationToken: cancellationToken) ?? new List<Search>();
        }

        /// <inheritdoc/>
        public async Task<Search?> GetSearchDetails(string searchId, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/searches/{searchId}"
            };
            request.AddUrlSegment("searchId", searchId);

            if (includeDeleted)
            {
                request.AddParameter("deleted", "true");
            }

            return await apiAdapter.MakeRequest<Search>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<int> CreateSavedSearch(CreateSearch search, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/searches",
            };
            request.AddJsonBody(search);

            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateSavedSearch(string searchId, CreateSearch search, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/searches/{searchId}"
            };
            request.AddUrlSegment("searchId", searchId);
            request.AddJsonBody(search);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteSavedSearch(string searchId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/searches/{searchId}"
            };
            request.AddUrlSegment("searchId", searchId);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<int> GetMembersMatchingSearchCount(string searchId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/searches/{searchId}/members"
            };
            request.AddUrlSegment("searchId", searchId);
            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Member>> GetMembersMatchingSearch(string searchId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/searches/{searchId}/members"
            };
            request.AddUrlSegment("searchId", searchId);

            return await apiAdapter.MakeRequest<List<Member>>(request, start, end, cancellationToken: cancellationToken) ?? new List<Member>();
        }
    }
}
