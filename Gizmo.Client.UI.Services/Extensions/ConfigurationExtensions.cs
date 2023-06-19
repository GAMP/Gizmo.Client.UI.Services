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
            services.AddOptions<ClientUIOptions>().Bind(configuration.GetSection("Interface"));
            services.AddOptions<CurrencyOptions>().Bind(configuration.GetSection("CurrencyOptions"));
            services.AddOptions<UserOnlineDepositOptions>().Bind(configuration.GetSection("UserOnlineDepositOptions"));
            services.AddOptions<PopularItemsOptions>().Bind(configuration.GetSection("PopularItemsOptions"));
            services.AddOptions<UserLoginOptions>().Bind(configuration.GetSection("UserLoginOptions"));
            services.AddOptions<HostQRCodeOptions>().Bind(configuration.GetSection("HostQRCodeOptions"));
            services.AddOptions<FeedsOptions>().Bind(configuration.GetSection("FeedsOptions"));
            services.AddOptions<NotificationsOptions>().Bind(configuration.GetSection("NotificationsOptions"));
            services.AddOptions<DialogOptions>().Bind(configuration.GetSection("DialogOptions"));
            return services;
        } 

        #endregion
    }
}
