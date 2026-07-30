using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace EmmaSharper.Unit.Tests
{
    public class TEmmaExtensions
    {
        private const string BaseUrl = "https://api.example.test";

        private static ServiceProvider BuildProvider()
        {
            ServiceCollection services = new();
            services.AddLogging();
            services.AddEmmaApiProviders(options =>
            {
                options.BaseUrl = BaseUrl;
                options.AccountId = "account-id";
                options.PublicKey = "public-key";
                options.SecretKey = "secret-key";
            });

            return services.BuildServiceProvider();
        }

        [Theory]
        [InlineData(typeof(IEmmaApiAdapter))]
        [InlineData(typeof(IEmmaAutomationProvider))]
        [InlineData(typeof(IEmmaFieldsProvider))]
        [InlineData(typeof(IEmmaGroupProvider))]
        [InlineData(typeof(IEmmaMailingProvider))]
        [InlineData(typeof(IEmmaMemberProvider))]
        [InlineData(typeof(IEmmaResponseProvider))]
        [InlineData(typeof(IEmmaSearchProvider))]
        [InlineData(typeof(IEmmaSignupFormProvider))]
        [InlineData(typeof(IEmmaSubscriptionProvider))]
        [InlineData(typeof(IEmmaWebhookProvider))]
        public void AddEmmaApiProviders_WithAction_ResolvesEveryService(Type type)
        {
            using ServiceProvider provider = BuildProvider();

            provider.GetRequiredService(type).Should().NotBeNull();
        }

        [Fact]
        public void AddEmmaApiProviders_ReturnsHttpClientBuilder_SoResilienceCanBeAttached()
        {
            ServiceCollection services = new();
            services.AddLogging();

            IHttpClientBuilder builder = services.AddEmmaApiProviders(options =>
            {
                options.BaseUrl = BaseUrl;
                options.PublicKey = "public-key";
                options.SecretKey = "secret-key";
            });

            builder.Should().NotBeNull();
            builder.Name.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void AddEmmaApiProviders_WithConfiguration_BindsTheEmmaSectionByDefault()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Emma:BaseUrl", BaseUrl },
                    { "Emma:AccountId", "account-id" },
                    { "Emma:PublicKey", "public-key" },
                    { "Emma:SecretKey", "secret-key" },
                })
                .Build();

            ServiceCollection services = new();
            services.AddLogging();
            services.AddEmmaApiProviders(configuration);

            using ServiceProvider provider = services.BuildServiceProvider();
            EmmaOptions options = provider.GetRequiredService<IOptions<EmmaOptions>>().Value;

            options.BaseUrl.Should().Be(BaseUrl);
            options.AccountId.Should().Be("account-id");
            options.PublicKey.Should().Be("public-key");
            options.SecretKey.Should().Be("secret-key");
        }

        [Fact]
        public void AddEmmaApiProviders_WithNullSection_BindsConfigurationRoot()
        {
            // 7.x behaviour: keys at the very top of appsettings.json.
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "BaseUrl", BaseUrl },
                    { "PublicKey", "public-key" },
                    { "SecretKey", "secret-key" },
                })
                .Build();

            ServiceCollection services = new();
            services.AddLogging();
            services.AddEmmaApiProviders(configuration, sectionName: null);

            using ServiceProvider provider = services.BuildServiceProvider();

            provider.GetRequiredService<IOptions<EmmaOptions>>().Value.BaseUrl.Should().Be(BaseUrl);
        }

        [Fact]
        public void AddEmmaApiProviders_MissingCredentials_FailsValidationOnResolve()
        {
            ServiceCollection services = new();
            services.AddLogging();
            services.AddEmmaApiProviders(options => options.BaseUrl = BaseUrl);

            using ServiceProvider provider = services.BuildServiceProvider();

            Action act = () => _ = provider.GetRequiredService<IOptions<EmmaOptions>>().Value;

            act.Should().Throw<OptionsValidationException>();
        }
    }
}
