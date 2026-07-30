using System.Threading;
﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmmaSharper
{
    /// <summary>
    /// In addition to the various CRUD endpoints here related to members, you can also change the status of members, including 
    /// opting them out. You’ll notice that there are calls related to individual members, but we also provide quite a few calls
    /// to deal with bulk updates of members. Please try to use these whenever possible as opposed to looping through a list of 
    /// members and calling the individual member calls. Where this is especially important is when adding new members. To do a 
    /// bulk import, you’ll POST to the <see cref="AddNewMembers"/> method. In return, you’ll receive an import ID. You can use 
    /// this ID to check the status and results of your import. Imports are generally pretty fast, but the time to completion 
    /// can vary with greater system usage.
    /// </summary>
    public interface IEmmaMemberProvider
    {
        /// <summary>Add a single member to one or more groups.</summary>
        /// <param name="memberId">Member identifier.</param>
        /// <param name="groupIds">Group ids to which to add this member.</param>
        /// <returns>An array of ids of the affected groups.</returns>
        /// <remarks>Http404 if no member is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<long>> AddMemberToGroups(string memberId, IEnumerable<long> groupIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add new members or update existing members in bulk. If you are doing actions for a single member please see the <see cref="AddOrUpdateSingleMember"/> method.
        /// </summary>
        /// <param name="members">An array of members to update. A member is a dictionary of member emails and field values to import. The only required field is “email”. All other fields are treated as the name of a member field.</param>
        /// <returns>An import id.</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<MembersAdd> AddNewMembers(AddMembers members, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds or updates a single audience member. If you are performing actions on bulk members please use the <see cref="AddNewMembers" /> method.
        /// </summary>
        /// <param name="member">Fields related to adding or updating a Member.</param>
        /// <returns>The member_id of the new or updated member, whether the member was added or an existing member was updated, and the status of the member. The status will be reported as ‘a’ (active), ‘e’ (error), or ‘o’ (optout).</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<MemberAdd> AddOrUpdateSingleMember(AddMember member, CancellationToken cancellationToken = default);

        /// <summary>
        /// Change the status for an array of members. The members will have their member_status_id update
        /// </summary>
        /// <param name="status">Class representing members and their new status.</param>
        /// <returns>True if the members are successfully updated, otherwise False.</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> ChangeMemberStatus(ChangeStatus status, CancellationToken cancellationToken = default);

        /// <summary>Copy all account members of one or more statuses into a group.</summary>
        /// <param name="groupId">Group identifier.</param>
        /// <param name="status">Class representing a list of Member statuses: ‘a’ (active), ‘o’ (optout), and/or ‘e’ (error).</param>
        /// <returns>True</returns>
        /// <remarks>Http404 if the group does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> CopyMembersIntoStatusGroup(string groupId, CopyStatus status, CancellationToken cancellationToken = default);

        /// <summary>Delete all members.</summary>
        /// <param name="memberStatusId">This is ‘a’ (active), ‘o’ (optout), or ‘e’ (error).</param>
        /// <returns>Returns true.</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> DeleteAllMembers(MemberStatusShort memberStatusId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete the specified member. The member, along with any associated response and history information, will be completely removed from the database.
        /// </summary>
        /// <param name="memberId">Member identifier.</param>
        /// <returns>True if the member is deleted.</returns>
        /// <remarks>Http404 if no member is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> DeleteMember(string memberId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete an array of members. The members will be marked as deleted and cannot be retrieved.
        /// </summary>
        /// <param name="members">Class representing an array of member ids to delete.</param>
        /// <returns>True if all members are successfully deleted, otherwise False.</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> DeleteMembers(DeleteMembers members, CancellationToken cancellationToken = default);

        /// <summary>Get information about all imports for this account.</summary>
        /// <param name="start">Pagination: start page. Defaults to first page (e.g. 0).</param>
        /// <param name="end">Pagination: end page. Defaults to first page (e.g. 500).</param>
        /// <returns>An array of import details.</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Import>> GetAllImports(uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get a count of all imports for this account.</summary>
        /// <returns>An array of import details.</returns>
        /// <remarks></remarks>
        Task<int> GetAllImportsCount(CancellationToken cancellationToken = default);

        /// <summary>Get information and statistics about this import.</summary>
        /// <param name="importId">Import identifier.</param>
        /// <returns>Import details for the given import_id.</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Import> GetImportInformation(string importId, CancellationToken cancellationToken = default);

        /// <summary>Get detailed information on a particular member, including all custom fields.</summary>
        /// <returns>A single member if one exists.</returns>
        /// <param name="memberId">Member identifier.</param>
        /// <param name="includeDeleted">Accepts True. Optional flag to include deleted members.</param>
        /// <remarks>Http404 if no member is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Member> GetMember(string memberId, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>Get detailed information on a particular member, including all custom fields, by email address instead of ID.</summary>
        /// <returns>A single member if one exists.</returns>
        /// <param name="memberEmail">Member email.</param>
        /// <param name="includeDeleted">Accepts True. Optional flag to include deleted members.</param>
        /// <remarks>Http404 if no member is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Member> GetMemberByEmail(string memberEmail, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>Get a count of members in an account.</summary>
        /// <returns>The number of members matching the filter.</returns>
        /// <param name="includeDeleted">Accepts True. Optional flag to include deleted members.</param>
        /// <param name="status">
        /// Optional. Count only members in this status. Note this is not the same as
        /// <paramref name="includeDeleted"/> - Emma tracks active, opt-out, error and forwarded
        /// separately from deletion, and a quota figure is normally defined on active only.
        /// </param>
        /// <param name="rawFilter">
        /// Optional. A raw Emma filter expression, e.g. <c>["member_status_id","eq","a"]</c>, for
        /// filters this wrapper does not model. Cannot be combined with <paramref name="status"/>.
        /// </param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetMemberCount(
            bool includeDeleted = false,
            MemberStatusShort? status = null,
            string? rawFilter = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get the groups to which a member belongs.</summary>
        /// <param name="memberId">Member identifier.</param>
        /// <returns>An array of groups.</returns>
        /// <remarks>Http404 if no member is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Group>> GetMemberGroups(string memberId, CancellationToken cancellationToken = default);

        /// <summary>Get the entire mailing history for a member.</summary>
        /// <param name="memberId">Member identifier.</param>
        /// <param name="start">Pagination: start page. Defaults to first page (e.g. 0).</param>
        /// <param name="end">Pagination: end page. Defaults to first page (e.g. 500).</param>
        /// <returns>Message history details for the specified member.</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<MailingHistory>> GetMemberMailingHistory(string memberId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get the number of mailing history entries for a member.</summary>
        /// <param name="memberId">Member identifier.</param>
        /// <returns>Message history details for the specified member.</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetMemberMailingHistoryCount(string memberId, CancellationToken cancellationToken = default);

        /// <summary>If a member has been opted out, returns the details of their optout, specifically date and mailing_id.</summary>
        /// <param name="memberId">Member identifier.</param>
        /// <returns>Member opt out date and mailing if member is opted out.</returns>
        /// <remarks>Http404 if no member is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<MemberOptout>> GetMemberOptout(string memberId, CancellationToken cancellationToken = default);

        /// <summary>Get a list of members affected by this import.</summary>
        /// <param name="importId">Import identifier.</param>
        /// <param name="start">Pagination: start page. Defaults to first page (e.g. 0).</param>
        /// <param name="end">Pagination: end page. Defaults to first page (e.g. 500).</param>
        /// <returns>A list of members in the given account and import.</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ImportMembers>> GetMembersAffectedByImport(string importId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get a count of members affected by this import.</summary>
        /// <param name="importId">Import identifier.</param>
        /// <returns>A list of members in the given account and import.</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetMembersAffectedByImportCount(string importId, CancellationToken cancellationToken = default);

        /// <summary>Get a basic listing of members in an account.</summary>
        /// <returns>The members matching the filter.</returns>
        /// <param name="includeDeleted">Accepts True. Optional flag to include deleted members.</param>
        /// <param name="start">
        /// Pagination: inclusive start index. Defaults to 0. These are record indices, not page
        /// numbers - Emma's range is inclusive, so a 500-record page is start 0 to end 499.
        /// </param>
        /// <param name="end">Pagination: inclusive end index. Defaults to <c>start + 499</c>.</param>
        /// <param name="status">
        /// Optional. Return only members in this status. Distinct from
        /// <paramref name="includeDeleted"/>: Emma tracks active, opt-out, error and forwarded
        /// separately from deletion.
        /// </param>
        /// <param name="fields">
        /// Whether to include custom fields. Excluding them is the single biggest throughput win
        /// available when only the email and id are needed.
        /// </param>
        /// <param name="rawFilter">
        /// Optional. A raw Emma filter expression, e.g. <c>["member_status_id","eq","a"]</c>, for
        /// filters this wrapper does not model. Cannot be combined with <paramref name="status"/>.
        /// </param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Member>> ListMembers(
            bool includeDeleted = false,
            uint? start = null,
            uint? end = null,
            MemberStatusShort? status = null,
            MemberFieldSelection fields = MemberFieldSelection.All,
            string? rawFilter = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takes the necessary actions to signup a member and enlist them in the provided group ids. You can send the same member multiple times and pass in new group ids to signup. This process triggers the opt-out workflow, and will send a mailing to the member on new group enlistments. If no new group ids are provided for an existing member, the endpoint will respond back with their status and member_id, performing no additional actions.
        /// </summary>
        /// <param name="member">Fields related to signing up a member.</param>
        /// <returns>The member_id of the member, and their status. The status will be reported as ‘a’ (active), ‘e’ (error), or ‘o’ (optout).</returns>
        /// <remarks></remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<MemberSignup> MemberSignup(SignupMember member, CancellationToken cancellationToken = default);

        /// <summary>Remove the specified member from all groups.</summary>
        /// <param name="memberId">Member identifier.</param>
        /// <returns>True if the member is removed from all groups.</returns>
        /// <remarks>Http404 if no member is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> RemoveMemberFromAllGroups(string memberId, CancellationToken cancellationToken = default);

        /// <summary>Remove a single member from one or more groups.</summary>
        /// <param name="memberId">Member identifier.</param>
        /// <param name="groupIds">Group ids from which to remove this member</param>
        /// <returns>An array of references to the affected groups.</returns>
        /// <remarks>Http404 if no member is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<long>> RemoveMemberFromGroups(string memberId, List<long> groupIds, CancellationToken cancellationToken = default);

        /// <summary>Remove multiple members from groups.</summary>
        /// <param name="groups">Class representing members and the groups to remove them from.</param>
        /// <returns>True if the members are deleted, otherwise False.</returns>
        /// <remarks>Http404 if any of the members or groups do not exist</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> RemoveMembersFromGroups(RemoveMemberGroups groups, CancellationToken cancellationToken = default);

        /// <summary>Update a member’s status to optout keyed on email address instead of an ID.</summary>
        /// <param name="memberEmail">Member email address for optout.</param>
        /// <returns>True if member status change was successful or member was already opted out.</returns>
        /// <remarks>Http404 if no member is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> UpdateMemberToOptoutByEmail(string memberEmail, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update a single member’s information. Update the information for an existing member (even if they are marked as deleted). Note that this method allows the email address to be updated (which cannot be done with a POST, since in that case the email address is used to identify the member).
        /// </summary>
        /// <param name="memberId">Member identifier.</param>
        /// <param name="member">Class representing fields to update member information.</param>
        /// <returns>True if the member was updated successfully</returns>
        /// <remarks>Http404 if no member is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> UpdateSingleMemberInformation(string memberId, UpdateMember member, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update the status for a group of members, based on their current status. Valid statuses id 
        /// are (‘a’,’e’, ‘f’, ‘o’) active, error, forwarded, optout.
        /// </summary>
        /// <param name="statusFrom">The current status of the members.</param>
        /// <param name="statusTo">The updated status of the members.</param>
        /// <param name="groupId">Optional. Limit the update to members of the specified group</param>
        /// <returns>True</returns>
        /// <remarks>Http400 if the specified status is invalid</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> UpdateStatusOfGroupMembersBasedOnCurrentStatus(MemberStatusShort statusFrom, MemberStatusShort statusTo, string? groupId = null, CancellationToken cancellationToken = default);
    }
}