using System.Threading;
﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmmaSharper
{
    /// <summary>
    /// Provides access to create, edit, and delete searches. You can also retrieve the members matching 
    /// any search created in your account.
    /// </summary>
    public interface IEmmaSearchProvider
    {
        /// <summary>Create a saved search</summary>
        /// <param name="search">A name used to describe this search and a combination of search conditions, as described in the documentation.</param>
        /// <returns>The ID of the new search.</returns>
        /// <remarks>Http400 if the search is invalid.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> CreateSavedSearch(CreateSearch search, CancellationToken cancellationToken = default);

        /// <summary>Delete a saved search. The member records referred to by the search are not affected</summary>
        /// <param name="searchId">Search identifier</param>
        /// <returns>True if the search is deleted.</returns>
        /// <remarks>Http404 if the search does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> DeleteSavedSearch(string searchId, CancellationToken cancellationToken = default);

        /// <summary>Get the members matching the search</summary>
        /// <param name="searchId">Search identifier</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>An array of members.</returns>
        /// <remarks>Http404 if the search does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Member>> GetMembersMatchingSearch(string searchId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get a count of the number of members matching the search</summary>
        /// <param name="searchId">Search identifier</param>
        /// <returns>An array of members.</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetMembersMatchingSearchCount(string searchId, CancellationToken cancellationToken = default);

        /// <summary>Get the details for a saved search</summary>
        /// <param name="searchId">Search identifier</param>
        /// <param name="includeDeleted">>Optional flag to include deleted searches.</param>
        /// <returns>A search.</returns>
        /// <remarks>Http404 if the search does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Search?> GetSearchDetails(string searchId, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>Retrieve a list of saved searches</summary>
        /// <param name="includeDeleted">Optional flag to include deleted searches.</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>An array of searches.</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<List<Search>> GetSearches(bool includeDeleted = false, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get a count of the number of saved searches</summary>
        /// <param name="includeDeleted">Optional flag to include deleted searches.</param>
        /// <returns>An array of searches.</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetSearchesCount(bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update a saved search. No parameters are required, but either the name or criteria parameter must be present for an update to occur.
        /// </summary>
        /// <param name="searchId">Search identifier</param>
        /// <param name="search">A name used to describe this search and/or a combination of search conditions, as described in the documentation.</param>
        /// <returns>True if the update was successful</returns>
        /// <remarks>Http404 if the search does not exist. Http400 if the search criteria is invalid.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> UpdateSavedSearch(string searchId, CreateSearch search, CancellationToken cancellationToken = default);
    }
}