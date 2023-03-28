using Gizmo.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Service collection extensions.
    /// </summary>
    public static partial class Extensions
    {
        #region FIELDS
        private static readonly bool _isWebBrowser = RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser"));
        private static readonly Assembly _executingAssembly = Assembly.GetExecutingAssembly();
        private static readonly Assembly _uiAssembly = typeof(Gizmo.UI.Services.UICompositionServiceBase).Assembly;
        private static readonly UICompositionInMemoryConfiurationSource _uiCompositionConfiurationSource = new();
        private static readonly UIOptionsInMemoryConfigurationSource _uiOptionsConfigurationSource = new();
        #endregion

        #region FUNCTIONS

        /// <summary>
        /// Registers all related services in di container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddClientServices(this IServiceCollection services)
        {
            services.AddClientUIServices();
            services.AddClientViewServices();
            services.AddClientViewStates();

            return services;
        }

        /// <summary>
        /// Registers UI services in di container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>Service collection.</returns>
        private static IServiceCollection AddClientUIServices(this IServiceCollection services)
        {
            //add js interop service
            services.AddScoped<JSInteropService>();

            //add localization with default options
            services.AddLocalization(opt =>
            {
                opt.ResourcesPath = "Properties";
            });

            //add default string localizer
            services.AddSingleton<IStringLocalizer, StringLocalizer<Resources.Resources>>();

            //add localization service
            services.AddSingleton<ILocalizationService, UILocalizationService>();

            //use appropriate component discovery service based on current platform
            if (_isWebBrowser)
            {
                services.AddSingleton<WebAssemblyUICompositionService>();
                services.AddSingleton<IUICompositionService>((sp) => sp.GetRequiredService<WebAssemblyUICompositionService>());
            }
            else
            {
                //add in memory configuration store as singelton
                services.AddSingleton((sp) => _uiCompositionConfiurationSource);
                services.AddSingleton((sp) => _uiOptionsConfigurationSource);
                services.AddSingleton<DesktopUICompositionService>();
                services.AddSingleton<IUICompositionService>((sp) => sp.GetRequiredService<DesktopUICompositionService>());
            }

            //add any Gizmo.UI services.
            Gizmo.UI.ServiceCollectionExtensions.AddUIServices(services);

            return services;
        }

        /// <summary>
        /// Registers client view services in di container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>Service collection.</returns>
        private static IServiceCollection AddClientViewServices(this IServiceCollection services)
        {
            //add any view services contained in Gizmo.UI assembly
            _ = Gizmo.UI.ServiceCollectionExtensions.AddViewServices(services, _uiAssembly);

            //add any view services in executing assembly Gizmo.Client.UI.Services
            return Gizmo.UI.ServiceCollectionExtensions.AddViewServices(services, _executingAssembly);
        }

        /// <summary>
        /// Registers client view states in di container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>Service collection.</returns>
        private static IServiceCollection AddClientViewStates(this IServiceCollection services)
        {
            //add any view states contained in Gizmo.UI assembly
            _ = Gizmo.UI.ServiceCollectionExtensions.AddViewStates(services, _uiAssembly);

            //add any view states in executing assembly Gizmo.Client.UI.Services
            return Gizmo.UI.ServiceCollectionExtensions.AddViewStates(services, _executingAssembly);
        }

        #endregion
    }
}
