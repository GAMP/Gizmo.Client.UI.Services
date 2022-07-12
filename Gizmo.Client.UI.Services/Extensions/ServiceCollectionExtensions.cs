using Gizmo.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        #region FIELDS
        private static readonly bool _isWebAssembly = RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY"));
        private static readonly Assembly _executingAssembly = Assembly.GetExecutingAssembly();
        #endregion

        #region FUNCTIONS

        /// <summary>
        /// Registers UI services in di container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddClientUIServices(this IServiceCollection services)
        {          
            //add localization service
            services.AddSingleton<ILocalizationService, UILocalizationService>();

            //use appropriate component discovery service based on current platform
            if (_isWebAssembly)
            {
                services.AddSingleton<IComponentDiscoveryService, WebAssemblyComponentDiscoveryService>();
            }
            else
            {
                services.AddSingleton<IComponentDiscoveryService, DesktopComponentDiscoveryService>();
            }

            //add any Gizmo.UI services.
            Gizmo.UI.ServiceCollectionExtensions.AddUIServices(services);

            return services;
        }         

        public static IServiceCollection AddClientViewServices(this IServiceCollection services)
        {
            //add any Gizmo.UI view services.
            return Gizmo.UI.ServiceCollectionExtensions.AddViewServices(services, _executingAssembly);
        }

        public static IServiceCollection AddClientViewStates(this IServiceCollection services)
        {
            //add any Gizmo.UI view states.
            return Gizmo.UI.ServiceCollectionExtensions.AddViewStates(services, _executingAssembly);
        }

        #endregion
    }
}
