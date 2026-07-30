using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <inheritdoc/>
    internal class GroupProvider : IEmmaGroupProvider
    {
        private readonly IEmmaApiAdapter apiAdapter;

        /// <inheritdoc cref="object.Object"/>
        public GroupProvider(IEmmaApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }

        /// <inheritdoc/>
        public async Task<int> ListGroupCount(IEnumerable<GroupType> groupType = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/groups"
            };
            request.AddParameter("count", "true");

            if (groupType != null)
            {
                string values = groupType.AsEnumStrings().JoinWith(',');
                request.AddParameter("group_types", values);
            }

            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Group>> ListGroups(IEnumerable<GroupType> groupType = null, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/groups"
            };

            if (groupType != null)
            {
                string values = groupType.AsEnumStrings().JoinWith(',');
                request.AddParameter("group_types", values);
            }

            return await apiAdapter.MakeRequest<List<Group>>(request, start, end, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Group>> CreateGroups(IEnumerable<GroupName> groups, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/groups",
            };
            request.AddJsonBody(new { Groups = groups });

            return await apiAdapter.MakeRequest<List<Group>>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Group> GetGroup(string memberGroupId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/groups/{memberGroupId}"
            };
            request.AddUrlSegment("memberGroupId", memberGroupId);

            return await apiAdapter.MakeRequest<Group>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateGroup(string memberGroupId, UpdateGroup group, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/groups/{memberIdGroup}"
            };
            request.AddUrlSegment("memberIdGroup", memberGroupId);
            request.AddJsonBody(group);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteGroup(string memberIdGroup, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/groups/{memberIdGroup}"
            };
            request.AddUrlSegment("memberIdGroup", memberIdGroup);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<int> ListGroupMembersCount(string memberGroupId, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/groups/{memberGroupId}/members"
            };
            request.AddUrlSegment("memberGroupId", memberGroupId);

            if (includeDeleted)
            {
                request.AddParameter("deleted", includeDeleted);
            }

            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Member>> ListGroupMembers(string memberGroupId, bool includeDeleted = false, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/groups/{memberGroupId}/members"
            };
            request.AddUrlSegment("memberGroupId", memberGroupId);

            if (includeDeleted)
            {
                request.AddParameter("deleted", includeDeleted);
            }

            return await apiAdapter.MakeRequest<List<Member>>(request, start, end, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<long>> AddMembersToGroup(string memberGroupId, MemberIdList memberIds, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/groups/{memberGroupId}/members"
            };
            request.AddUrlSegment("memberGroupId", memberGroupId);
            request.AddJsonBody(memberIds);

            return await apiAdapter.MakeRequest<List<long>>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<long>> RemoveMembersFromGroup(string memberGroupId, MemberIdList memberIds, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/groups/{memberGroupId}/members/remove"
            };
            request.AddUrlSegment("memberGroupId", memberGroupId);
            request.AddJsonBody(memberIds);

            return await apiAdapter.MakeRequest<List<long>>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<int> DeleteAllMembersFromGroup(string memberGroupId, MemberStatusShort? status = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/groups/{memberGroupId}/members"
            };
            request.AddUrlSegment("memberGroupId", memberGroupId);

            if (status != null)
            {
                request.AddParameter("member_status_id", status.Value.ToEnumString());
            }

            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAllFromMemberGroupsByStatus(string memberGroupId, MemberStatusShort status, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/groups/{memberGroupId}/members/remove"
            };
            request.AddUrlSegment("memberGroupId", memberGroupId);
            request.AddParameter("member_status_id", status.ToEnumString());

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> CopyUsersFromGroup(string fromGroupId, string toGroupId, MemberStatusShortList status, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/groups/{fromGroupId}/{toGroupId}/members/copy"
            };
            request.AddUrlSegment("fromGroupId", fromGroupId);
            request.AddUrlSegment("toGroupId", toGroupId);
            request.AddJsonBody(status);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }
    }
}
