using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using EmmaSharper.Adapters;
using EmmaSharper.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: InternalsVisibleTo("EmmaSharper.Unit")]

namespace EmmaSharper
{
    /// <summary>Extension methods for registering <see cref="EmmaSharper"/> with the DI container.</summary>
    public static class EmmaSharperExtensions
    {
        /// <summary>Default configuration section bound by the <see cref="IConfiguration"/> overload.</summary>
        public const string DefaultSectionName = "Emma";

        /// <summary>Adds the Emma API providers, binding options from configuration.</summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">Configuration to bind from.</param>
        /// <param name="sectionName">
        /// Section to bind. Defaults to <see cref="DefaultSectionName"/>. Pass <c>null</c> to bind
        /// the root, which is what 7.x did unconditionally - it required AccountId, PublicKey and
        /// SecretKey to sit at the very top of appsettings.json.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpClientBuilder"/> for the Emma client, so callers can attach a
        /// resilience handler - see <c>EmmaRetryDefaults.ShouldHandle</c>.
        /// </returns>
        public static IHttpClientBuilder AddEmmaApiProviders(
            this IServiceCollection services,
            IConfiguration configuration,
            string? sectionName = DefaultSectionName)
        {
            // `!` because IsNullOrWhiteSpace guarantees non-null in this branch. The compiler
            // knows that on net8.0+ via [NotNullWhen(false)], but the netstandard2.0 reference
            // assemblies carry no nullable annotations, so it needs telling there.
            IConfiguration section = string.IsNullOrWhiteSpace(sectionName)
                ? configuration
                : configuration.GetSection(sectionName!);

            return services.AddEmmaApiProviders(section.Bind);
        }

        /// <summary>Adds the Emma API providers with options configured in code.</summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">Configures <see cref="EmmaOptions"/>.</param>
        /// <returns>The <see cref="IHttpClientBuilder"/> for the Emma client.</returns>
        public static IHttpClientBuilder AddEmmaApiProviders(
            this IServiceCollection services,
            Action<EmmaOptions> configure)
        {
            // 7.x registered EmmaOptions as Transient and re-ran the configure delegate on every
            // resolution. The options pattern builds it once and validates eagerly.
            services.AddOptions<EmmaOptions>()
                    .Configure(configure)
                    .Validate(
                        o => !string.IsNullOrWhiteSpace(o.PublicKey) && !string.IsNullOrWhiteSpace(o.SecretKey),
                        $"{nameof(EmmaOptions)}.{nameof(EmmaOptions.PublicKey)} and " +
                        $"{nameof(EmmaOptions.SecretKey)} are required.")
                    .Validate(
                        o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _),
                        $"{nameof(EmmaOptions)}.{nameof(EmmaOptions.BaseUrl)} must be an absolute URI.");

            // A pooled, factory-managed handler. 7.x built a new client per request from a
            // Transient factory, which is the classic socket-exhaustion / stale-DNS pattern -
            // and this client is driven concurrently during a subaccount sweep.
            IHttpClientBuilder builder = services.AddHttpClient<IEmmaApiAdapter, EmmaApiAdapter>(
                static (provider, client) =>
                {
                    EmmaOptions options = provider.GetRequiredService<IOptions<EmmaOptions>>().Value;

                    client.BaseAddress = new Uri(options.BaseUrl);
                    client.Timeout = options.Timeout;
                    client.DefaultRequestHeaders.Authorization = BasicAuth(options);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });

            services.AddEmmaProviders();

            return builder;
        }

        private static AuthenticationHeaderValue BasicAuth(EmmaOptions options)
        {
            string pair = $"{options.PublicKey}:{options.SecretKey}";
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(pair)));
        }

        /// <summary>Registers the provider implementations.</summary>
        /// <remarks>Providers are stateless, so transient is intentional here.</remarks>
        internal static void AddEmmaProviders(this IServiceCollection services)
        {
            // Transient, not singleton: it captures the typed HttpClient-backed adapter, which the
            // factory registers as transient. A singleton here would be a captive dependency.
            services.AddTransient<IEmmaAccountScopeFactory, EmmaAccountScopeFactory>();

            services.AddTransient<IEmmaAccountProvider, AccountProvider>();
            services.AddTransient<IEmmaEnterpriseProvider, EnterpriseProvider>();

            services.AddTransient<IEmmaAutomationProvider, AutomationProvider>();
            services.AddTransient<IEmmaFieldsProvider, FieldsProvider>();
            services.AddTransient<IEmmaGroupProvider, GroupProvider>();
            services.AddTransient<IEmmaMailingProvider, MailingProvider>();
            services.AddTransient<IEmmaMemberProvider, MemberProvider>();
            services.AddTransient<IEmmaResponseProvider, ResponseProvider>();
            services.AddTransient<IEmmaSearchProvider, SearchProvider>();
            services.AddTransient<IEmmaSignupFormProvider, SignupFormProvider>();
            services.AddTransient<IEmmaSubscriptionProvider, SubscriptionProvider>();
            services.AddTransient<IEmmaWebhookProvider, WebhookProvider>();
        }

        /// <summary>Convert <see cref="Enum"/> to <see cref="string"/>.</summary>
        internal static string ToEnumString<T>(this T @enum) where T : Enum
        {
            string value = @enum.ToString();
            EnumMemberAttribute? attribute = typeof(T).GetField(value)?
                .GetCustomAttributes<EnumMemberAttribute>(false)
                .SingleOrDefault();

            return attribute is null ? value : attribute.Value!;
        }

        /// <summary>Converts an <see cref="Enum"/> into its direct string value, or attribute value when attributed.</summary>
        internal static IEnumerable<string> AsEnumStrings<T>(this IEnumerable<T> enums) where T : Enum
            => enums.Select(x => x.ToEnumString());

        /// <summary>Syntactic sugar for <see cref="string.Join(string, IEnumerable{string})"/>.</summary>
        internal static string JoinWith(this IEnumerable<string> items, char seperator)
            // string.Join(char, ...) is netstandard2.1+; the string overload exists everywhere.
            => string.Join(seperator.ToString(), items);
    }
}
