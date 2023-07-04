using Gizmo.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.Services
{
    public static partial class Extensions
    {
        #region FUNCTIONS
        
        /// <summary>
        /// Adds client configuration source.
        /// </summary>
        /// <param name="configuration">Configuration builder.</param>
        /// <returns>Configuration builder.</returns>
        public static IConfigurationBuilder AddClientConfigurationSource(this IConfigurationBuilder configuration)
        {
            if (_isWebBrowser)
            {
                configuration.AddJsonFile("appsettings.json", true);
            }
            else
            {
                configuration.Add(_uiCompositionConfiurationSource);
                configuration.Add(_uiOptionsConfigurationSource);
            }

            return configuration;
        }

        /// <summary>
        /// Adds client options.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">Configuration.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddClientOptions(this IServiceCollection services, IConfiguration configuration)
        {
            //bind client app configuration to the desired class
            services.AddOptions<UICompositionOptions>().Bind(configuration.GetSection("UIComposition"));
            services.AddOptions<ClientInterfaceOptions>().Bind(configuration.GetSection("ClientInterface"));
            services.AddOptions<LogoOptions>().Bind(configuration.GetSection("Logo"));
            services.AddOptions<CurrencyOptions>().Bind(configuration.GetSection("Currency"));
            services.AddOptions<UserOnlineDepositOptions>().Bind(configuration.GetSection("UserOnlineDeposit"));
            services.AddOptions<PopularItemsOptions>().Bind(configuration.GetSection("PopularItems"));
            services.AddOptions<UserLoginOptions>().Bind(configuration.GetSection("UserLogin"));
            services.AddOptions<HostQRCodeOptions>().Bind(configuration.GetSection("HostQRCode"));
            services.AddOptions<FeedsOptions>().Bind(configuration.GetSection("Feeds"));
            services.AddOptions<NotificationsOptions>().Bind(configuration.GetSection("Notifications"));
            services.AddOptions<DialogOptions>().Bind(configuration.GetSection("Dialog"));
            services.AddOptions<LoginRotatorOptions>().Bind(configuration.GetSection("LoginRotator"));
            services.AddOptions<ClientShopOptions>().Bind(configuration.GetSection("Shop"));
            services.AddOptions<PasswordValidationOptions>().Bind(configuration.GetSection("Validation:Password"));
            services.AddOptions<IntegrationOptions>().Bind(configuration.GetSection("Integration"));
            return services;
        } 

        #endregion
    }
}
