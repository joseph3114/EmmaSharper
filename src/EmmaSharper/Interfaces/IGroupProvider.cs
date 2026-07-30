using System.Threading;
﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmmaSharper
{
    /// <summary>
    /// Provides access to manage all aspects of the groups in your account. In addition to various CRUD 
    /// methods, you can also use these endpoints to manage the members of your groups. You’ll want to use these 
    /// methods if you’re managing group membership for more than one member at a time. For dealing with single 
    /// members, there are better methods in the members endpoints.
    /// </summary>
    public interface IEmmaGroupProvider
    {
        /// <summary>Get number of all active member groups for a single account</summary>
        /// <returns>An int of groups.</returns>
        Task<int> ListGroupCount(IEnumerable<GroupType>? groupType = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a basic listing of all active member groups for a single account. Be sure to get a count of groups
        /// before accessing this method, so you're aware of paging requirements.
        /// </summary>
        /// <param name="groupType">Accepts a comma-separated string with one or more GroupTypes. Defaults to Group.</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>An array of groups.</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Group>> ListGroups(IEnumerable<GroupType>? groupType = null, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get the detailed information for a single member group</summary>
        /// <param name="memberGroupId">The Member Group Id to be retrieved.</param>
        /// <returns>A group.</returns>
        /// <remarks>Http404 if the group does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Group> GetGroup(string memberGroupId, CancellationToken cancellationToken = default);

        /// <summary>Create one or more new member groups</summary>
        /// <param name="groups">A Group to be created. Each object must contain a group_name parameter.</param>
        /// <returns>An array of the new group ids and group names.</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Group>> CreateGroups(IEnumerable<GroupName> groups, CancellationToken cancellationToken = default);

        /// <summary>Update information for a single member group</summary>
        /// <param name="memberGroupId">The Member Group Id to be updated.</param>
        /// <param name="group">The Group to be updated.</param>
        /// <returns>True if the update was successful</returns>
        /// <remarks>Http404 if the group does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> UpdateGroup(string memberGroupId, UpdateGroup group, CancellationToken cancellationToken = default);

        /// <summary>Delete a single member group</summary>
        /// <param name="memberIdGroup">The Member Group Id to be deleted.</param>
        /// <returns>True if the group is deleted.</returns>
        /// <remarks>Http404 if the group does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> DeleteGroup(string memberIdGroup, CancellationToken cancellationToken = default);

        /// <summary>Get the count of members in a single active member group</summary>
        /// <param name="memberGroupId">The Member Group Id to be retrieved.</param>
        /// <param name="includeDeleted">Include deleted members. Optional, defaults to false.</param>
        /// <returns>An array of members.</returns>
        /// <remarks>Http404 if the group does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> ListGroupMembersCount(string memberGroupId, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>Get the members in a single active member group</summary>
        /// <param name="memberGroupId">The Member Group Id to be retrieved.</param>
        /// <param name="includeDeleted">Include deleted members. Optional, defaults to false.</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>An array of members.</returns>
        /// <remarks>Http404 if the group does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Member>> ListGroupMembers(string memberGroupId, bool includeDeleted = false, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Add a list of members to a single active member group</summary>
        /// <param name="memberGroupId">The Member Group Id to be retrieved.</param>
        /// <param name="memberIds">An array of member ids.</param>
        /// <returns>An array of references to the members added to the group. If a member already exists in the group or is not a valid member, that reference will not be returned.</returns>
        /// <remarks>Http404 if the group does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<long>> AddMembersToGroup(string memberGroupId, MemberIdList memberIds, CancellationToken cancellationToken = default);

        /// <summary>Copy all the users of one group into another group</summary>
        /// <param name="fromGroupId">The Member Group ID to be copied from.</param>
        /// <param name="toGroupId">The Member Group ID to be copied to.</param>
        /// <param name="status">An Array of Member Status strings. This is ‘a’ (active), ‘o’ (optout), or ‘e’ (error).</param>
        /// <returns>Returns true.</returns>
        /// <remarks>Http404 if the group does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> CopyUsersFromGroup(string fromGroupId, string toGroupId, MemberStatusShortList status, CancellationToken cancellationToken = default);

        /// <summary>Remove members from a single active member group</summary>
        /// <param name="memberGroupId">The Member Group Id to be retrieved.</param>
        /// <param name="memberIds">An array of member ids.</param>
        /// <returns>An array of references to the removed members.</returns>
        /// <remarks>Http404 if the group does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<long>> RemoveMembersFromGroup(string memberGroupId, MemberIdList memberIds, CancellationToken cancellationToken = default);

        /// <summary>Remove all members from a single active member group</summary>
        /// <param name="memberGroupId">The Member Group Id to be retrieved.</param>
        /// <param name="status">A Member Status string. Optional. This is ‘a’ (active), ‘o’ (optout), or ‘e’ (error).</param>
        /// <returns>Returns the number of members removed from the group.</returns>
        /// <remarks>Http404 if the group does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> DeleteAllMembersFromGroup(string memberGroupId, MemberStatusShort? status = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete all members in this group with the specified status. Then, remove those members from all active 
        /// member groups as a background job. The member_status_id parameter must be set.
        /// </summary>
        /// <param name="memberGroupId">The Member Group Id to be retrieved.</param>
        /// <param name="status">A Member Status string. This is ‘a’ (active), ‘o’ (optout), or ‘e’ (error).</param>
        /// <returns>Returns true.</returns>
        /// <remarks>Http404 if the group does not exist.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> DeleteAllFromMemberGroupsByStatus(string memberGroupId, MemberStatusShort status, CancellationToken cancellationToken = default);
    }
}