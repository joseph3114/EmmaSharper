using System.Threading;
﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmmaSharper
{
    /// <summary>
    /// Provides a way to retrieve information about your mailings including their HTML contents. You
    /// can retrieve the members to whom the mailing was sent. You can also pause mailings and cancel mailings
    /// that are pending or paused.
    /// </summary>
    public interface IEmmaMailingProvider
    {
        /// <summary>Get number of current mailings</summary>
        /// <param name="mailingTypes">Accepts a List with one or more of the following mailing types: ‘m’ (standard), ‘t’ (test), ‘r’ (trigger), ‘s’ (split). Defaults to ‘m,t’, standard and test mailings, when none are specified.</param>
        /// <param name="mailingStatuses">Accepts a List with one or more of the following mailing statuses: ‘p’ (pending), ‘a’ (paused), ‘s’ (sending), ‘x’ (canceled), ‘c’ (complete), ‘f’ (failed). Defaults to ‘p,a,s,x,c,f’, all statuses, when none are specified.</param>
        /// <param name="includeArchived">Boolean. Optional flag to include archived mailings in the list.</param>
        /// <param name="includeScheduled">Boolean. Mailings that have a scheduled timestamp.</param>
        /// <param name="includeHtmlBody">Boolean. Include the html_body content.</param>
        /// <param name="includePlaintext">Boolean. Include the plaintext content.</param>
        /// <returns>An number of mailings.</returns>
        /// <remarks>Http400 if invalid mailing types or statuses are specified.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> ListMailingsCount(IEnumerable<MailingType>? mailingTypes = null, IEnumerable<MailingStatus>? mailingStatuses = null, bool includeArchived = false, bool includeScheduled = false, bool includeHtmlBody = false, bool includePlaintext = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get information about current mailings. Be sure to get a count of mailings before accessing this method, so 
        /// you're aware of paging requirements.
        /// </summary>
        /// <param name="mailingTypes">
        /// Accepts a List with one or more of the following mailing types: ‘m’ (standard), ‘t’ (test), ‘r’ (trigger),
        /// ‘s’ (split). Defaults to ‘m,t’, standard and test mailings, when none are specified.
        /// </param>
        /// <param name="mailingStatuses">
        /// Accepts a List with one or more of the following mailing statuses: ‘p’ (pending), ‘a’ (paused), ‘s’ (sending),
        /// ‘x’ (canceled), ‘c’ (complete), ‘f’ (failed). Defaults to ‘p,a,s,x,c,f’, all statuses, when none are specified.
        /// </param>
        /// <param name="includeArchived">Boolean. Optional flag to include archived mailings in the list.</param>
        /// <param name="includeScheduled">Boolean. Mailings that have a scheduled timestamp.</param>
        /// <param name="includeHtmlBody">Boolean. Include the html_body content.</param>
        /// <param name="includePlaintext">Boolean. Include the plaintext content.</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>An array of mailings.</returns>
        /// <remarks>Http400 if invalid mailing types or statuses are specified.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<MailingInfo>> ListMailings(IEnumerable<MailingType>? mailingTypes = null, IEnumerable<MailingStatus>? mailingStatuses = null, bool includeArchived = false, bool includeScheduled = false, bool includeHtmlBody = false, bool includePlaintext = false, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get detailed information for one mailing.</summary>
        /// <returns>The mailing.</returns>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <remarks>Http404 if no mailing is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Mailing?> GetMailing(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get the count of members to whom the given mailing was sent. This does not include groups or searches.</summary>
        /// <returns>An array of members including status and member fields.</returns>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <remarks>Http404 if no mailing is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetMailingMembersCount(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get the list of members to whom the given mailing was sent. This does not include groups or searches.</summary>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>An array of members including status and member fields.</returns>
        /// <remarks>Http404 if no mailing is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Member>> GetMailingMembers(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Gets the personalized message content as sent to a specific member as part of the specified mailing.</summary>
        /// <returns>Message content from a mailing, personalized for a member. The response will contain all parts of the mailing content by default, or just the type of content specified by type..</returns>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <param name="memberId">Member identifier.</param>
        /// <param name="type">Accepts: ‘all’, ‘html’, ‘plaintext’, ‘subject’. Defaults to ‘all’, if not provided.</param>
        /// <remarks>Http404 if no mailing is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<MailingPersonalization?> GetMailingMembersPersonalization(string mailingId, string memberId, PersonalizationType? type = null, CancellationToken cancellationToken = default);

        /// <summary>Get the count of groups to which a particular mailing was sent.</summary>
        /// <returns>An array of groups.</returns>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <remarks>Http404 if no mailing is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetMailingGroupsCount(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get the groups to which a particular mailing was sent.</summary>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>An array of groups.</returns>
        /// <remarks>Http404 if no mailing is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Group>> GetMailingGroups(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get all searches associated with a sent mailing.</summary>
        /// <returns>An array of searches.</returns>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <remarks>Http404 if no mailing is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Search>> GetMailingSearches(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Update status of a current mailing.</summary>
        /// <returns>Returns the mailing’s new status.</returns> 
        /// <param name="mailingId">Mailing identifier.</param>
        /// <param name="status">The status can be one of canceled, paused or ready. This method can be used to control the progress of a mailing by pausing, canceling or resuming it. Once a mailing is canceled it can’t be resumed, and will not show in the normal mailing_list output.</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<UpdateMailing?> UpdateMailingStatus(string mailingId, UpdateMailingStatus status, CancellationToken cancellationToken = default);

        /// <summary>Sets archived timestamp for a mailing so it is no longer included in mailing_list.</summary>
        /// <returns>True if the mailing is successfully archived.</returns>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> ArchiveMailing(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Cancels a mailing that has a current status of pending or paused. All other statuses will result in a 404.</summary>
        /// <returns>True if mailing marked as cancelled.</returns>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> CancelMailing(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Forward a previous message to additional recipients. If these recipients are not already in the 
        /// audience, they will be added with a status of FORWARDED.
        /// </summary>
        /// <returns>A reference to the new mailing.</returns>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <param name="memberId">Member identifier.</param>
        /// <param name="mailing">Class representing the fields to forward and email to additional recipients.</param>
        /// <remarks>Http404 if no message is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<MailingIdentifier?> ForwardMailing(string mailingId, string memberId, ForwardMailing mailing, CancellationToken cancellationToken = default);

        /// <summary>Send a prior mailing to additional recipients. A new mailing will be created that inherits its content from the original.</summary>
        /// <returns>The mailing id of the new mailing.</returns>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <param name="mailing">Class representing the available fields when resending a mailing.</param>
        /// <remarks>Http404 if no message is found.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<MailingIdentifier?> ResendMailing(string mailingId, ResendMailing mailing, CancellationToken cancellationToken = default);

        /// <summary>
        /// Declare the winner of a split test manually. In the event that the test duration has not elapsed, 
        /// the current stats for each test will be frozen and the content defined in the user declared winner 
        /// will sent to the remaining members for the mailing. Please note, any messages that are pending for 
        /// each of the test variations will receive the content assigned to them when the test was initially
        /// constructed.
        /// </summary>
        /// <returns><c>true</c>, if winner was declared, <c>false</c> otherwise.</returns>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <param name="winnerId">Winner identifier.</param>
        /// <remarks>Http403 if the winner cannot be manually declared.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> DeclareWinner(string mailingId, string winnerId, CancellationToken cancellationToken = default);

        /// <summary>Get heads up email address(es) related to a mailing.</summary>
        /// <returns>An array of heads up email addresses.</returns>
        /// <param name="mailingId">Mailing identifier.</param>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<MailingHeadsUp>> GetHeadsUpEmailsForMailing(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Validate that a mailing has valid personalization-tag syntax. Checks tag syntax in three params:</summary>
        /// <returns><c>true</c>, if personalization syntax was validated, <c>false</c> otherwise.</returns>
        /// <param name="personalization">HTML body, plaintext body and subject line for personalization testing.</param>
        /// <remarks>Http400 if any tags are invalid. The response body will have information about the invalid tags.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<bool> VaildatePersonalizationSyntax(MailingPersonalization personalization, CancellationToken cancellationToken = default);
    }
}