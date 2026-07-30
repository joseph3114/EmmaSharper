using System.Threading;
﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmmaSharper
{
    /// <summary>
    /// Provides access to custom fields in your account. Of particular interest is the <see cref="ClearField"/>
    /// method which lets you clear out all the data in a single field for all members in your account. This makes 
    /// it easy to re-initialize a dataset if you’re looking to correct an import error or syncing issue
    /// </summary>
    public interface IEmmaFieldsProvider
    {
        /// <summary>Gets number of fields for paging</summary>
        /// <param name="includeDeleted">Accepts True. Optional flag to include deleted fields</param>
        /// <returns>An array of fields.</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> ListFieldsCount(bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of this account's defined fields. Be sure to get a count of fields before accessing this method, 
        /// so you're aware of paging requirements
        /// </summary>
        /// <param name="includeDeleted">Accepts True. Optional flag to include deleted fields</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>An array of fields.</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Field>> ListFields(bool includeDeleted = false, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Gets the detailed information about a particular field</summary>
        /// <param name="fieldId">The Field Id of the field to retrieve.</param>
        /// <param name="includeDeleted">Accepts True. Optionally show a field even if it has been deleted.</param>
        /// <returns>A field.</returns>
        /// <remarks>Http404 if the field does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Field> GetField(string fieldId, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>Create a new field. There must not already be a field with this name</summary>
        /// <param name="field">The Field to be created.</param>
        /// <returns>A reference (Field ID as int) to the new field.</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> CreateField(CreateField field, CancellationToken cancellationToken = default);

        /// <summary>Updates an existing field</summary>
        /// <param name="fieldId">The Field Id of the field to update.</param>
        /// <param name="field">The Field to be updated.</param>
        /// <returns>A reference (Field ID as int) to the updated field.</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> UpdateField(string fieldId, UpdateField field, CancellationToken cancellationToken = default);

        /// <summary>Clear the member data for the specified field</summary>
        /// <param name="fieldId">The Field Id of the field to clear.</param>
        /// <returns>True if all of the member field data is deleted</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> ClearField(string fieldId, CancellationToken cancellationToken = default);

        /// <summary>Deletes a field</summary>
        /// <param name="fieldId">The Field Id of the field to delete.</param>
        /// <returns>True if the field is deleted, False otherwise.</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> DeleteField(string fieldId, CancellationToken cancellationToken = default);
    }
}