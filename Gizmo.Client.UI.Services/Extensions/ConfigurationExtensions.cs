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
            services.Configure<UICompositionOptions>(configuration.GetSection("UIComposition"));
            services.Configure<ClientUIOptions>(configuration.GetSection("Interface"));

            return services;
        } 

        #endregion
    }
}
