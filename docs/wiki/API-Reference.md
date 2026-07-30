# API Reference

Every public type and member, generated from the compiled assembly and its XML
documentation. Regenerated at release, so it cannot drift from the code.

`EmmaSharper` 8.0.0

## Contents

**Providers**  
[IEmmaAccountProvider](#iemmaaccountprovider) · [IEmmaAccountScope](#iemmaaccountscope) · [IEmmaAccountScopeFactory](#iemmaaccountscopefactory) · [IEmmaAutomationProvider](#iemmaautomationprovider) · [IEmmaEnterpriseProvider](#iemmaenterpriseprovider) · [IEmmaFieldsProvider](#iemmafieldsprovider) · [IEmmaGroupProvider](#iemmagroupprovider) · [IEmmaMailingProvider](#iemmamailingprovider) · [IEmmaMemberProvider](#iemmamemberprovider) · [IEmmaResponseProvider](#iemmaresponseprovider) · [IEmmaSearchProvider](#iemmasearchprovider) · [IEmmaSignupFormProvider](#iemmasignupformprovider) · [IEmmaSubscriptionProvider](#iemmasubscriptionprovider) · [IEmmaWebhookProvider](#iemmawebhookprovider)

**Configuration and helpers**  
[EmmaRetryDefaults](#emmaretrydefaults) · [EmmaSharperExtensions](#emmasharperextensions) · [EmmaOptions](#emmaoptions)

**Exceptions**  
[EmmaException](#emmaexception) · [EmmaRateLimitException](#emmaratelimitexception)

**Enums**  
[DeliveryType](#deliverytype) · [DeliveryTypeShort](#deliverytypeshort) · [FieldType](#fieldtype) · [GroupType](#grouptype) · [ImportChangeType](#importchangetype) · [ImportStatus](#importstatus) · [MailingStatus](#mailingstatus) · [MailingType](#mailingtype) · [MemberFieldSelection](#memberfieldselection) · [MemberStatus](#memberstatus) · [MemberStatusShort](#memberstatusshort) · [PersonalizationType](#personalizationtype) · [SubaccountStatusFilter](#subaccountstatusfilter) · [UpdateMailingStatus](#updatemailingstatus) · [WebhookMethod](#webhookmethod) · [WidgetType](#widgettype) · [WorkflowStatus](#workflowstatus)

**Models** — 80 types, listed at the end.

---

## Providers

### IEmmaAccountProvider

Account-level endpoints for the account currently in scope.

#### `Task<IReadOnlyList<AccountUser>> ListUsers(CancellationToken cancellationToken = default)`

Lists the users with access to this account.

| Parameter | |
|---|---|
| `cancellationToken` | Cancels the in-flight request. |

### IEmmaAccountScope

The full set of Emma providers, bound to one specific account id.

> Obtained from `String)`. A scope reuses the
> same credentials and the same pooled `HttpClient` as the
> default providers - only the account segment of the request path differs.

| Property | Type | |
|---|---|---|
| `Account` | `IEmmaAccountProvider` | Account-level endpoints, such as the account's users. |
| `AccountId` | `string` | The account id every call made through this scope targets. |
| `Automation` | `IEmmaAutomationProvider` | Automation workflows for this account. |
| `Enterprise` | `IEmmaEnterpriseProvider` | Enterprise endpoints. Only meaningful when the scoped account is itself an enterprise account; scoping to a subaccount and calling this will not list that subaccount's peers. |
| `Fields` | `IEmmaFieldsProvider` | Member fields for this account. |
| `Groups` | `IEmmaGroupProvider` | Groups for this account. |
| `Mailings` | `IEmmaMailingProvider` | Mailings for this account. |
| `Members` | `IEmmaMemberProvider` | Members for this account. |
| `Responses` | `IEmmaResponseProvider` | Mailing response data for this account. |
| `Searches` | `IEmmaSearchProvider` | Saved searches for this account. |
| `SignupForms` | `IEmmaSignupFormProvider` | Signup forms for this account. |
| `Subscriptions` | `IEmmaSubscriptionProvider` | Subscriptions for this account. |
| `Webhooks` | `IEmmaWebhookProvider` | Webhooks for this account. |

### IEmmaAccountScopeFactory

Creates provider sets bound to a specific Emma account, using the configured credentials.

> Emma enterprise accounts authenticate once and then address many subaccounts. A scope
> reuses the configured credentials and the same pooled HttpClient, changing only the
> account segment of the request path, so creating one per subaccount is cheap.
> ```csharp
> foreach (Subaccount sub in await enterprise.ListSubaccounts(cancellationToken: ct))
> {
>     IEmmaAccountScope scope = scopeFactory.ForAccount(sub.AccountId!);
>     int active = await scope.Members.GetMemberCount(cancellationToken: ct);
> }
> ```

#### `IEmmaAccountScope ForAccount(string accountId)`

Returns providers that address `accountId`.

| Parameter | |
|---|---|
| `accountId` | The Emma account or subaccount id to target. |

> Cheap - no new HTTP client or handler is created.

### IEmmaAutomationProvider

Provides access to automation APIs

#### `Task<Workflow?> GetWorkflowById(string workflowId, CancellationToken cancellationToken = default)`

Gets detailed information about a single workflow

| Parameter | |
|---|---|
| `workflowId` | The ID of the Workflow to return. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A single workflow if one exists

#### `Task<WorkflowCount?> GetWorkflowCounts(CancellationToken cancellationToken = default)`

Gets a count of this account’s automation workflows.

**Returns** — A count of automation workflows in the given account.

#### `Task<IEnumerable<Workflow>> GetWorkflows(CancellationToken cancellationToken = default)`

Gets a list of this account’s automation workflows.

**Returns** — A list of automation workflows in the given account.

### IEmmaEnterpriseProvider

Enterprise-level endpoints, which operate across subaccounts.

> Requires enterprise credentials. Pair this with
> `IEmmaAccountScopeFactory` to discover subaccounts and then address each one.

#### `Task<IReadOnlyList<Subaccount>> ListSubaccounts(SubaccountStatusFilter status = 15, CancellationToken cancellationToken = default)`

Lists the subaccounts belonging to this enterprise account.

| Parameter | |
|---|---|
| `status` | Which lifecycle states to include. Defaults to `All` so nothing is dropped without the caller asking for it. |
| `cancellationToken` | Cancels the in-flight request. |

### IEmmaFieldsProvider

Provides access to custom fields in your account. Of particular interest is the `CancellationToken)`
method which lets you clear out all the data in a single field for all members in your account. This makes
it easy to re-initialize a dataset if you’re looking to correct an import error or syncing issue

#### `Task<bool> ClearField(string fieldId, CancellationToken cancellationToken = default)`

Clear the member data for the specified field

| Parameter | |
|---|---|
| `fieldId` | The Field Id of the field to clear. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if all of the member field data is deleted

#### `Task<int> CreateField(CreateField field, CancellationToken cancellationToken = default)`

Create a new field. There must not already be a field with this name

| Parameter | |
|---|---|
| `field` | The Field to be created. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A reference (Field ID as int) to the new field.

#### `Task<bool> DeleteField(string fieldId, CancellationToken cancellationToken = default)`

Deletes a field

| Parameter | |
|---|---|
| `fieldId` | The Field Id of the field to delete. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if the field is deleted, False otherwise.

#### `Task<Field?> GetField(string fieldId, bool includeDeleted = false, CancellationToken cancellationToken = default)`

Gets the detailed information about a particular field

| Parameter | |
|---|---|
| `fieldId` | The Field Id of the field to retrieve. |
| `includeDeleted` | Accepts True. Optionally show a field even if it has been deleted. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A field.

> Http404 if the field does not exist.

#### `Task<IEnumerable<Field>> ListFields(bool includeDeleted = false, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Gets a list of this account's defined fields. Be sure to get a count of fields before accessing this method,
so you're aware of paging requirements

| Parameter | |
|---|---|
| `includeDeleted` | Accepts True. Optional flag to include deleted fields |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of fields.

#### `Task<int> ListFieldsCount(bool includeDeleted = false, CancellationToken cancellationToken = default)`

Gets number of fields for paging

| Parameter | |
|---|---|
| `includeDeleted` | Accepts True. Optional flag to include deleted fields |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of fields.

#### `Task<int> UpdateField(string fieldId, UpdateField field, CancellationToken cancellationToken = default)`

Updates an existing field

| Parameter | |
|---|---|
| `fieldId` | The Field Id of the field to update. |
| `field` | The Field to be updated. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A reference (Field ID as int) to the updated field.

### IEmmaGroupProvider

Provides access to manage all aspects of the groups in your account. In addition to various CRUD
methods, you can also use these endpoints to manage the members of your groups. You’ll want to use these
methods if you’re managing group membership for more than one member at a time. For dealing with single
members, there are better methods in the members endpoints.

#### `Task<IEnumerable<long>> AddMembersToGroup(string memberGroupId, MemberIdList memberIds, CancellationToken cancellationToken = default)`

Add a list of members to a single active member group

| Parameter | |
|---|---|
| `memberGroupId` | The Member Group Id to be retrieved. |
| `memberIds` | An array of member ids. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of references to the members added to the group. If a member already exists in the group or is not a valid member, that reference will not be returned.

> Http404 if the group does not exist.

#### `Task<bool> CopyUsersFromGroup(string fromGroupId, string toGroupId, MemberStatusShortList status, CancellationToken cancellationToken = default)`

Copy all the users of one group into another group

| Parameter | |
|---|---|
| `fromGroupId` | The Member Group ID to be copied from. |
| `toGroupId` | The Member Group ID to be copied to. |
| `status` | An Array of Member Status strings. This is ‘a’ (active), ‘o’ (optout), or ‘e’ (error). |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Returns true.

> Http404 if the group does not exist.

#### `Task<IEnumerable<Group>> CreateGroups(IEnumerable<GroupName> groups, CancellationToken cancellationToken = default)`

Create one or more new member groups

| Parameter | |
|---|---|
| `groups` | A Group to be created. Each object must contain a group_name parameter. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of the new group ids and group names.

#### `Task<bool> DeleteAllFromMemberGroupsByStatus(string memberGroupId, MemberStatusShort status, CancellationToken cancellationToken = default)`

Delete all members in this group with the specified status. Then, remove those members from all active
member groups as a background job. The member_status_id parameter must be set.

| Parameter | |
|---|---|
| `memberGroupId` | The Member Group Id to be retrieved. |
| `status` | A Member Status string. This is ‘a’ (active), ‘o’ (optout), or ‘e’ (error). |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Returns true.

> Http404 if the group does not exist.

#### `Task<int> DeleteAllMembersFromGroup(string memberGroupId, MemberStatusShort? status = null, CancellationToken cancellationToken = default)`

Remove all members from a single active member group

| Parameter | |
|---|---|
| `memberGroupId` | The Member Group Id to be retrieved. |
| `status` | A Member Status string. Optional. This is ‘a’ (active), ‘o’ (optout), or ‘e’ (error). |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Returns the number of members removed from the group.

> Http404 if the group does not exist.

#### `Task<bool> DeleteGroup(string memberIdGroup, CancellationToken cancellationToken = default)`

Delete a single member group

| Parameter | |
|---|---|
| `memberIdGroup` | The Member Group Id to be deleted. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if the group is deleted.

> Http404 if the group does not exist.

#### `Task<Group?> GetGroup(string memberGroupId, CancellationToken cancellationToken = default)`

Get the detailed information for a single member group

| Parameter | |
|---|---|
| `memberGroupId` | The Member Group Id to be retrieved. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A group.

> Http404 if the group does not exist.

#### `Task<int> ListGroupCount(IEnumerable<GroupType>? groupType = null, CancellationToken cancellationToken = default)`

Get number of all active member groups for a single account

**Returns** — An int of groups.

#### `Task<IEnumerable<Member>> ListGroupMembers(string memberGroupId, bool includeDeleted = false, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get the members in a single active member group

| Parameter | |
|---|---|
| `memberGroupId` | The Member Group Id to be retrieved. |
| `includeDeleted` | Include deleted members. Optional, defaults to false. |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of members.

> Http404 if the group does not exist.

#### `Task<int> ListGroupMembersCount(string memberGroupId, bool includeDeleted = false, CancellationToken cancellationToken = default)`

Get the count of members in a single active member group

| Parameter | |
|---|---|
| `memberGroupId` | The Member Group Id to be retrieved. |
| `includeDeleted` | Include deleted members. Optional, defaults to false. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of members.

> Http404 if the group does not exist.

#### `Task<IEnumerable<Group>> ListGroups(IEnumerable<GroupType>? groupType = null, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get a basic listing of all active member groups for a single account. Be sure to get a count of groups
before accessing this method, so you're aware of paging requirements.

| Parameter | |
|---|---|
| `groupType` | Accepts a comma-separated string with one or more GroupTypes. Defaults to Group. |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of groups.

#### `Task<IEnumerable<long>> RemoveMembersFromGroup(string memberGroupId, MemberIdList memberIds, CancellationToken cancellationToken = default)`

Remove members from a single active member group

| Parameter | |
|---|---|
| `memberGroupId` | The Member Group Id to be retrieved. |
| `memberIds` | An array of member ids. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of references to the removed members.

> Http404 if the group does not exist.

#### `Task<bool> UpdateGroup(string memberGroupId, UpdateGroup group, CancellationToken cancellationToken = default)`

Update information for a single member group

| Parameter | |
|---|---|
| `memberGroupId` | The Member Group Id to be updated. |
| `group` | The Group to be updated. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if the update was successful

> Http404 if the group does not exist.

### IEmmaMailingProvider

Provides a way to retrieve information about your mailings including their HTML contents. You
can retrieve the members to whom the mailing was sent. You can also pause mailings and cancel mailings
that are pending or paused.

#### `Task<bool> ArchiveMailing(string mailingId, CancellationToken cancellationToken = default)`

Sets archived timestamp for a mailing so it is no longer included in mailing_list.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if the mailing is successfully archived.

#### `Task<bool> CancelMailing(string mailingId, CancellationToken cancellationToken = default)`

Cancels a mailing that has a current status of pending or paused. All other statuses will result in a 404.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if mailing marked as cancelled.

#### `Task<bool> DeclareWinner(string mailingId, string winnerId, CancellationToken cancellationToken = default)`

Declare the winner of a split test manually. In the event that the test duration has not elapsed,
the current stats for each test will be frozen and the content defined in the user declared winner
will sent to the remaining members for the mailing. Please note, any messages that are pending for
each of the test variations will receive the content assigned to them when the test was initially
constructed.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `winnerId` | Winner identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — `true`, if winner was declared, `false` otherwise.

> Http403 if the winner cannot be manually declared.

#### `Task<MailingIdentifier?> ForwardMailing(string mailingId, string memberId, ForwardMailing mailing, CancellationToken cancellationToken = default)`

Forward a previous message to additional recipients. If these recipients are not already in the
audience, they will be added with a status of FORWARDED.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `memberId` | Member identifier. |
| `mailing` | Class representing the fields to forward and email to additional recipients. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A reference to the new mailing.

> Http404 if no message is found.

#### `Task<IEnumerable<MailingHeadsUp>> GetHeadsUpEmailsForMailing(string mailingId, CancellationToken cancellationToken = default)`

Get heads up email address(es) related to a mailing.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of heads up email addresses.

#### `Task<Mailing?> GetMailing(string mailingId, CancellationToken cancellationToken = default)`

Get detailed information for one mailing.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — The mailing.

> Http404 if no mailing is found.

#### `Task<IEnumerable<Group>> GetMailingGroups(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get the groups to which a particular mailing was sent.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of groups.

> Http404 if no mailing is found.

#### `Task<int> GetMailingGroupsCount(string mailingId, CancellationToken cancellationToken = default)`

Get the count of groups to which a particular mailing was sent.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of groups.

> Http404 if no mailing is found.

#### `Task<IEnumerable<Member>> GetMailingMembers(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get the list of members to whom the given mailing was sent. This does not include groups or searches.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of members including status and member fields.

> Http404 if no mailing is found.

#### `Task<int> GetMailingMembersCount(string mailingId, CancellationToken cancellationToken = default)`

Get the count of members to whom the given mailing was sent. This does not include groups or searches.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of members including status and member fields.

> Http404 if no mailing is found.

#### `Task<MailingPersonalization?> GetMailingMembersPersonalization(string mailingId, string memberId, PersonalizationType? type = null, CancellationToken cancellationToken = default)`

Gets the personalized message content as sent to a specific member as part of the specified mailing.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `memberId` | Member identifier. |
| `type` | Accepts: ‘all’, ‘html’, ‘plaintext’, ‘subject’. Defaults to ‘all’, if not provided. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Message content from a mailing, personalized for a member. The response will contain all parts of the mailing content by default, or just the type of content specified by type..

> Http404 if no mailing is found.

#### `Task<IEnumerable<Search>> GetMailingSearches(string mailingId, CancellationToken cancellationToken = default)`

Get all searches associated with a sent mailing.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of searches.

> Http404 if no mailing is found.

#### `Task<IEnumerable<MailingInfo?>?> ListMailings(IEnumerable<MailingType>? mailingTypes = null, IEnumerable<MailingStatus>? mailingStatuses = null, bool includeArchived = false, bool includeScheduled = false, bool includeHtmlBody = false, bool includePlaintext = false, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get information about current mailings. Be sure to get a count of mailings before accessing this method, so
you're aware of paging requirements.

| Parameter | |
|---|---|
| `mailingTypes` | Accepts a List with one or more of the following mailing types: ‘m’ (standard), ‘t’ (test), ‘r’ (trigger), ‘s’ (split). Defaults to ‘m,t’, standard and test mailings, when none are specified. |
| `mailingStatuses` | Accepts a List with one or more of the following mailing statuses: ‘p’ (pending), ‘a’ (paused), ‘s’ (sending), ‘x’ (canceled), ‘c’ (complete), ‘f’ (failed). Defaults to ‘p,a,s,x,c,f’, all statuses, when none are specified. |
| `includeArchived` | Boolean. Optional flag to include archived mailings in the list. |
| `includeScheduled` | Boolean. Mailings that have a scheduled timestamp. |
| `includeHtmlBody` | Boolean. Include the html_body content. |
| `includePlaintext` | Boolean. Include the plaintext content. |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of mailings.

> Http400 if invalid mailing types or statuses are specified.

#### `Task<int> ListMailingsCount(IEnumerable<MailingType>? mailingTypes = null, IEnumerable<MailingStatus>? mailingStatuses = null, bool includeArchived = false, bool includeScheduled = false, bool includeHtmlBody = false, bool includePlaintext = false, CancellationToken cancellationToken = default)`

Get number of current mailings

| Parameter | |
|---|---|
| `mailingTypes` | Accepts a List with one or more of the following mailing types: ‘m’ (standard), ‘t’ (test), ‘r’ (trigger), ‘s’ (split). Defaults to ‘m,t’, standard and test mailings, when none are specified. |
| `mailingStatuses` | Accepts a List with one or more of the following mailing statuses: ‘p’ (pending), ‘a’ (paused), ‘s’ (sending), ‘x’ (canceled), ‘c’ (complete), ‘f’ (failed). Defaults to ‘p,a,s,x,c,f’, all statuses, when none are specified. |
| `includeArchived` | Boolean. Optional flag to include archived mailings in the list. |
| `includeScheduled` | Boolean. Mailings that have a scheduled timestamp. |
| `includeHtmlBody` | Boolean. Include the html_body content. |
| `includePlaintext` | Boolean. Include the plaintext content. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An number of mailings.

> Http400 if invalid mailing types or statuses are specified.

#### `Task<MailingIdentifier?> ResendMailing(string mailingId, ResendMailing mailing, CancellationToken cancellationToken = default)`

Send a prior mailing to additional recipients. A new mailing will be created that inherits its content from the original.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `mailing` | Class representing the available fields when resending a mailing. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — The mailing id of the new mailing.

> Http404 if no message is found.

#### `Task<UpdateMailing?> UpdateMailingStatus(string mailingId, UpdateMailingStatus status, CancellationToken cancellationToken = default)`

Update status of a current mailing.

| Parameter | |
|---|---|
| `mailingId` | Mailing identifier. |
| `status` | The status can be one of canceled, paused or ready. This method can be used to control the progress of a mailing by pausing, canceling or resuming it. Once a mailing is canceled it can’t be resumed, and will not show in the normal mailing_list output. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Returns the mailing’s new status.

#### `Task<bool> VaildatePersonalizationSyntax(MailingPersonalization personalization, CancellationToken cancellationToken = default)`

Validate that a mailing has valid personalization-tag syntax. Checks tag syntax in three params:

| Parameter | |
|---|---|
| `personalization` | HTML body, plaintext body and subject line for personalization testing. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — `true`, if personalization syntax was validated, `false` otherwise.

> Http400 if any tags are invalid. The response body will have information about the invalid tags.

### IEmmaMemberProvider

In addition to the various CRUD endpoints here related to members, you can also change the status of members, including
opting them out. You’ll notice that there are calls related to individual members, but we also provide quite a few calls
to deal with bulk updates of members. Please try to use these whenever possible as opposed to looping through a list of
members and calling the individual member calls. Where this is especially important is when adding new members. To do a
bulk import, you’ll POST to the `CancellationToken)` method. In return, you’ll receive an import ID. You can use
this ID to check the status and results of your import. Imports are generally pretty fast, but the time to completion
can vary with greater system usage.

#### `Task<IEnumerable<long>> AddMemberToGroups(string memberId, IEnumerable<long> groupIds, CancellationToken cancellationToken = default)`

Add a single member to one or more groups.

| Parameter | |
|---|---|
| `memberId` | Member identifier. |
| `groupIds` | Group ids to which to add this member. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of ids of the affected groups.

> Http404 if no member is found.

#### `Task<MembersAdd?> AddNewMembers(AddMembers members, CancellationToken cancellationToken = default)`

Add new members or update existing members in bulk. If you are doing actions for a single member please see the `CancellationToken)` method.

| Parameter | |
|---|---|
| `members` | An array of members to update. A member is a dictionary of member emails and field values to import. The only required field is “email”. All other fields are treated as the name of a member field. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An import id.

#### `Task<MemberAdd?> AddOrUpdateSingleMember(AddMember member, CancellationToken cancellationToken = default)`

Adds or updates a single audience member. If you are performing actions on bulk members please use the `CancellationToken)` method.

| Parameter | |
|---|---|
| `member` | Fields related to adding or updating a Member. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — The member_id of the new or updated member, whether the member was added or an existing member was updated, and the status of the member. The status will be reported as ‘a’ (active), ‘e’ (error), or ‘o’ (optout).

#### `Task<bool> ChangeMemberStatus(ChangeStatus status, CancellationToken cancellationToken = default)`

Change the status for an array of members. The members will have their member_status_id update

| Parameter | |
|---|---|
| `status` | Class representing members and their new status. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if the members are successfully updated, otherwise False.

#### `Task<bool> CopyMembersIntoStatusGroup(string groupId, CopyStatus status, CancellationToken cancellationToken = default)`

Copy all account members of one or more statuses into a group.

| Parameter | |
|---|---|
| `groupId` | Group identifier. |
| `status` | Class representing a list of Member statuses: ‘a’ (active), ‘o’ (optout), and/or ‘e’ (error). |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True

> Http404 if the group does not exist.

#### `Task<bool> DeleteAllMembers(MemberStatusShort memberStatusId, CancellationToken cancellationToken = default)`

Delete all members.

| Parameter | |
|---|---|
| `memberStatusId` | This is ‘a’ (active), ‘o’ (optout), or ‘e’ (error). |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Returns true.

#### `Task<bool> DeleteMember(string memberId, CancellationToken cancellationToken = default)`

Delete the specified member. The member, along with any associated response and history information, will be completely removed from the database.

| Parameter | |
|---|---|
| `memberId` | Member identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if the member is deleted.

> Http404 if no member is found.

#### `Task<bool> DeleteMembers(DeleteMembers members, CancellationToken cancellationToken = default)`

Delete an array of members. The members will be marked as deleted and cannot be retrieved.

| Parameter | |
|---|---|
| `members` | Class representing an array of member ids to delete. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if all members are successfully deleted, otherwise False.

#### `Task<IEnumerable<Import>> GetAllImports(uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get information about all imports for this account.

| Parameter | |
|---|---|
| `start` | Pagination: start page. Defaults to first page (e.g. 0). |
| `end` | Pagination: end page. Defaults to first page (e.g. 500). |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of import details.

#### `Task<int> GetAllImportsCount(CancellationToken cancellationToken = default)`

Get a count of all imports for this account.

**Returns** — An array of import details.

#### `Task<Import?> GetImportInformation(string importId, CancellationToken cancellationToken = default)`

Get information and statistics about this import.

| Parameter | |
|---|---|
| `importId` | Import identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Import details for the given import_id.

#### `Task<Member?> GetMember(string memberId, bool includeDeleted = false, CancellationToken cancellationToken = default)`

Get detailed information on a particular member, including all custom fields.

| Parameter | |
|---|---|
| `memberId` | Member identifier. |
| `includeDeleted` | Accepts True. Optional flag to include deleted members. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A single member if one exists.

> Http404 if no member is found.

#### `Task<Member?> GetMemberByEmail(string memberEmail, bool includeDeleted = false, CancellationToken cancellationToken = default)`

Get detailed information on a particular member, including all custom fields, by email address instead of ID.

| Parameter | |
|---|---|
| `memberEmail` | Member email. |
| `includeDeleted` | Accepts True. Optional flag to include deleted members. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A single member if one exists.

> Http404 if no member is found.

#### `Task<int> GetMemberCount(bool includeDeleted = false, MemberStatusShort? status = null, string? rawFilter = null, CancellationToken cancellationToken = default)`

Get a count of members in an account.

| Parameter | |
|---|---|
| `includeDeleted` | Accepts True. Optional flag to include deleted members. |
| `status` | Optional. Count only members in this status. Note this is not the same as `includeDeleted` - Emma tracks active, opt-out, error and forwarded separately from deletion, and a quota figure is normally defined on active only. |
| `rawFilter` | Optional. A raw Emma filter expression, e.g. `["member_status_id","eq","a"]`, for filters this wrapper does not model. Cannot be combined with `status`. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — The number of members matching the filter.

#### `Task<IEnumerable<Group>> GetMemberGroups(string memberId, CancellationToken cancellationToken = default)`

Get the groups to which a member belongs.

| Parameter | |
|---|---|
| `memberId` | Member identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of groups.

> Http404 if no member is found.

#### `Task<IEnumerable<MailingHistory>> GetMemberMailingHistory(string memberId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get the entire mailing history for a member.

| Parameter | |
|---|---|
| `memberId` | Member identifier. |
| `start` | Pagination: start page. Defaults to first page (e.g. 0). |
| `end` | Pagination: end page. Defaults to first page (e.g. 500). |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Message history details for the specified member.

#### `Task<int> GetMemberMailingHistoryCount(string memberId, CancellationToken cancellationToken = default)`

Get the number of mailing history entries for a member.

| Parameter | |
|---|---|
| `memberId` | Member identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Message history details for the specified member.

#### `Task<IEnumerable<MemberOptout>> GetMemberOptout(string memberId, CancellationToken cancellationToken = default)`

If a member has been opted out, returns the details of their optout, specifically date and mailing_id.

| Parameter | |
|---|---|
| `memberId` | Member identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Member opt out date and mailing if member is opted out.

> Http404 if no member is found.

#### `Task<IEnumerable<ImportMembers>> GetMembersAffectedByImport(string importId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get a list of members affected by this import.

| Parameter | |
|---|---|
| `importId` | Import identifier. |
| `start` | Pagination: start page. Defaults to first page (e.g. 0). |
| `end` | Pagination: end page. Defaults to first page (e.g. 500). |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A list of members in the given account and import.

#### `Task<int> GetMembersAffectedByImportCount(string importId, CancellationToken cancellationToken = default)`

Get a count of members affected by this import.

| Parameter | |
|---|---|
| `importId` | Import identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A list of members in the given account and import.

#### `Task<IEnumerable<Member>> ListMembers(bool includeDeleted = false, uint? start = null, uint? end = null, MemberStatusShort? status = null, MemberFieldSelection fields = 0, string? rawFilter = null, CancellationToken cancellationToken = default)`

Get a basic listing of members in an account.

| Parameter | |
|---|---|
| `includeDeleted` | Accepts True. Optional flag to include deleted members. |
| `start` | Pagination: inclusive start index. Defaults to 0. These are record indices, not page numbers - Emma's range is inclusive, so a 500-record page is start 0 to end 499. |
| `end` | Pagination: inclusive end index. Defaults to `start + 499`. |
| `status` | Optional. Return only members in this status. Distinct from `includeDeleted`: Emma tracks active, opt-out, error and forwarded separately from deletion. |
| `fields` | Whether to include custom fields. Excluding them is the single biggest throughput win available when only the email and id are needed. |
| `rawFilter` | Optional. A raw Emma filter expression, e.g. `["member_status_id","eq","a"]`, for filters this wrapper does not model. Cannot be combined with `status`. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — The members matching the filter.

#### `Task<MemberSignup?> MemberSignup(SignupMember member, CancellationToken cancellationToken = default)`

Takes the necessary actions to signup a member and enlist them in the provided group ids. You can send the same member multiple times and pass in new group ids to signup. This process triggers the opt-out workflow, and will send a mailing to the member on new group enlistments. If no new group ids are provided for an existing member, the endpoint will respond back with their status and member_id, performing no additional actions.

| Parameter | |
|---|---|
| `member` | Fields related to signing up a member. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — The member_id of the member, and their status. The status will be reported as ‘a’ (active), ‘e’ (error), or ‘o’ (optout).

#### `Task<bool> RemoveMemberFromAllGroups(string memberId, CancellationToken cancellationToken = default)`

Remove the specified member from all groups.

| Parameter | |
|---|---|
| `memberId` | Member identifier. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if the member is removed from all groups.

> Http404 if no member is found.

#### `Task<IEnumerable<long>> RemoveMemberFromGroups(string memberId, List<long> groupIds, CancellationToken cancellationToken = default)`

Remove a single member from one or more groups.

| Parameter | |
|---|---|
| `memberId` | Member identifier. |
| `groupIds` | Group ids from which to remove this member |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of references to the affected groups.

> Http404 if no member is found.

#### `Task<bool> RemoveMembersFromGroups(RemoveMemberGroups groups, CancellationToken cancellationToken = default)`

Remove multiple members from groups.

| Parameter | |
|---|---|
| `groups` | Class representing members and the groups to remove them from. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if the members are deleted, otherwise False.

> Http404 if any of the members or groups do not exist

#### `Task<bool> UpdateMemberToOptoutByEmail(string memberEmail, CancellationToken cancellationToken = default)`

Update a member’s status to optout keyed on email address instead of an ID.

| Parameter | |
|---|---|
| `memberEmail` | Member email address for optout. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if member status change was successful or member was already opted out.

> Http404 if no member is found.

#### `Task<bool> UpdateSingleMemberInformation(string memberId, UpdateMember member, CancellationToken cancellationToken = default)`

Update a single member’s information. Update the information for an existing member (even if they are marked as deleted). Note that this method allows the email address to be updated (which cannot be done with a POST, since in that case the email address is used to identify the member).

| Parameter | |
|---|---|
| `memberId` | Member identifier. |
| `member` | Class representing fields to update member information. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if the member was updated successfully

> Http404 if no member is found.

#### `Task<bool> UpdateStatusOfGroupMembersBasedOnCurrentStatus(MemberStatusShort statusFrom, MemberStatusShort statusTo, string? groupId = null, CancellationToken cancellationToken = default)`

Update the status for a group of members, based on their current status. Valid statuses id
are (‘a’,’e’, ‘f’, ‘o’) active, error, forwarded, optout.

| Parameter | |
|---|---|
| `statusFrom` | The current status of the members. |
| `statusTo` | The updated status of the members. |
| `groupId` | Optional. Limit the update to members of the specified group |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True

> Http400 if the specified status is invalid

### IEmmaResponseProvider

Provides access to response data. You can get overview numbers for all of your mailings and also drill down
into finding out the actual members who opened a particular mailing.

#### `Task<IEnumerable<ResponseClicks>> GetMailingClicks(string mailingId, string? memberId = null, string? linkId = null, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get the list of clicks for this mailing///

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `memberId` | Optional. Limits results to a single member. |
| `linkId` | Optional. Limits results to a single link. |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of link objects for the mailing.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ for
> standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<int> GetMailingClicksCount(string mailingId, CancellationToken cancellationToken = default)`

Get the count of the list of clicks for this mailing

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of link objects for the mailing.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ for
> standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<ResponseCustomerShare?> GetMailingCustomerShare(string shareId, CancellationToken cancellationToken = default)`

Get the customer share associated with the share id.

| Parameter | |
|---|---|
| `shareId` | Share Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A customer share for the mailing.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid
> mailing type - ‘m’ for standard mailings, ‘t’ for test mailings and ‘r’ for
> trigger mailings.

#### `Task<IEnumerable<ResponseCustomerShareClicks>> GetMailingCustomerShareClicks(string mailingId, CancellationToken cancellationToken = default)`

Get the list of customer share clicks for this mailing.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of customer share click objects for the mailing.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid
> mailing type - ‘m’ for standard mailings, ‘t’ for test mailings and ‘r’ for
> trigger mailings.

#### `Task<IEnumerable<ResponseCustomerShare>> GetMailingCustomerShares(string mailingId, CancellationToken cancellationToken = default)`

Get the list of customer shares for this mailing.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of customer shares objects for the mailing.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing
> type - ‘m’ for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<IEnumerable<ResponseDeliveries>> GetMailingDelieveries(string mailingId, DeliveryType result = 1, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get the list of messages that have finished delivery. This will include those that were
successfully delivered, as well as those that failed due to hard or soft bounces.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `result` | Optional. Accepted options: ‘all’, ‘delivered’, ‘bounced’, ‘hard’, ‘soft’. Defaults to ‘all’, if not provided. |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of message responses that have finished delivery.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing
> type - ‘m’ for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<int> GetMailingDelieveriesCount(string mailingId, DeliveryType result = 1, CancellationToken cancellationToken = default)`

Get the count of the list of messages that have finished delivery. This will include those that were successfully
delivered, as well as those that failed due to hard or soft bounces.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `result` | Optional. Accepted options: ‘all’, ‘delivered’, ‘bounced’, ‘hard’, ‘soft’. Defaults to ‘all’, if not provided. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of message responses that have finished delivery.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’
> for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<IEnumerable<ResponseForwards>> GetMailingForwards(string mailingId, CancellationToken cancellationToken = default)`

Get the list of forwards for this mailing

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of forwards objects for the mailing.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’
> for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<IEnumerable<ResponseGeneric>> GetMailingInProgress(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get the list of messages that are in the queue, possibly sent, but not yet delivered.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Get the list of messages that are in-progress.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’
> for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<int> GetMailingInProgressCount(string mailingId, CancellationToken cancellationToken = default)`

Get the count of the list of messages that are in the queue, possibly sent, but not yet delivered.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Get the list of messages that are in-progress.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ for
> standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<IEnumerable<Link>> GetMailingLinks(string mailingId, CancellationToken cancellationToken = default)`

Get the list of links for this mailing

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of link objects for the mailing.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ for standard
> mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<IEnumerable<ResponseGeneric>> GetMailingOpens(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get the count of the list of opened messages for this campaign

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Get the list of messages that opened.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ for standard
> mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<int> GetMailingOpensCount(string mailingId, CancellationToken cancellationToken = default)`

Get the list of opened messages for this campaign.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Get the list of messages that opened.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’
> for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<IEnumerable<ResponseGeneric>> GetMailingOptouts(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get the list of optouts for this mailing.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of optouts objects for the mailing.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’
> for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<int> GetMailingOptoutsCount(string mailingId, CancellationToken cancellationToken = default)`

Get the count of the list of optouts for this mailing.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of optouts objects for the mailing.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’
> for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<Response?> GetMailingResponse(string mailingId, CancellationToken cancellationToken = default)`

Get the response summary for a particular mailing. This method will return the counts of each type of
response activity for a particular mailing.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A single mailing object.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’
> for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<IEnumerable<ResponseGeneric>> GetMailingSends(string mailingId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get the list of messages that have been sent to an MTA (Message Transfer Agent) for delivery.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Get the list of messages that have been sent to an MTA for delivery.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’
> for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<int> GetMailingSendsCount(string mailingId, CancellationToken cancellationToken = default)`

Get the count of the list of messages that have been sent to an MTA (Message Transfer Agent) for delivery.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’
> for standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<IEnumerable<ResponseShares>> GetMailingShares(string mailingId, CancellationToken cancellationToken = default)`

Get the list of shares for this mailing.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of signups objects for the mailing.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid
> mailing type - ‘m’ for standard mailings, ‘t’ for test mailings and ‘r’ for
> trigger mailings.

#### `Task<IEnumerable<ResponseSharesOverview>> GetMailingSharesOverview(string mailingId, CancellationToken cancellationToken = default)`

Get overview of shares pertaining to this mailing_id.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of share summary objects for the mailing, by network.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid.

#### `Task<IEnumerable<ResponseSignups>> GetMailingSignups(string mailingId, CancellationToken cancellationToken = default)`

Get the list of signups for this mailing.

| Parameter | |
|---|---|
| `mailingId` | Mailing Identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of signups objects for the mailing.

> Http404 if the mailing does not exist. Http404 if the mailing is not valid mailing type - ‘m’ for
> standard mailings, ‘t’ for test mailings and ‘r’ for trigger mailings.

#### `Task<IEnumerable<ResponseSummary>> GetResponseSummary(DateRange? range = null, bool includeArchived = false, CancellationToken cancellationToken = default)`

Get the response summary for an account. This method will return a month-based time series of data including sends,
opens, clicks, mailings, forwards, and opt-outs. Test mailings and forwards are not included in the data returned.

| Parameter | |
|---|---|
| `range` | Optional DateRange object to build the range parameter. |
| `includeArchived` | Optional flag to include archived mailings in the list. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A list of objects with each object representing one month.

#### `Task<IEnumerable<ResponseSummary>> GetResponseSummary(DateTime? date, bool includeArchived = false, CancellationToken cancellationToken = default)`

Get the response summary for an account. This method will return a month-based time series of data including sends,
opens, clicks, mailings, forwards, and opt-outs. Test mailings and forwards are not included in the data returned.

| Parameter | |
|---|---|
| `date` | Optional date to build the range parameter. |
| `includeArchived` | Optional flag to include archived mailings in the list. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A list of objects with each object representing one month.

### IEmmaSearchProvider

Provides access to create, edit, and delete searches. You can also retrieve the members matching
any search created in your account.

#### `Task<int> CreateSavedSearch(CreateSearch search, CancellationToken cancellationToken = default)`

Create a saved search

| Parameter | |
|---|---|
| `search` | A name used to describe this search and a combination of search conditions, as described in the documentation. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — The ID of the new search.

> Http400 if the search is invalid.

#### `Task<bool> DeleteSavedSearch(string searchId, CancellationToken cancellationToken = default)`

Delete a saved search. The member records referred to by the search are not affected

| Parameter | |
|---|---|
| `searchId` | Search identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if the search is deleted.

> Http404 if the search does not exist.

#### `Task<IEnumerable<Member>> GetMembersMatchingSearch(string searchId, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Get the members matching the search

| Parameter | |
|---|---|
| `searchId` | Search identifier |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of members.

> Http404 if the search does not exist.

#### `Task<int> GetMembersMatchingSearchCount(string searchId, CancellationToken cancellationToken = default)`

Get a count of the number of members matching the search

| Parameter | |
|---|---|
| `searchId` | Search identifier |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of members.

#### `Task<Search?> GetSearchDetails(string searchId, bool includeDeleted = false, CancellationToken cancellationToken = default)`

Get the details for a saved search

| Parameter | |
|---|---|
| `searchId` | Search identifier |
| `includeDeleted` | >Optional flag to include deleted searches. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A search.

> Http404 if the search does not exist.

#### `Task<List<Search>> GetSearches(bool includeDeleted = false, uint? start = null, uint? end = null, CancellationToken cancellationToken = default)`

Retrieve a list of saved searches

| Parameter | |
|---|---|
| `includeDeleted` | Optional flag to include deleted searches. |
| `start` | Start paging record at. |
| `end` | End paging record at. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of searches.

#### `Task<int> GetSearchesCount(bool includeDeleted = false, CancellationToken cancellationToken = default)`

Get a count of the number of saved searches

| Parameter | |
|---|---|
| `includeDeleted` | Optional flag to include deleted searches. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — An array of searches.

#### `Task<bool> UpdateSavedSearch(string searchId, CreateSearch search, CancellationToken cancellationToken = default)`

Update a saved search. No parameters are required, but either the name or criteria parameter must be present for an update to occur.

| Parameter | |
|---|---|
| `searchId` | Search identifier |
| `search` | A name used to describe this search and/or a combination of search conditions, as described in the documentation. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if the update was successful

> Http404 if the search does not exist. Http400 if the search criteria is invalid.

### IEmmaSignupFormProvider

Provides a list of all of your signup forms

#### `Task<IEnumerable<SignupForm>> GetSignupForms(CancellationToken cancellationToken = default)`

Gets a list of this account’s signup forms

**Returns** — An array of signup forms.

### IEmmaSubscriptionProvider

Provides access to subscriptions and subscription members

#### `Task<Subscription?> DeleteSubscription(string subscription_id, CancellationToken cancellationToken = default)`

Delete a subscription

| Parameter | |
|---|---|
| `subscription_id` |  |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Information about the subscription, including the date and time it was deleted.

#### `Task<Subscription?> EditSubscription(SubscriptionNew subscription, string subscription_id, CancellationToken cancellationToken = default)`

Edit a subscription's name or description

| Parameter | |
|---|---|
| `subscription` | Name and description of the subscription text to update. Visible in the Subscription Center. |
| `subscription_id` |  |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Information about the updated subscription.Limited to name and description.

#### `Task<Subscription?> GetAccountSubscription(string subscription_id, CancellationToken cancellationToken = default)`

Get detailed information for a specific subscription

| Parameter | |
|---|---|
| `subscription_id` | URL segment for the subscription ID to query details on |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Information about a subscription.

#### `Task<IEnumerable<Subscription>> GetAccountSubscriptions(bool includeDeletedOnly = false, bool includeDeleted = false, CancellationToken cancellationToken = default)`

Get a list of all subscriptions in an account

| Parameter | |
|---|---|
| `includeDeletedOnly` | true or false. Returns deleted subscriptions only. Optional, defaults to false. |
| `includeDeleted` | true or false. Returns deleted subscriptions along with active. Optional, defaults to false. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A list of subscriptions in an account along with related information, including member count and subscription ID.

#### `Task<IEnumerable<SubscriptionMembers>> GetOptOutSubscriptionMembers(string subscription_id, uint start = 0, uint end = 500, CancellationToken cancellationToken = default)`

Get a list of member IDs for members who have opted out of a specific subscription

| Parameter | |
|---|---|
| `subscription_id` | URL segment for the subscription ID to query details on |
| `start` | Pagination: start page. Defaults to first page (e.g. 0). |
| `end` | Pagination: end page. Defaults to first page (e.g. 500). |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A list of member IDs.

#### `Task<IEnumerable<SubscriptionMembers>> GetSubscriptionMembers(string subscription_id, uint start = 0, uint end = 500, CancellationToken cancellationToken = default)`

Get a list of member IDs for members subscribed to a specific subscription

| Parameter | |
|---|---|
| `subscription_id` | URL segment for the subscription ID to query details on |
| `start` | Pagination: start page. Defaults to first page (e.g. 0). |
| `end` | Pagination: end page. Defaults to first page (e.g. 500). |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — A list of member IDs.

#### `Task<bool> PostBulkImportSubscriptions(SubscriptionImportBulk importId, string subscription_id, CancellationToken cancellationToken = default)`

Bulk subscribe members to a subscription using the import ID of all members

| Parameter | |
|---|---|
| `importId` | import ID to bulk subscribe |
| `subscription_id` | subscription id |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if successful.

#### `Task<bool> PostBulkMemberSubscriptions(SubscriptionBulk memberIds, string subscription_id, CancellationToken cancellationToken = default)`

Bulk subscribe members to a subscription using a list of member IDs

| Parameter | |
|---|---|
| `memberIds` | List of memberIDs |
| `subscription_id` | subscription id |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if successful.

#### `Task<Subscription?> PostNewSubscription(SubscriptionNew subscription, CancellationToken cancellationToken = default)`

Create a subscription

| Parameter | |
|---|---|
| `subscription` | Name and description of the new subscription to create |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Information about the created subscription, including the subscription ID.

### IEmmaWebhookProvider

Provides access to webhooks

#### `Task<int> CreateWebhook(CreateWebhook webhook, CancellationToken cancellationToken = default)`

Create an new webhook

| Parameter | |
|---|---|
| `webhook` | The webhook to be created. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — The ID of the newly created webhook.

#### `Task<bool> DeleteAllWebhooks(CancellationToken cancellationToken = default)`

Delete all webhooks registered for an account

**Returns** — True if the webhook deleted successfully.

#### `Task<bool> DeleteWebhookById(string webhookId, CancellationToken cancellationToken = default)`

Deletes an existing webhook

| Parameter | |
|---|---|
| `webhookId` | The ID of the Webhook to delete. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — True if the webhook deleted successfully.

#### `Task<Webhook?> GetWebhookById(string webhookId, CancellationToken cancellationToken = default)`

Get information for a specific webhook belonging to a specific account

| Parameter | |
|---|---|
| `webhookId` | The ID of the Webhook to return. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — Details for a single webhook

#### `Task<IEnumerable<WebhookEvents>> GetWebhookEvents(CancellationToken cancellationToken = default)`

Get a listing of all event types that are available for webhooks

**Returns** — A list of event types and descriptions

#### `Task<IEnumerable<Webhook>> GetWebhooks(CancellationToken cancellationToken = default)`

Get a basic listing of all webhooks associated with an account

**Returns** — A list of webhooks that belong to the given account.

#### `Task<int> UpdateWebhook(string webhookId, UpdateWebhook webhook, CancellationToken cancellationToken = default)`

Update an existing webhook

| Parameter | |
|---|---|
| `webhookId` | The ID of the Webhook to update. |
| `webhook` | The webhook parameters to be updated. |
| `cancellationToken` | Cancels the in-flight request. |

**Returns** — The id of the updated webhook, or False if the update failed.

## Configuration and helpers

### EmmaRetryDefaults

Classifies Emma responses as retryable, so consumers do not each have to rediscover how
Emma signals throttling.

> This library deliberately does **not** own a retry policy. Retry belongs to the
> consumer's resilience pipeline, which knows the surrounding budget. What the wrapper can
> usefully own is the classification — and Emma's is genuinely surprising.
> 
> These are plain predicates over `HttpResponseMessage` and
> `Exception` rather than Polly types, so the library keeps working on
> netstandard2.0. `Microsoft.Extensions.Http.Resilience` requires net8.0 or later; if
> the library took a dependency on it, .NET Framework consumers would be shut out.
> Wiring it into the standard resilience handler:
> 
> 
> ```csharp
> services.AddEmmaApiProviders(configuration)
>         .AddStandardResilienceHandler(options =>
>         {
>             options.Retry.ShouldHandle = args =>
>                 ValueTask.FromResult(EmmaRetryDefaults.IsTransient(args.Outcome.Result)
>                                   || EmmaRetryDefaults.IsTransient(args.Outcome.Exception));
> 
>             // The default 10s per attempt is not enough for a 500-record member page.
>             options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
>             options.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(2);
>         });
> ```

#### `TimeSpan? GetRetryAfter(HttpResponseMessage? response)`

Reads Emma's `Retry-After` hint, whether expressed as a delay or a date.

**Returns** — How long to wait, or `` if Emma did not say.

#### `bool IsTransient(HttpStatusCode statusCode, bool treatForbiddenAsThrottle = true)`

Whether a status code should be retried.

| Parameter | |
|---|---|
| `statusCode` | The status Emma returned. |
| `treatForbiddenAsThrottle` | Whether `403` counts as throttling. Defaults to ``, because Emma uses 403 for rate limiting as well as the conventional 429 — the single most surprising behaviour in the API, and the one every consumer gets wrong first.  The trade-off is real: a genuine credentials failure also returns 403, and cannot be told apart from a throttle without inspecting the response body, which a resilience handler is not well placed to do. Retrying a bad-credentials 403 wastes the attempt budget and then fails, which is the less damaging error of the two. Pass `` if your credentials are dynamic and you would rather fail fast. |

#### `bool IsTransient(HttpResponseMessage? response, bool treatForbiddenAsThrottle = true)`

Whether a response should be retried.

| Parameter | |
|---|---|
| `response` | The response, which may be ``. |
| `treatForbiddenAsThrottle` | See the overload taking a status code. |

#### `bool IsTransient(Exception? exception)`

Whether a failure should be retried.

| Parameter | |
|---|---|
| `exception` | The exception, which may be ``. |

> Covers `EmmaRateLimitException`, transient `EmmaException`
> statuses, transport faults, and per-attempt timeouts.

### EmmaSharperExtensions

Extension methods for registering `N:EmmaSharper` with the DI container.

#### `IHttpClientBuilder AddEmmaApiProviders(IServiceCollection services, IConfiguration configuration, string? sectionName = "Emma")`

Adds the Emma API providers, binding options from configuration.

| Parameter | |
|---|---|
| `services` | The service collection. |
| `configuration` | Configuration to bind from. |
| `sectionName` | Section to bind. Defaults to `DefaultSectionName`. Pass `null` to bind the configuration root instead, which expects AccountId, PublicKey and SecretKey at the top level of appsettings.json. |

**Returns** — The `IHttpClientBuilder` for the Emma client, so callers can attach a
resilience handler - see `EmmaRetryDefaults.ShouldHandle`.

#### `IHttpClientBuilder AddEmmaApiProviders(IServiceCollection services, Action<EmmaOptions> configure)`

Adds the Emma API providers with options configured in code.

| Parameter | |
|---|---|
| `services` | The service collection. |
| `configure` | Configures `EmmaOptions`. |

**Returns** — The `IHttpClientBuilder` for the Emma client.

### EmmaOptions

Emma configuration options.

| Property | Type | |
|---|---|---|
| `AccountId` | `string` | Default Emma account identifier. |
| `BaseUrl` | `string` | Represents the default Emma API endpoint. |
| `PublicKey` | `string` | Emma public key. |
| `SecretKey` | `string` | Emma private key. |
| `Timeout` | `TimeSpan` | Per-request timeout. |

## Exceptions

### EmmaException

Thrown when the Emma API returns a non-success status code.

> Carries the status code, response body, verb and resource as typed properties, so callers
> can branch on them directly. `Message` is for humans and its wording
> is not a contract - do not match on it.

| Property | Type | |
|---|---|---|
| `Method` | `HttpMethod` | The HTTP verb of the failing request. |
| `Resource` | `string` | The resolved request path of the failing request. |
| `ResponseBody` | `string` | The raw response body, which usually carries Emma's own error text. |
| `StatusCode` | `HttpStatusCode` | The HTTP status code Emma returned. |

### EmmaRateLimitException

Thrown when Emma throttles a request.

> Emma signals throttling with **403 Forbidden** as well as the conventional 429. That is
> the single most surprising behaviour in the API and the easiest thing for a consumer to get
> wrong - a naive client treats the 403 as an auth failure and gives up instead of backing off.
> Classifying it here means every consumer gets it right by default.
> 
> Callers that want automatic retries should attach a resilience handler rather than catching
> this - see `EmmaRetryDefaults.ShouldHandle`.

| Property | Type | |
|---|---|---|
| `RetryAfter` | `TimeSpan?` | How long Emma asked the caller to wait, when it said so. |

## Enums

### DeliveryType

| Member | Description |
|---|---|
| `Unknown` |  |
| `All` |  |
| `Delivered` |  |
| `Bounced` |  |
| `Hard` |  |
| `Soft` |  |

### DeliveryTypeShort

| Member | Description |
|---|---|
| `Unknown` |  |
| `Delivered` |  |
| `Hard` |  |
| `Soft` |  |

### FieldType

| Member | Description |
|---|---|
| `Unknown` |  |
| `Text` |  |
| `TextArray` |  |
| `Numeric` |  |
| `Boolean` |  |
| `Date` |  |
| `Timestamp` |  |

### GroupType

| Member | Description |
|---|---|
| `Unknown` |  |
| `Group` |  |
| `Test` |  |
| `Hidden` |  |
| `All` |  |

### ImportChangeType

| Member | Description |
|---|---|
| `Unknown` |  |
| `Added` |  |
| `Confirmed` |  |
| `Deleted` |  |
| `Undeleted` |  |
| `Updated` |  |
| `UpdateRejected` |  |
| `SignedUp` |  |
| `StatusShifted` |  |

### ImportStatus

| Member | Description |
|---|---|
| `Unknown` |  |
| `Okay` |  |
| `Error` |  |
| `Queued` |  |

### MailingStatus

| Member | Description |
|---|---|
| `Unknown` |  |
| `Pending` |  |
| `Paused` |  |
| `Sending` |  |
| `Canceled` |  |
| `Complete` |  |
| `Failed` |  |

### MailingType

| Member | Description |
|---|---|
| `Unknown` |  |
| `Standard` |  |
| `Test` |  |
| `Trigger` |  |
| `Split` |  |
| `ContentSplit` |  |

### MemberFieldSelection

How much of each member record to ask Emma for.

> The single biggest throughput lever on a large account. Pulling every custom field for
> every member is the difference between a fast sync and a hostile one when the account
> holds hundreds of thousands of members and only the email and id are wanted.

| Member | Description |
|---|---|
| `All` | Return the full member record, including custom fields. |
| `ExcludeCustomFields` | Omit custom fields. Maps to Emma's `exclude_fields=1`. |

### MemberStatus

| Member | Description |
|---|---|
| `Unknown` |  |
| `Active` |  |
| `Optout` |  |
| `Error` |  |
| `Forwarded` |  |

### MemberStatusShort

| Member | Description |
|---|---|
| `Unknown` |  |
| `Active` |  |
| `Optout` |  |
| `Error` |  |
| `Forwarded` |  |

### PersonalizationType

| Member | Description |
|---|---|
| `Unknown` |  |
| `All` |  |
| `Html` |  |
| `PlainText` |  |
| `Subject` |  |

### SubaccountStatusFilter

Which subaccount lifecycle states to include when listing.

> Defaults to `All`. Narrowing the filter changes which subaccounts you
> enumerate and therefore any total computed across them, so choose deliberately rather than
> assuming `Active` is what you want.

| Member | Description |
|---|---|
| `Active` | Active subaccounts. |
| `Trial` | Subaccounts still in trial. |
| `PendingRetirement` | Subaccounts scheduled for retirement. |
| `Retired` | Retired subaccounts. These may still contain member records. |
| `All` | Every lifecycle state. |

### UpdateMailingStatus

| Member | Description |
|---|---|
| `Unknown` |  |
| `Canceled` |  |
| `Paused` |  |
| `Ready` |  |

### WebhookMethod

Webhook method enumeration

| Member | Description |
|---|---|
| `Unknown` |  |
| `Get` | Webhook uses HTTP GET |
| `Post` | Webhook uses HTTP POST |

### WidgetType

| Member | Description |
|---|---|
| `Unknown` |  |
| `Text` |  |
| `LongInt` |  |
| `Checkbox` |  |
| `SelectMultiple` |  |
| `CheckMultiple` |  |
| `Radio` |  |
| `Date` |  |
| `SelectOne` |  |
| `Number` |  |

### WorkflowStatus

| Member | Description |
|---|---|
| `Unknown` |  |
| `Active` |  |
| `Inactive` |  |
| `Draft` |  |

## Models

Data types returned by, or sent to, the providers above.

### AccountUser

A user with access to an Emma account.

| Property | Type | |
|---|---|---|
| `AdditionalData` | `Dictionary<string, JsonElement>` | Any additional fields Emma returns for an account user. |
| `CreatedAt` | `DateTime?` | When the user was created. |
| `Email` | `string` | The user's email address. |
| `FirstName` | `string` | Given name. |
| `LastLoginAttempt` | `DateTime?` | The user's most recent sign-in attempt. |
| `LastName` | `string` | Family name. |
| `Role` | `string` | The user's role on this account. |
| `UserId` | `long?` | The user's Emma id. |

### AddMember

Parameters to add a single member to an audience. Group Ids and Field Triggers are optional

| Property | Type | |
|---|---|---|
| `FieldTriggers` | `bool?` | Optional. Fires related field change auto-responders when set to true. |
| `Fields` | `Dictionary<string, object>` | Names and values of user-defined fields to update |
| `GroupIds` | `List<long>` | Optional. Add imported members to this list of groups. |
| `MemberEmail` | `string` | Email address of member to add or update |

### AddMembers

Parameters to add a batch members to an audience

| Property | Type | |
|---|---|---|
| `AddOnly` | `bool?` | Optional. Only add new members, ignore existing members. |
| `AutomateFieldChanges` | `bool?` | Optional. Fires related field change auto-responders when set to true. |
| `GroupIds` | `List<long>` | Optional. Add imported members to this list of groups. |
| `Members` | `List<MemberBulk>` | Email address of member to add or update |
| `SourceFileName` | `string` | Names and values of user-defined fields to update |

### BaseField

| Property | Type | |
|---|---|---|
| `ColumnOrder` | `int?` |  |
| `DisplayName` | `string` |  |
| `FieldType` | `FieldType` |  |
| `WidgetType` | `WidgetType` |  |

### ChangeStatus

Change the status for an array of members

| Property | Type | |
|---|---|---|
| `MemberIds` | `List<long>` | The array of member ids to change. |
| `StatusTo` | `MemberStatusShort` | The new status for the given members. Accepts one of ‘a’ (active), ‘e’ (error), ‘o’ (optout). |

### CopyStatus

Copy all account members of one or more statuses into a group

| Property | Type | |
|---|---|---|
| `MemberStatusId` | `List<MemberStatusShort>` | ‘a’ (active), ‘o’ (optout), and/or ‘e’ (error) |

### CreateField

| Property | Type | |
|---|---|---|
| `ColumnOrder` | `int?` |  |
| `DisplayName` | `string` |  |
| `FieldType` | `FieldType` |  |
| `ShortcutName` | `string` |  |
| `WidgetType` | `WidgetType` |  |

### CreateSearch

| Property | Type | |
|---|---|---|
| `Criteria` | `string` |  |
| `Name` | `string` |  |

### CreateWebhook

Properties associated with creating webhooks

| Property | Type | |
|---|---|---|
| `Event` | `string` |  |
| `Method` | `WebhookMethod` |  |
| `PublicKey` | `string` | The public_key to use for authentication. Note: this can also be spelled “user_id” but this is deprecated. |
| `Url` | `string` |  |

### Date

### Dates

| Property | Type | |
|---|---|---|
| `Key` | `Date` |  |
| `Value` | `int` |  |

### DeleteMembers

| Property | Type | |
|---|---|---|
| `MemberIds` | `List<long>` | An array of member ids to delete. |

### Email

| Property | Type | |
|---|---|---|
| `Value` | `string` |  |

### Field

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `ColumnOrder` | `int?` |  |
| `DeletedAt` | `DateTime?` |  |
| `DisplayName` | `string` |  |
| `FieldId` | `long?` |  |
| `FieldType` | `FieldType` |  |
| `Options` | `string[]` |  |
| `Required` | `bool` |  |
| `ShortDisplayName` | `string` |  |
| `ShortcutName` | `string` |  |
| `WidgetType` | `WidgetType` |  |

### ForwardMailing

Forward a previous message to additional recipients. If these recipients are not already in the audience, they will be added with a status of FORWARDED.

| Property | Type | |
|---|---|---|
| `Note` | `string` | A note to include in the forward. This note will be HTML encoded and is limited to 500 characters. |
| `RecipientEmails` | `List<string>` | An array of email addresses to which to forward the specified message. |

### Group

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `ActiveCount` | `int?` |  |
| `DeletedAt` | `DateTime?` |  |
| `ErrorCount` | `int?` |  |
| `GroupName` | `string` |  |
| `GroupType` | `GroupType` |  |
| `MemberGroupId` | `long?` |  |
| `OptoutCount` | `int?` |  |
| `PurgedAt` | `DateTime?` |  |

### GroupName

| Property | Type | |
|---|---|---|
| `Name` | `string` |  |

### Import

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `ErrorMessage` | `string` |  |
| `FieldsUpdated` | `List<Field>` |  |
| `GroupsUpdated` | `List<Group>` |  |
| `ImportFinished` | `DateTime?` |  |
| `ImportId` | `long?` |  |
| `ImportStarted` | `DateTime?` |  |
| `NumDuplicates` | `int?` |  |
| `NumMembersAdded` | `int?` |  |
| `NumMembersUpdated` | `int?` |  |
| `NumSkipped` | `int?` |  |
| `SourceFilename` | `string` |  |
| `Status` | `ImportStatus?` |  |
| `Style` | `string` |  |

### ImportMembers

| Property | Type | |
|---|---|---|
| `ChangeType` | `ImportChangeType` |  |
| `Email` | `string` |  |
| `MemberId` | `long?` |  |
| `MemberStatusId` | `MemberStatusShort` |  |

### Interval

### Intervals

| Property | Type | |
|---|---|---|
| `Key` | `Interval` |  |
| `Value` | `int` |  |

### Link

| Property | Type | |
|---|---|---|
| `LinkId` | `long?` |  |
| `LinkName` | `string` |  |
| `LinkOrder` | `int?` |  |
| `LinkTarget` | `string` |  |
| `Plaintext` | `bool` |  |
| `TotalClicks` | `int?` |  |
| `UniqueClicks` | `int?` |  |

### Mailing

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `ArchivedTimestamp` | `DateTime?` |  |
| `HeadsUpEmails` | `List<Email>` |  |
| `HtmlBody` | `string` |  |
| `Links` | `List<Link>` |  |
| `MailingId` | `long?` |  |
| `MailingStatus` | `MailingStatus` |  |
| `MailingType` | `MailingType` |  |
| `Name` | `string` |  |
| `ParentMailingId` | `long?` |  |
| `Plaintext` | `string` |  |
| `PublicWebviewUrl` | `string` |  |
| `RecipientCount` | `int?` |  |
| `RecipientGroups` | `List<MailingGroup>` |  |
| `RecipientMembers` | `List<Member>` |  |
| `RecipientSearches` | `List<Search>` |  |
| `SendAt` | `DateTime?` |  |
| `SendFinished` | `DateTime?` |  |
| `SendStarted` | `DateTime?` |  |
| `Sender` | `string` |  |
| `SignupFormId` | `long?` |  |
| `Subject` | `string` |  |

### MailingBase

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `ArchivedTimestamp` | `DateTime?` |  |
| `MailingId` | `long?` |  |
| `MailingStatus` | `MailingStatus` |  |
| `MailingType` | `MailingType` |  |
| `Name` | `string` |  |
| `ParentMailingId` | `long?` |  |
| `RecipientCount` | `int?` |  |
| `SendAt` | `DateTime?` |  |
| `SendFinished` | `DateTime?` |  |
| `SendStarted` | `DateTime?` |  |
| `Sender` | `string` |  |
| `SignupFormId` | `long?` |  |
| `Subject` | `string` |  |

### MailingDetails

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `ArchivedTimestamp` | `DateTime?` |  |
| `CancelByUserId` | `long?` |  |
| `CancelTimestamp` | `DateTime?` |  |
| `CreatedTimestamp` | `DateTime?` |  |
| `Datacenter` | `string` |  |
| `Disabled` | `bool` |  |
| `FailureMessage` | `string` |  |
| `FailureTimestamp` | `DateTime?` |  |
| `MailingId` | `long?` |  |
| `MailingStatus` | `MailingStatus` |  |
| `MailingType` | `MailingType` |  |
| `Month` | `int?` |  |
| `Name` | `string` |  |
| `ParentMailingId` | `long?` |  |
| `PlaintextOnly` | `bool` |  |
| `PurgedAt` | `DateTime?` |  |
| `RecipientCount` | `int?` |  |
| `ReplyTo` | `string` |  |
| `SendAt` | `DateTime?` |  |
| `SendFinished` | `DateTime?` |  |
| `SendStarted` | `DateTime?` |  |
| `Sender` | `string` |  |
| `SignupFormId` | `long?` |  |
| `StartedOrFinished` | `DateTime?` |  |
| `Subject` | `string` |  |
| `Year` | `int?` |  |

### MailingGroup

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `ActiveCount` | `int?` |  |
| `DeletedAt` | `DateTime?` |  |
| `ErrorCount` | `int?` |  |
| `GroupName` | `string` |  |
| `GroupType` | `GroupType` |  |
| `MemberGroupId` | `long?` |  |
| `OptoutCount` | `int?` |  |
| `PurgedAt` | `DateTime?` |  |

### MailingHeadsUp

Class representing the return values on a heads up information of a mailing.

| Property | Type | |
|---|---|---|
| `Email` | `string` | Email address the heads up email was sent |
| `MailingId` | `long` | Mailing associated with these heads up mailings. |
| `SentTimestamp` | `DateTime?` | Timestamp of when the heads up mailing was sent. |

### MailingHistory

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `Clicked` | `DateTime?` |  |
| `DelieveryTimestamp` | `DateTime?` |  |
| `DelieveryType` | `DeliveryTypeShort` |  |
| `Forwarded` | `DateTime?` |  |
| `MailingId` | `long?` |  |
| `MailingType` | `MailingType` |  |
| `Name` | `string` |  |
| `Opened` | `DateTime?` |  |
| `ParentMailingId` | `long?` |  |
| `Shared` | `DateTime?` |  |
| `Subject` | `string` |  |

### MailingIdentifier

Class including just the Mailing Identifier.

| Property | Type | |
|---|---|---|
| `MailingId` | `long` | Mailing Identifier. |

### MailingInfo

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `ArchivedTimestamp` | `DateTime?` |  |
| `CancelByUserId` | `long?` |  |
| `CancelTimestamp` | `DateTime?` |  |
| `CreatedTimestamp` | `DateTime?` |  |
| `Datacenter` | `string` |  |
| `DeletedAt` | `DateTime?` |  |
| `Disabled` | `bool` |  |
| `FailureMessage` | `string` |  |
| `FailureTimestamp` | `DateTime?` |  |
| `MailingId` | `long?` |  |
| `MailingStatus` | `MailingStatus` |  |
| `MailingType` | `MailingType` |  |
| `Month` | `int?` |  |
| `Name` | `string` |  |
| `ParentMailingId` | `long?` |  |
| `PlaintextOnly` | `bool` |  |
| `PurgedAt` | `DateTime?` |  |
| `RecipientCount` | `int?` |  |
| `ReplyTo` | `string` |  |
| `SendAt` | `DateTime?` |  |
| `SendFinished` | `DateTime?` |  |
| `SendStarted` | `DateTime?` |  |
| `Sender` | `string` |  |
| `SignupFormId` | `long?` |  |
| `StartedOrFinished` | `DateTime?` |  |
| `Subject` | `string` |  |
| `Year` | `int?` |  |

### MailingPersonalization

Validate that a mailing has valid personalization-tag syntax.

| Property | Type | |
|---|---|---|
| `HtmlBody` | `string` | The html contents of the mailing. |
| `Plaintext` | `string` | The plaintext contents of the mailing. Unlike in create_mailing, this param is not required. |
| `Subject` | `string` | The subject of the mailing. |

### MailingTrigger

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `ArchivedTimestamp` | `DateTime?` |  |
| `CancelByUserId` | `long?` |  |
| `CancelTimestamp` | `DateTime?` |  |
| `CreatedTimestamp` | `DateTime?` |  |
| `Datacenter` | `string` |  |
| `DeletedAt` | `DateTime?` |  |
| `Disabled` | `bool` |  |
| `FailureMessage` | `string` |  |
| `FailureTimestamp` | `DateTime?` |  |
| `HtmlBody` | `string` |  |
| `MailingId` | `long?` |  |
| `MailingStatus` | `MailingStatus` |  |
| `MailingType` | `MailingType` |  |
| `Month` | `int?` |  |
| `Name` | `string` |  |
| `ParentMailingId` | `long?` |  |
| `Plaintext` | `string` |  |
| `PlaintextOnly` | `bool` |  |
| `PurgedAt` | `DateTime?` |  |
| `RecipientCount` | `int?` |  |
| `ReplyTo` | `string` |  |
| `SendAt` | `DateTime?` |  |
| `SendFinished` | `DateTime?` |  |
| `SendStarted` | `DateTime?` |  |
| `Sender` | `string` |  |
| `SignupFormId` | `long?` |  |
| `StartedOrFinished` | `DateTime?` |  |
| `Subject` | `string` |  |
| `Year` | `int?` |  |

### Member

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `BounceCount` | `int?` |  |
| `ConfirmedOptIn` | `DateTime?` |  |
| `DeletedAt` | `DateTime?` |  |
| `Email` | `string` |  |
| `EmailError` | `string` |  |
| `Fields` | `Dictionary<string, object>` |  |
| `LastModifiedAt` | `DateTime?` |  |
| `MemberId` | `long?` |  |
| `MemberSince` | `DateTime` |  |
| `MemberStatusId` | `MemberStatusShort` |  |
| `PlaintextPreferred` | `bool` |  |
| `Status` | `MemberStatus` |  |

### MemberAdd

| Property | Type | |
|---|---|---|
| `Added` | `bool` |  |
| `MemberId` | `long?` |  |
| `Status` | `MemberStatusShort` |  |

### MemberBulk

Used to add new members or update existing members in bulk.

| Property | Type | |
|---|---|---|
| `Fields` | `Dictionary<string, object>` | Names and values of user-defined fields to update |
| `MemberEmail` | `string` | Email address of member to add or update |

### MemberIdList

| Property | Type | |
|---|---|---|
| `MemberIds` | `List<long>` |  |

### MemberMailings

### MemberOptout

| Property | Type | |
|---|---|---|
| `MailingId` | `long?` |  |
| `Timestamp` | `DateTime` |  |

### MemberSignup

The class representing the returned properties when signing up a member.

| Property | Type | |
|---|---|---|
| `MemberId` | `long?` | The member id of the member. |
| `Status` | `MemberStatusShort` | The status of the member. The short status code will be returned as Active, Error, or Optout. |

### MemberStatusShortList

| Property | Type | |
|---|---|---|
| `MemberStatusId` | `List<MemberStatusShort>` |  |

### MembersAdd

| Property | Type | |
|---|---|---|
| `ImportId` | `long` |  |

### RemoveMemberGroups

Remove multiple members from groups.

| Property | Type | |
|---|---|---|
| `GroupIds` | `List<long>` | Group ids from which to remove the given members. |
| `MemberIds` | `List<long>` | Member ids to remove from the given groups. |

### ResendMailing

Send a prior mailing to additional recipients. A new mailing will be created that inherits its content from the original.

| Property | Type | |
|---|---|---|
| `HeadsUpEmails` | `List<string>` | A list of email addresses that heads up notification emails will be sent to. |
| `RecipientEmails` | `List<string>` | An array of email addresses to which the new mailing should be sent. |
| `RecipientGroups` | `List<string>` | An array of member groups to which the new mailing should be sent. |
| `RecipientSearches` | `List<string>` | A list of searches that this mailing should be sent to. |
| `Sender` | `string` | The message sender. If this is not supplied, the sender of the original mailing will be used. |

### Response

| Property | Type | |
|---|---|---|
| `Bounced` | `int?` |  |
| `Clicked` | `int?` |  |
| `ClickedUnique` | `int?` |  |
| `CountPurchased` | `int?` |  |
| `Delivered` | `int?` |  |
| `Forwarded` | `int?` |  |
| `InProgress` | `int?` |  |
| `Name` | `string` |  |
| `Opened` | `int?` |  |
| `OptedOut` | `int?` |  |
| `RecipientCount` | `int?` |  |
| `Sent` | `int?` |  |
| `ShareClicked` | `int?` |  |
| `Shared` | `int?` |  |
| `SignedUp` | `int?` |  |
| `Subject` | `string` |  |
| `SumPurchased` | `Decimal?` |  |
| `WebviewShareClicked` | `int?` |  |
| `WebviewShared` | `int?` |  |

### ResponseBase

| Property | Type | |
|---|---|---|
| `Bounced` | `int?` |  |
| `Clicked` | `int?` |  |
| `ClickedUnique` | `int?` |  |
| `CountPurchased` | `int?` |  |
| `Delivered` | `int?` |  |
| `Forwarded` | `int?` |  |
| `Opened` | `int?` |  |
| `OptedOut` | `int?` |  |
| `Sent` | `int?` |  |
| `ShareClicked` | `int?` |  |
| `Shared` | `int?` |  |
| `SignedUp` | `int?` |  |
| `SumPurchased` | `Decimal?` |  |
| `WebviewShareClicked` | `int?` |  |
| `WebviewShared` | `int?` |  |

### ResponseClicks

| Property | Type | |
|---|---|---|
| `Email` | `string` |  |
| `EmailDomain` | `string` |  |
| `EmailUser` | `string` |  |
| `Fields` | `Dictionary<string, object>` |  |
| `LinkId` | `long?` |  |
| `MemberId` | `long?` |  |
| `MemberSince` | `DateTime?` |  |
| `MemberStatusId` | `MemberStatusShort` |  |
| `Timestamp` | `DateTime?` |  |

### ResponseCustomerShare

### ResponseCustomerShareClicks

### ResponseDeliveries

| Property | Type | |
|---|---|---|
| `DeliveryType` | `DeliveryType` |  |
| `Email` | `string` |  |
| `EmailDomain` | `string` |  |
| `EmailUser` | `string` |  |
| `Fields` | `Dictionary<string, object>` |  |
| `MailingId` | `long?` |  |
| `MailingName` | `string` |  |
| `MemberId` | `long?` |  |
| `MemberSince` | `DateTime?` |  |
| `MemberStatusId` | `MemberStatusShort` |  |
| `Timestamp` | `DateTime?` |  |

### ResponseForwards

| Property | Type | |
|---|---|---|
| `Email` | `string` |  |
| `EmailDomain` | `string` |  |
| `EmailUser` | `string` |  |
| `Fields` | `Dictionary<string, object>` |  |
| `ForwardMailingId` | `long?` |  |
| `MemberId` | `long?` |  |
| `MemberSince` | `DateTime?` |  |
| `MemberStatusId` | `MemberStatusShort` |  |
| `Timestamp` | `DateTime?` |  |

### ResponseGeneric

| Property | Type | |
|---|---|---|
| `Email` | `string` |  |
| `EmailDomain` | `string` |  |
| `EmailUser` | `string` |  |
| `Fields` | `Dictionary<string, object>` |  |
| `MemberId` | `long?` |  |
| `MemberSince` | `DateTime?` |  |
| `MemberStatusId` | `MemberStatusShort` |  |
| `Timestamp` | `DateTime?` |  |

### ResponseShares

| Property | Type | |
|---|---|---|
| `Email` | `string` |  |
| `EmailDomain` | `string` |  |
| `EmailUser` | `string` |  |
| `Fields` | `Dictionary<string, object>` |  |
| `MemberId` | `long?` |  |
| `MemberSince` | `DateTime?` |  |
| `MemberStatusId` | `MemberStatusShort` |  |
| `Network` | `string` |  |
| `ShareClicks` | `int?` |  |
| `Timestamp` | `DateTime?` |  |

### ResponseSharesBase

| Property | Type | |
|---|---|---|
| `Network` | `string` |  |
| `ShareClicks` | `int?` |  |

### ResponseSharesOverview

| Property | Type | |
|---|---|---|
| `Network` | `string` |  |
| `ShareClicks` | `int?` |  |
| `ShareCount` | `int?` |  |

### ResponseSignups

| Property | Type | |
|---|---|---|
| `Email` | `string` |  |
| `EmailDomain` | `string` |  |
| `EmailUser` | `string` |  |
| `Fields` | `Dictionary<string, object>` |  |
| `MailingMailingId` | `long?` |  |
| `MemberId` | `long?` |  |
| `MemberSince` | `DateTime?` |  |
| `MemberStatusId` | `MemberStatusShort` |  |
| `ReferingMemberId` | `long?` |  |
| `Timestamp` | `DateTime?` |  |

### ResponseSummary

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `Bounced` | `int?` |  |
| `Clicked` | `int?` |  |
| `ClickedUnique` | `int?` |  |
| `CountPurchased` | `int?` |  |
| `Delivered` | `int?` |  |
| `Forwarded` | `int?` |  |
| `Mailings` | `int?` |  |
| `Month` | `int?` |  |
| `Opened` | `int?` |  |
| `OptedOut` | `int?` |  |
| `Sent` | `int?` |  |
| `ShareClicked` | `int?` |  |
| `Shared` | `int?` |  |
| `SignedUp` | `int?` |  |
| `SumPurchased` | `Decimal?` |  |
| `WebviewShareClicked` | `int?` |  |
| `WebviewShared` | `int?` |  |
| `Year` | `int?` |  |

### Search

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `ActiveCount` | `int?` |  |
| `Criteria` | `string` |  |
| `DeletedAt` | `DateTime?` |  |
| `ErrorCount` | `int?` |  |
| `LastRunAt` | `DateTime?` |  |
| `Name` | `string` |  |
| `OptoutCount` | `int?` |  |
| `PurgedAt` | `DateTime?` |  |
| `SearchId` | `long?` |  |

### SignupForm

| Property | Type | |
|---|---|---|
| `Name` | `string` |  |
| `SignupFormId` | `long?` |  |

### SignupMember

| Property | Type | |
|---|---|---|
| `FieldTriggers` | `bool` | Optional. Fires related field change autoresponders when set to true. |
| `Fields` | `Dictionary<string, object>` | Optional. Names and values of user-defined fields to update. |
| `GroupIds` | `List<long>` | An array of group ids to associate sign-up with. |
| `MemberEmail` | `string` | Email address of the member to sign-up. |
| `OptInConfirmation` | `bool` | Optional. Sends the default plaintext confirmation email when set to true. NOTE: Confirmation email will be sent by default if this parameter is left out. |
| `OptInMessage` | `string` | Optional. Override the confirmation message body with your own copy. Must include the following tags: [rsvp_name], [rsvp_email], [opt_in_url], [opt_out_url]. |
| `OptInSubject` | `string` | Optional. Override the confirmation message subject with your own copy. |
| `SignupFormId` | `long?` | Optional. Indicate that this member used a particular signup form. This is important if you have custom mailings for a particular signup form and so that signup-based triggers will be fired. |

### Subaccount

A subaccount belonging to an Emma enterprise account.

| Property | Type | |
|---|---|---|
| `AccountId` | `string` | The subaccount's Emma account id. Use this to scope calls to it. |
| `AccountName` | `string` | The subaccount's display name. |
| `AdditionalData` | `Dictionary<string, JsonElement>` | Any additional fields Emma returns for a subaccount. |
| `Status` | `string` | Lifecycle status, e.g. `active`, `trial`, `retired`. |

### Subscription

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `CreatedAt` | `DateTime?` |  |
| `DeletedAt` | `DateTime?` |  |
| `Description` | `string` |  |
| `ImportStatus` | `string` |  |
| `MemberCount` | `int?` |  |
| `ModifiedAt` | `string` |  |
| `OptoutCount` | `int?` |  |
| `PurgedAt` | `DateTime?` |  |
| `Settings` | `SubscriptionSettings` |  |
| `SubscriptionId` | `long?` |  |
| `SubscriptionName` | `string` |  |
| `SubscriptionOrder` | `int?` |  |

### SubscriptionBulk

| Property | Type | |
|---|---|---|
| `MemberIds` | `List<long>` |  |

### SubscriptionImportBulk

| Property | Type | |
|---|---|---|
| `ImportId` | `long` |  |

### SubscriptionMembers

| Property | Type | |
|---|---|---|
| `MemberId` | `long` |  |

### SubscriptionNew

| Property | Type | |
|---|---|---|
| `Description` | `string` |  |
| `Name` | `string` |  |

### SubscriptionSettings

| Property | Type | |
|---|---|---|
| `ShowOnDefaultPreferenceForm` | `bool` |  |

### Trigger

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `DeletedAt` | `DateTime?` |  |
| `EventType` | `string` |  |
| `FieldId` | `long?` |  |
| `Groups` | `List<Group>` |  |
| `IsDisabled` | `bool` |  |
| `Links` | `string` |  |
| `Name` | `string` |  |
| `ParentMailing` | `MailingTrigger` |  |
| `ParentMailingId` | `long?` |  |
| `PushOffset` | `string` |  |
| `PushOffsetUnits` | `string` |  |
| `SignupIntegrations` | `string` |  |
| `Signups` | `int?[]` |  |
| `StartTimestamp` | `DateTime?` |  |
| `Surveys` | `string` |  |
| `TriggerId` | `long?` |  |

### UpdateField

| Property | Type | |
|---|---|---|
| `ColumnOrder` | `int?` |  |
| `DisplayName` | `string` |  |
| `FieldType` | `FieldType` |  |
| `WidgetType` | `WidgetType` |  |

### UpdateGroup

| Property | Type | |
|---|---|---|
| `Name` | `string` |  |

### UpdateMailing

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `ArchivedTimestamp` | `DateTime?` |  |
| `CancelByUserId` | `long?` |  |
| `CancelTimestamp` | `DateTime?` |  |
| `CreatedTimestamp` | `DateTime?` |  |
| `Datacenter` | `string` |  |
| `DeletedAt` | `DateTime?` |  |
| `Disabled` | `bool` |  |
| `FailureMessage` | `string` |  |
| `FailureTimestamp` | `DateTime?` |  |
| `HtmlBody` | `string` |  |
| `MailingId` | `long?` |  |
| `MailingStatus` | `MailingStatus` |  |
| `MailingType` | `MailingType` |  |
| `Month` | `int?` |  |
| `Name` | `string` |  |
| `ParentMailingId` | `long?` |  |
| `Plaintext` | `string` |  |
| `PlaintextOnly` | `bool` |  |
| `PurgedAt` | `DateTime?` |  |
| `RecipientCount` | `int?` |  |
| `ReplyTo` | `string` |  |
| `SendAt` | `DateTime?` |  |
| `SendFinished` | `DateTime?` |  |
| `SendStarted` | `DateTime?` |  |
| `Sender` | `string` |  |
| `SignupFormId` | `long?` |  |
| `StartedOrFinished` | `DateTime?` |  |
| `Subject` | `string` |  |
| `Year` | `int?` |  |

### UpdateMember

Update a single member’s information.

| Property | Type | |
|---|---|---|
| `FieldTriggers` | `bool` | Optional. Fires related field change autoresponders when set to true. |
| `Fields` | `Dictionary<string, object>` | An array of fields with associated values for this member |
| `MemberEmail` | `string` | A new email address for the member. |
| `StatusTo` | `MemberStatusShort?` | A new status for the member. Accepts one of ‘a’ (active), ‘e’ (error), ‘o’ (opt-out). |

### UpdateWebhook

Properties associated with updating webhooks

| Property | Type | |
|---|---|---|
| `Event` | `string` |  |
| `Method` | `WebhookMethod` |  |
| `PublicKey` | `string` |  |
| `Url` | `string` |  |

### Webhook

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` | The ID associated with the webhook account |
| `Event` | `string` |  |
| `Method` | `WebhookMethod` |  |
| `Url` | `string` |  |
| `WebhookId` | `long?` | The Id of the webhook |

### WebhookBase

Common Properties to all Webhook classes.

| Property | Type | |
|---|---|---|
| `Event` | `string` | The name of an event to register this webhook for |
| `Method` | `WebhookMethod` | The method to use when calling the webhook. Can be GET or POST. Defaults to POST. |
| `Url` | `string` | The URL to call when the event happens |

### WebhookEvents

| Property | Type | |
|---|---|---|
| `Description` | `string` |  |
| `EventName` | `string` |  |
| `WebhookEventId` | `long?` |  |

### WebhookPostDataMemberSignup

| Property | Type | |
|---|---|---|
| `AccountId` | `string` |  |
| `MailingId` | `long` |  |
| `MemberId` | `string` |  |
| `SignupFormId` | `string` |  |
| `Timestamp` | `DateTime` |  |

### WebhookPostMemberSignup

| Property | Type | |
|---|---|---|
| `Data` | `WebhookPostDataMemberSignup` |  |
| `EventName` | `string` |  |
| `ResourceUrl` | `string` |  |

### Workflow

| Property | Type | |
|---|---|---|
| `CreatedAt` | `DateTime` |  |
| `Name` | `string` |  |
| `Status` | `WorkflowStatus` |  |
| `UpdatedAt` | `DateTime` |  |
| `WorkflowId` | `string` |  |

### WorkflowCount

| Property | Type | |
|---|---|---|
| `AccountId` | `long?` |  |
| `Active` | `int` |  |
| `Draft` | `int` |  |
| `Inactive` | `int` |  |

---

*Generated by `tools/gen-api-reference.cs`. Do not edit by hand — regenerate instead.*
