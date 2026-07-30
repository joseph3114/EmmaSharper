namespace EmmaSharper
{
    /// <summary>
    /// The full set of Emma providers, bound to one specific account id.
    /// </summary>
    /// <remarks>
    /// Obtained from <see cref="IEmmaAccountScopeFactory.ForAccount(string)"/>. A scope reuses the
    /// same credentials and the same pooled <see cref="System.Net.Http.HttpClient"/> as the
    /// default providers - only the account segment of the request path differs.
    /// </remarks>
    public interface IEmmaAccountScope
    {
        /// <summary>The account id every call made through this scope targets.</summary>
        string AccountId { get; }

        /// <summary>Account-level endpoints, such as the account's users.</summary>
        IEmmaAccountProvider Account { get; }

        /// <summary>
        /// Enterprise endpoints. Only meaningful when the scoped account is itself an enterprise
        /// account; scoping to a subaccount and calling this will not list that subaccount's peers.
        /// </summary>
        IEmmaEnterpriseProvider Enterprise { get; }

        /// <summary>Automation workflows for this account.</summary>
        IEmmaAutomationProvider Automation { get; }

        /// <summary>Member fields for this account.</summary>
        IEmmaFieldsProvider Fields { get; }

        /// <summary>Groups for this account.</summary>
        IEmmaGroupProvider Groups { get; }

        /// <summary>Mailings for this account.</summary>
        IEmmaMailingProvider Mailings { get; }

        /// <summary>Members for this account.</summary>
        IEmmaMemberProvider Members { get; }

        /// <summary>Mailing response data for this account.</summary>
        IEmmaResponseProvider Responses { get; }

        /// <summary>Saved searches for this account.</summary>
        IEmmaSearchProvider Searches { get; }

        /// <summary>Signup forms for this account.</summary>
        IEmmaSignupFormProvider SignupForms { get; }

        /// <summary>Subscriptions for this account.</summary>
        IEmmaSubscriptionProvider Subscriptions { get; }

        /// <summary>Webhooks for this account.</summary>
        IEmmaWebhookProvider Webhooks { get; }
    }
}
