using System;
using EmmaSharper.Internals;

namespace EmmaSharper.Services
{
    /// <inheritdoc cref="IEmmaAccountScope"/>
    internal sealed class EmmaAccountScope : IEmmaAccountScope
    {
        internal EmmaAccountScope(string accountId, IEmmaApiAdapter adapter)
        {
            AccountId = string.IsNullOrWhiteSpace(accountId)
                ? throw new ArgumentException("An account id is required.", nameof(accountId))
                : accountId;

            // Providers are stateless wrappers over the adapter, so constructing the set eagerly
            // costs ten field assignments. Nothing here opens a connection.
            Account = new AccountProvider(adapter);
            Enterprise = new EnterpriseProvider(adapter);
            Automation = new AutomationProvider(adapter);
            Fields = new FieldsProvider(adapter);
            Groups = new GroupProvider(adapter);
            Mailings = new MailingProvider(adapter);
            Members = new MemberProvider(adapter);
            Responses = new ResponseProvider(adapter);
            Searches = new SearchProvider(adapter);
            SignupForms = new SignupFormProvider(adapter);
            Subscriptions = new SubscriptionProvider(adapter);
            Webhooks = new WebhookProvider(adapter);
        }

        public string AccountId { get; }

        public IEmmaAccountProvider Account { get; }

        public IEmmaEnterpriseProvider Enterprise { get; }

        public IEmmaAutomationProvider Automation { get; }

        public IEmmaFieldsProvider Fields { get; }

        public IEmmaGroupProvider Groups { get; }

        public IEmmaMailingProvider Mailings { get; }

        public IEmmaMemberProvider Members { get; }

        public IEmmaResponseProvider Responses { get; }

        public IEmmaSearchProvider Searches { get; }

        public IEmmaSignupFormProvider SignupForms { get; }

        public IEmmaSubscriptionProvider Subscriptions { get; }

        public IEmmaWebhookProvider Webhooks { get; }
    }
}
