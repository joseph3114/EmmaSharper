using System.Collections.Generic;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <inheritdoc/>
	internal class MemberProvider : IEmmaMemberProvider
    {
        private readonly IEmmaApiAdapter apiAdapter;

        /// <inheritdoc cref="object.Object"/>
        public MemberProvider(IEmmaApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }

        /// <inheritdoc/>
        public async Task<int> GetMemberCount(bool includeDeleted = false)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members"
            };
            request.AddParameter("count", "true");

            if (includeDeleted)
            {
                request.AddParameter("deleted", includeDeleted);
            }

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Member>> ListMembers(bool includeDeleted = false, uint? start = null, uint? end = null)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members"
            };

            if (includeDeleted)
            {
                request.AddParameter("deleted", includeDeleted);
            }

            return await apiAdapter.MakeRequest<List<Member>>(request, start, end);
        }

        /// <inheritdoc/>
        public async Task<Member> GetMember(string memberId, bool includeDeleted = false)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/{memberId}"
            };
            request.AddUrlSegment("memberId", memberId);

            if (includeDeleted)
            {
                request.AddParameter("deleted", includeDeleted.ToString());
            }

            return await apiAdapter.MakeRequest<Member>(request);
        }

        /// <inheritdoc/>
        public async Task<Member> GetMemberByEmail(string memberEmail, bool includeDeleted = false)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/email/{memberEmail}"
            };
            request.AddUrlSegment("memberEmail", memberEmail);

            if (includeDeleted)
            {
                request.AddParameter("deleted", includeDeleted.ToString());
            }

            return await apiAdapter.MakeRequest<Member>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MemberOptout>> GetMemberOptout(string memberId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/{memberId}/optout"
            };
            request.AddUrlSegment("memberId", memberId);

            return await apiAdapter.MakeRequest<List<MemberOptout>>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateMemberToOptoutByEmail(string memberEmail)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/email/optout/{memberEmail}"
            };
            request.AddUrlSegment("memberEmail", memberEmail);

            return await apiAdapter.MakeRequest<bool>(request);
        }

        /// <inheritdoc/>
        public async Task<MembersAdd> AddNewMembers(AddMembers members)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/members",
            };
            request.AddJsonBody(members);

            return await apiAdapter.MakeRequest<MembersAdd>(request);
        }

        /// <inheritdoc/>
        public async Task<MemberAdd> AddOrUpdateSingleMember(AddMember member)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/members/add",
            };
            request.AddJsonBody(member);

            return await apiAdapter.MakeRequest<MemberAdd>(request);
        }

        /// <inheritdoc/>
        public async Task<MemberSignup> MemberSignup(SignupMember member)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/members/signup",
            };
            request.AddJsonBody(member);

            return await apiAdapter.MakeRequest<MemberSignup>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteMembers(DeleteMembers members)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/delete",
            };
            request.AddJsonBody(members);

            return await apiAdapter.MakeRequest<bool>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> ChangeMemberStatus(ChangeStatus status)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/status",
            };
            request.AddJsonBody(status);

            return await apiAdapter.MakeRequest<bool>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateSingleMemberInformation(string memberId, UpdateMember member)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/{memberId}"
            };
            request.AddUrlSegment("memberId", memberId);
            request.AddJsonBody(member);

            return await apiAdapter.MakeRequest<bool>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteMember(string memberId)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/members/{memberId}"
            };
            request.AddUrlSegment("memberId", memberId);

            return await apiAdapter.MakeRequest<bool>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Group>> GetMemberGroups(string memberId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/{memberId}/groups"
            };
            request.AddUrlSegment("memberId", memberId);

            return await apiAdapter.MakeRequest<List<Group>>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<int>> AddMemberToGroups(string memberId, IEnumerable<int> groupIds)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/{memberId}/groups"
            };
            request.AddUrlSegment("memberId", memberId);
            request.AddJsonBody(new { group_ids = groupIds });

            return await apiAdapter.MakeRequest<List<int>>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<int>> RemoveMemberFromGroups(string memberId, List<int> groupIds)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/{memberId}/groups/remove"
            };
            request.AddUrlSegment("memberId", memberId);
            request.AddJsonBody(new { group_ids = groupIds });

            return await apiAdapter.MakeRequest<List<int>>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAllMembers(MemberStatusShort memberStatusId)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/members"
            };
            request.AddParameter("member_status_id", memberStatusId.ToEnumString());

            return await apiAdapter.MakeRequest<bool>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveMemberFromAllGroups(string memberId)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/members/{memberId}/groups"
            };
            request.AddUrlSegment("memberId", memberId);

            return await apiAdapter.MakeRequest<bool>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveMembersFromGroups(RemoveMemberGroups groups)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/groups/remove",
            };
            request.AddJsonBody(groups);

            return await apiAdapter.MakeRequest<bool>(request);
        }

        /// <inheritdoc/>
        public async Task<int> GetMemberMailingHistoryCount(string memberId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/{memberId}/mailings"
            };
            request.AddUrlSegment("memberId", memberId);

            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MailingHistory>> GetMemberMailingHistory(string memberId, uint? start = null, uint? end = null)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/{memberId}/mailings"
            };
            request.AddUrlSegment("memberId", memberId);

            return await apiAdapter.MakeRequest<List<MailingHistory>>(request, start, end);
        }

        /// <inheritdoc/>
        public async Task<int> GetMembersAffectedByImportCount(string importId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/imports/{importId}/members"
            };
            request.AddUrlSegment("importId", importId);

            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ImportMembers>> GetMembersAffectedByImport(string importId, uint? start = null, uint? end = null)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/imports/{importId}/members"
            };
            request.AddUrlSegment("importId", importId);

            return await apiAdapter.MakeRequest<List<ImportMembers>>(request, start, end);
        }

        /// <inheritdoc/>
        public async Task<Import> GetImportInformation(string importId)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/imports/{importId}"
            };
            request.AddUrlSegment("importId", importId);

            return await apiAdapter.MakeRequest<Import>(request);
        }

        /// <inheritdoc/>
        public async Task<int> GetAllImportsCount()
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/imports"
            };

            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Import>> GetAllImports(uint? start = null, uint? end = null)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/imports"
            };

            return await apiAdapter.MakeRequest<List<Import>>(request, start, end);
        }

        /// <inheritdoc/>
        public async Task<bool> CopyMembersIntoStatusGroup(string groupId, CopyStatus status)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/{groupId}/copy"
            };
            request.AddUrlSegment("groupId", groupId);
            request.AddJsonBody(status);

            return await apiAdapter.MakeRequest<bool>(request);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateStatusOfGroupMembersBasedOnCurrentStatus(MemberStatusShort statusFrom, MemberStatusShort statusTo, string groupId = null)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/status/{statusFrom}/to/{statusTo}"
            };
            request.AddUrlSegment("statusFrom", statusFrom.ToEnumString());
            request.AddUrlSegment("statusTo", statusTo.ToEnumString());

            if (!string.IsNullOrWhiteSpace(groupId))
            {
                request.AddJsonBody(new { group_id = groupId });
            }

            return await apiAdapter.MakeRequest<bool>(request);
        }
    }
}

