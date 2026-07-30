using System.Threading;
﻿using System.Collections.Generic;
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
        public async Task<int> GetMemberCount(
            bool includeDeleted = false,
            MemberStatusShort? status = null,
            string? rawFilter = null,
            CancellationToken cancellationToken = default)
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

            string? filter = EmmaFilter.ResolveMemberFilter(status, rawFilter);
            if (filter is not null)
            {
                request.AddParameter("filter", filter);
            }

            // Responds with a bare integer, not JSON - see the note in EnvelopedResponses.
            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Member>> ListMembers(
            bool includeDeleted = false,
            uint? start = null,
            uint? end = null,
            MemberStatusShort? status = null,
            MemberFieldSelection fields = MemberFieldSelection.All,
            string? rawFilter = null,
            CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members"
            };

            if (includeDeleted)
            {
                request.AddParameter("deleted", includeDeleted);
            }

            string? filter = EmmaFilter.ResolveMemberFilter(status, rawFilter);
            if (filter is not null)
            {
                request.AddParameter("filter", filter);
            }

            if (fields == MemberFieldSelection.ExcludeCustomFields)
            {
                request.AddParameter("exclude_fields", 1);
            }

            return await apiAdapter.MakeRequest<List<Member>>(request, start, end, cancellationToken: cancellationToken) ?? new List<Member>();
        }

        /// <inheritdoc/>
        public async Task<Member?> GetMember(string memberId, bool includeDeleted = false, CancellationToken cancellationToken = default)
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

            return await apiAdapter.MakeRequest<Member>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Member?> GetMemberByEmail(string memberEmail, bool includeDeleted = false, CancellationToken cancellationToken = default)
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

            return await apiAdapter.MakeRequest<Member>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MemberOptout>> GetMemberOptout(string memberId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/{memberId}/optout"
            };
            request.AddUrlSegment("memberId", memberId);

            return await apiAdapter.MakeRequest<List<MemberOptout>>(request, cancellationToken: cancellationToken) ?? new List<MemberOptout>();
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateMemberToOptoutByEmail(string memberEmail, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/email/optout/{memberEmail}"
            };
            request.AddUrlSegment("memberEmail", memberEmail);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<MembersAdd?> AddNewMembers(AddMembers members, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/members",
            };
            request.AddJsonBody(members);

            return await apiAdapter.MakeRequest<MembersAdd>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<MemberAdd?> AddOrUpdateSingleMember(AddMember member, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/members/add",
            };
            request.AddJsonBody(member);

            return await apiAdapter.MakeRequest<MemberAdd>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<MemberSignup?> MemberSignup(SignupMember member, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/members/signup",
            };
            request.AddJsonBody(member);

            return await apiAdapter.MakeRequest<MemberSignup>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteMembers(DeleteMembers members, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/delete",
            };
            request.AddJsonBody(members);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> ChangeMemberStatus(ChangeStatus status, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/status",
            };
            request.AddJsonBody(status);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateSingleMemberInformation(string memberId, UpdateMember member, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/{memberId}"
            };
            request.AddUrlSegment("memberId", memberId);
            request.AddJsonBody(member);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteMember(string memberId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/members/{memberId}"
            };
            request.AddUrlSegment("memberId", memberId);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Group>> GetMemberGroups(string memberId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/{memberId}/groups"
            };
            request.AddUrlSegment("memberId", memberId);

            return await apiAdapter.MakeRequest<List<Group>>(request, cancellationToken: cancellationToken) ?? new List<Group>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<long>> AddMemberToGroups(string memberId, IEnumerable<long> groupIds, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/{memberId}/groups"
            };
            request.AddUrlSegment("memberId", memberId);
            request.AddJsonBody(new { group_ids = groupIds });

            return await apiAdapter.MakeRequest<List<long>>(request, cancellationToken: cancellationToken) ?? new List<long>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<long>> RemoveMemberFromGroups(string memberId, List<long> groupIds, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/{memberId}/groups/remove"
            };
            request.AddUrlSegment("memberId", memberId);
            request.AddJsonBody(new { group_ids = groupIds });

            return await apiAdapter.MakeRequest<List<long>>(request, cancellationToken: cancellationToken) ?? new List<long>();
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAllMembers(MemberStatusShort memberStatusId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/members"
            };
            request.AddParameter("member_status_id", memberStatusId.ToEnumString());

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveMemberFromAllGroups(string memberId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/members/{memberId}/groups"
            };
            request.AddUrlSegment("memberId", memberId);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveMembersFromGroups(RemoveMemberGroups groups, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/groups/remove",
            };
            request.AddJsonBody(groups);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<int> GetMemberMailingHistoryCount(string memberId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/{memberId}/mailings"
            };
            request.AddUrlSegment("memberId", memberId);

            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MailingHistory>> GetMemberMailingHistory(string memberId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/{memberId}/mailings"
            };
            request.AddUrlSegment("memberId", memberId);

            return await apiAdapter.MakeRequest<List<MailingHistory>>(request, start, end, cancellationToken: cancellationToken) ?? new List<MailingHistory>();
        }

        /// <inheritdoc/>
        public async Task<int> GetMembersAffectedByImportCount(string importId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/imports/{importId}/members"
            };
            request.AddUrlSegment("importId", importId);

            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ImportMembers>> GetMembersAffectedByImport(string importId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/imports/{importId}/members"
            };
            request.AddUrlSegment("importId", importId);

            return await apiAdapter.MakeRequest<List<ImportMembers>>(request, start, end, cancellationToken: cancellationToken) ?? new List<ImportMembers>();
        }

        /// <inheritdoc/>
        public async Task<Import?> GetImportInformation(string importId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/imports/{importId}"
            };
            request.AddUrlSegment("importId", importId);

            return await apiAdapter.MakeRequest<Import>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<int> GetAllImportsCount(CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/imports"
            };

            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Import>> GetAllImports(uint? start = null, uint? end = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/members/imports"
            };

            return await apiAdapter.MakeRequest<List<Import>>(request, start, end, cancellationToken: cancellationToken) ?? new List<Import>();
        }

        /// <inheritdoc/>
        public async Task<bool> CopyMembersIntoStatusGroup(string groupId, CopyStatus status, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/members/{groupId}/copy"
            };
            request.AddUrlSegment("groupId", groupId);
            request.AddJsonBody(status);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateStatusOfGroupMembersBasedOnCurrentStatus(MemberStatusShort statusFrom, MemberStatusShort statusTo, string? groupId = null, CancellationToken cancellationToken = default)
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

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }
    }
}

