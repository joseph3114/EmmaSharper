using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EmmaSharper
{
    /// <summary>
    /// Provides access to response data. You can get overview numbers for all of your mailings and also drill down 
    /// into finding out the actual members who opened a particular mailing.
    /// </summary>
    public interface IEmmaResponseProvider
    {
        /// <summary>Get the list of clicks for this mailing/// </summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <param name="memberId">Optional. Limits results to a single member.</param>
        /// <param name="linkId">Optional. Limits results to a single link.</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>An array of link objects for the mailing.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ for
        /// standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseClicks>> GetMailingClicks(string mailingId, string? memberId = null, string? linkId = null, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get the count of the list of clicks for this mailing</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <returns>An array of link objects for the mailing.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ for 
        /// standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetMailingClicksCount(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get the customer share associated with the share id.</summary>
        /// <param name="shareId">Share Identifier</param>
        /// <returns>A customer share for the mailing.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid 
        /// mailing type - ‘m’ for standard mailings, ‘t’ for test mailings and ‘r’ for
        /// trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<ResponseCustomerShare?> GetMailingCustomerShare(string shareId, CancellationToken cancellationToken = default);

        /// <summary>Get the list of customer share clicks for this mailing.</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <returns>An array of customer share click objects for the mailing.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid
        /// mailing type - ‘m’ for standard mailings, ‘t’ for test mailings and ‘r’ for
        /// trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseCustomerShareClicks>> GetMailingCustomerShareClicks(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get the list of customer shares for this mailing.</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <returns>An array of customer shares objects for the mailing.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing 
        /// type - ‘m’ for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseCustomerShare>> GetMailingCustomerShares(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the list of messages that have finished delivery. This will include those that were 
        /// successfully delivered, as well as those that failed due to hard or soft bounces.
        /// </summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <param name="result">Optional. Accepted options: ‘all’, ‘delivered’, ‘bounced’, ‘hard’, ‘soft’. Defaults to ‘all’, if not provided.</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>An array of message responses that have finished delivery.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing 
        /// type - ‘m’ for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseDeliveries>> GetMailingDelieveries(string mailingId, DeliveryType result = DeliveryType.All, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the count of the list of messages that have finished delivery. This will include those that were successfully 
        /// delivered, as well as those that failed due to hard or soft bounces.
        /// </summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <param name="result">Optional. Accepted options: ‘all’, ‘delivered’, ‘bounced’, ‘hard’, ‘soft’. Defaults to ‘all’, if not provided.</param>
        /// <returns>An array of message responses that have finished delivery.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ 
        /// for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetMailingDelieveriesCount(string mailingId, DeliveryType result = DeliveryType.All, CancellationToken cancellationToken = default);

        /// <summary>Get the list of forwards for this mailing</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <returns>An array of forwards objects for the mailing.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’
        /// for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseForwards>> GetMailingForwards(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get the list of messages that are in the queue, possibly sent, but not yet delivered.</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>Get the list of messages that are in-progress.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ 
        /// for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseGeneric>> GetMailingInProgress(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get the count of the list of messages that are in the queue, possibly sent, but not yet delivered.</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <returns>Get the list of messages that are in-progress.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ for
        /// standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetMailingInProgressCount(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get the list of links for this mailing</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <returns>An array of link objects for the mailing.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ for standard
        /// mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<Link>> GetMailingLinks(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get the count of the list of opened messages for this campaign</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>Get the list of messages that opened.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ for standard
        /// mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseGeneric>> GetMailingOpens(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get the list of opened messages for this campaign.</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <returns>Get the list of messages that opened.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’
        /// for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetMailingOpensCount(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get the list of optouts for this mailing.</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>An array of optouts objects for the mailing.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ 
        /// for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseGeneric>> GetMailingOptouts(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get the count of the list of optouts for this mailing.</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <returns>An array of optouts objects for the mailing.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ 
        /// for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetMailingOptoutsCount(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the response summary for a particular mailing. This method will return the counts of each type of 
        /// response activity for a particular mailing.
        /// </summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <returns>A single mailing object.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ 
        /// for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<Response?> GetMailingResponse(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get the list of messages that have been sent to an MTA (Message Transfer Agent) for delivery.</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <param name="start">Start paging record at.</param>
        /// <param name="end">End paging record at.</param>
        /// <returns>Get the list of messages that have been sent to an MTA for delivery.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ 
        /// for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseGeneric>> GetMailingSends(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default);

        /// <summary>Get the count of the list of messages that have been sent to an MTA (Message Transfer Agent) for delivery.</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ 
        /// for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<int> GetMailingSendsCount(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get the list of shares for this mailing.</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <returns>An array of signups objects for the mailing.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid 
        /// mailing type - ‘m’ for standard mailings, ‘t’ for test mailings and ‘r’ for
        /// trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseShares>> GetMailingShares(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get overview of shares pertaining to this mailing_id.</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <returns>An array of share summary objects for the mailing, by network.</returns>
        /// <remarks>Http404 if the mailing does not exist. Http404 if the mailing is not valid.</remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseSharesOverview>> GetMailingSharesOverview(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>Get the list of signups for this mailing.</summary>
        /// <param name="mailingId">Mailing Identifier</param>
        /// <returns>An array of signups objects for the mailing.</returns>
        /// <remarks>
        /// Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ for 
        /// standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.
        /// </remarks>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseSignups>> GetMailingSignups(string mailingId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the response summary for an account. This method will return a month-based time series of data including sends, 
        /// opens, clicks, mailings, forwards, and opt-outs. Test mailings and forwards are not included in the data returned.
        /// </summary>
        /// <param name="includeArchived">Optional flag to include archived mailings in the list.</param>
        /// <param name="range">Optional DateRange object to build the range parameter.</param>
        /// <returns>A list of objects with each object representing one month.</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseSummary>> GetResponseSummary(DateRange? range = null, bool includeArchived = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the response summary for an account. This method will return a month-based time series of data including sends, 
        /// opens, clicks, mailings, forwards, and opt-outs. Test mailings and forwards are not included in the data returned.
        /// </summary>
        /// <param name="includeArchived">Optional flag to include archived mailings in the list.</param>
        /// <param name="date">Optional date to build the range parameter.</param>
        /// <returns>A list of objects with each object representing one month.</returns>
        /// <param name="cancellationToken">Cancels the in-flight request.</param>
        Task<IEnumerable<ResponseSummary>> GetResponseSummary(DateTime? date, bool includeArchived = false, CancellationToken cancellationToken = default);
    }
}
