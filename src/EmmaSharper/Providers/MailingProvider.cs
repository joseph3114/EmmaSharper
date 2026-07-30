using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmmaSharper.Internals;

namespace EmmaSharper
{
    /// <inheritdoc/>
    internal class MailingProvider : IEmmaMailingProvider
    {
        private readonly IEmmaApiAdapter apiAdapter;

        /// <inheritdoc cref="object.Object"/>
        public MailingProvider(IEmmaApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }

        /// <inheritdoc/>
        public async Task<int> ListMailingsCount(IEnumerable<MailingType>? mailingTypes = null, IEnumerable<MailingStatus>? mailingStatuses = null, bool includeArchived = false, bool includeScheduled = false, bool includeHtmlBody = false, bool includePlaintext = false, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/mailings"
            };
            request.AddParameter("count", "true");

            if (mailingTypes != null)
            {
                string values = mailingTypes.AsEnumStrings().JoinWith(',');
                request.AddParameter("mailing_types", values);
            }

            if (mailingStatuses != null)
            {
                string values = mailingStatuses.AsEnumStrings().JoinWith(',');
                request.AddParameter("mailing_statuses", values);
            }

            if (includeArchived)
            {
                request.AddParameter("include_archived", includeArchived);
            }

            if (includeScheduled)
            {
                request.AddParameter("is_scheduled", includeScheduled);
            }

            if (includeHtmlBody)
            {
                request.AddParameter("with_html_body", includeHtmlBody);
            }

            if (includePlaintext)
            {
                request.AddParameter("with_plaintext", includePlaintext);
            }

            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MailingInfo>> ListMailings(IEnumerable<MailingType>? mailingTypes = null, IEnumerable<MailingStatus>? mailingStatuses = null, bool includeArchived = false, bool includeScheduled = false, bool includeHtmlBody = false, bool includePlaintext = false, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/mailings"
            };

            if (mailingTypes != null)
            {
                string values = mailingTypes.AsEnumStrings().JoinWith(',');
                request.AddParameter("mailing_types", values);
            }

            if (mailingStatuses != null)
            {
                string values = mailingStatuses.AsEnumStrings().JoinWith(',');
                request.AddParameter("mailing_statuses", values);
            }

            if (includeArchived)
            {
                request.AddParameter("include_archived", includeArchived);
            }

            if (includeScheduled)
            {
                request.AddParameter("is_scheduled", includeScheduled);
            }

            if (includeHtmlBody)
            {
                request.AddParameter("with_html_body", includeHtmlBody);
            }

            if (includePlaintext)
            {
                request.AddParameter("with_plaintext", includePlaintext);
            }

            return await apiAdapter.MakeRequest<List<MailingInfo>>(request, start, end, cancellationToken: cancellationToken) ?? new List<MailingInfo>();
        }

        /// <inheritdoc/>
        public async Task<Mailing?> GetMailing(string mailingId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/mailings/{mailingId}"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<Mailing>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<int> GetMailingMembersCount(string mailingId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/mailings/{mailingId}/members"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Member>> GetMailingMembers(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/mailings/{mailingId}/members"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<Member>>(request, start, end, cancellationToken: cancellationToken) ?? new List<Member>();
        }

        /// <inheritdoc/>
        public async Task<MailingPersonalization?> GetMailingMembersPersonalization(string mailingId, string memberId, PersonalizationType? type = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/mailings/{mailingId}/messages/{memberId}"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddUrlSegment("memberId", memberId);

            if (type != null)
            {
                request.AddParameter("type", type);
            }

            return await apiAdapter.MakeRequest<MailingPersonalization>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<int> GetMailingGroupsCount(string mailingId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/mailings/{mailingId}/groups"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddParameter("count", "true");

            return await apiAdapter.MakeRequest<int>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Group>> GetMailingGroups(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/mailings/{mailingId}/groups"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<Group>>(request, start, end, cancellationToken: cancellationToken) ?? new List<Group>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Search>> GetMailingSearches(string mailingId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/mailings/{mailingId}/searches"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<Search>>(request, cancellationToken: cancellationToken) ?? new List<Search>();
        }

        /// <inheritdoc/>
        public async Task<UpdateMailing?> UpdateMailingStatus(string mailingId, UpdateMailingStatus status, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.PUT)
            {
                Resource = "/{accountId}/mailings/{mailingId}"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddJsonBody(new { status = status.ToEnumString() });

            return await apiAdapter.MakeRequest<UpdateMailing>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> ArchiveMailing(string mailingId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/mailings/{mailingId}"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> CancelMailing(string mailingId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.DELETE)
            {
                Resource = "/{accountId}/mailings/cancel/{mailingId}"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<MailingIdentifier?> ForwardMailing(string mailingId, string memberId, ForwardMailing mailing, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/forwards/{mailingId}/{memberId}"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddUrlSegment("memberId", memberId);
            request.AddJsonBody(mailing);

            return await apiAdapter.MakeRequest<MailingIdentifier>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<MailingIdentifier?> ResendMailing(string mailingId, ResendMailing mailing, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/mailings/{mailingId}"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddJsonBody(mailing);

            return await apiAdapter.MakeRequest<MailingIdentifier>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MailingHeadsUp>> GetHeadsUpEmailsForMailing(string mailingId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest
            {
                Resource = "/{accountId}/mailings/{mailingId}/headsup"
            };
            request.AddUrlSegment("mailingId", mailingId);

            return await apiAdapter.MakeRequest<List<MailingHeadsUp>>(request, cancellationToken: cancellationToken) ?? new List<MailingHeadsUp>();
        }

        /// <inheritdoc/>
        public async Task<bool> VaildatePersonalizationSyntax(MailingPersonalization personalization, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/mailings/validate",
            };
            request.AddJsonBody(personalization);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> DeclareWinner(string mailingId, string winnerId, CancellationToken cancellationToken = default)
        {
            EmmaRequest request = new EmmaRequest(Method.POST)
            {
                Resource = "/{accountId}/mailings/{mailingId}/winner/{winnerId}"
            };
            request.AddUrlSegment("mailingId", mailingId);
            request.AddUrlSegment("winnerId", winnerId);

            return await apiAdapter.MakeRequest<bool>(request, cancellationToken: cancellationToken);
        }
    }
}
