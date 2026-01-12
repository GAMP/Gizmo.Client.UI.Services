using System.Reflection;
using System.Runtime.InteropServices;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Clients.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Service collection extensions.
    /// </summary>
    public static partial class Extensions
    {
        #region FIELDS
        private static readonly bool IS_WEB_BROWSER = RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser"));
        private static readonly Assembly EXECUTING_ASSEMBLY = Assembly.GetExecutingAssembly();
        private static readonly Assembly UI_ASSEMBLY = typeof(Gizmo.UI.Services.UICompositionServiceBase).Assembly;
        private static readonly UICompositionInMemoryConfiurationSource UI_CONFIGURATION_SOURCE = new();
        private static readonly UIOptionsInMemoryConfigurationSource UI_OPTIONS_CONFIGURATION_SROUCE = new();
        private static readonly TimeSpan API_HTTP_CLIENT_DEFAULT_TIMEOUT = TimeSpan.FromSeconds(15);
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
            services.AddWebApiSupport();

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

            //add and configure required http clients
            services.AddHttpClient(CountryInformationService.HTTP_CLIENT_NAME_REST_COUNTRIES, (client) =>
            {
                client.BaseAddress = new Uri("https://restcountries.com/v3.1/");
                client.Timeout = API_HTTP_CLIENT_DEFAULT_TIMEOUT;
            });
            services.AddHttpClient(CountryInformationService.HTTP_CLIENT_NAME_GEO_PLUGIN, (client) =>
            {
                client.BaseAddress = new Uri("http://www.geoplugin.net");
                client.Timeout = API_HTTP_CLIENT_DEFAULT_TIMEOUT;
            });
            services.AddHttpClient(nameof(ImageService));

            //add country info service, singleton for now
            services.AddSingleton<CountryInformationService>();

            //add default string localizer
            services.AddSingleton<IStringLocalizer, StringLocalizer<Resources.Resources>>();
            services.AddSingleton<AssemblyResourcesLocalizationService>();
            services.AddSingleton<IAssemblyResourcesLocalizationService>(sp => sp.GetRequiredService<AssemblyResourcesLocalizationService>());

            //add localization service

            //use appropriate component discovery service based on current platform
            if (IS_WEB_BROWSER)
            {
                services.AddSingleton<ILocalizationService, WebLocalizationService>();
                services.AddSingleton<WebAssemblyUICompositionService>();
                services.AddSingleton<IUICompositionService>((sp) => sp.GetRequiredService<WebAssemblyUICompositionService>());
            }
            else
            {
                //add in memory configuration store as singleton
                services.AddSingleton<ILocalizationService, WpfLocalizationService>();
                services.AddSingleton((sp) => UI_CONFIGURATION_SOURCE);
                services.AddSingleton((sp) => UI_OPTIONS_CONFIGURATION_SROUCE);
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
            _ = Gizmo.UI.ServiceCollectionExtensions.AddViewServices(services, UI_ASSEMBLY);

            //add any view services in executing assembly Gizmo.Client.UI.Services
            return Gizmo.UI.ServiceCollectionExtensions.AddViewServices(services, EXECUTING_ASSEMBLY);
        }

        /// <summary>
        /// Registers client view states in di container.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>Service collection.</returns>
        private static IServiceCollection AddClientViewStates(this IServiceCollection services)
        {
            //add any view states contained in Gizmo.UI assembly
            _ = Gizmo.UI.ServiceCollectionExtensions.AddViewStates(services, UI_ASSEMBLY);

            //add any view states in executing assembly Gizmo.Client.UI.Services
            return Gizmo.UI.ServiceCollectionExtensions.AddViewStates(services, EXECUTING_ASSEMBLY);
        }

        private static IServiceCollection AddWebApiSupport(this IServiceCollection services)
        {
            services.AddSingleton<UserAccessTokenHandler>();
            services.AddTransient<UserApiClientDelegatingHandler>();
            services.AddSecureWebApiClients(Constants.SecureWebApiClientsName, httpClientConfig);
            services.AddUnsecureWebApiClients(Constants.UnsecureWebApiClientsName, httpClientConfig);

            static void httpClientConfig(IServiceProvider serviceProvider, HttpClient client)
            {
                var webApiSettings = serviceProvider.GetRequiredService<IOptions<ClientNetworkOptions>>().Value;
                var logger = serviceProvider.GetRequiredService<ILogger<ClientNetworkOptions>>();

                var baseUrl = webApiSettings.ServerUri;

                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    var navManager = serviceProvider.GetRequiredService<NavigationManager>();
                    baseUrl = navManager.BaseUri;
                }

                logger.LogInformation("Current base url: {baseUrl}", baseUrl);

                client.BaseAddress = new Uri(baseUrl);
            }

            var httpClientBuilder = services
                 .AddSecureWebApiClients(Constants.SecureWebApiClientsName, httpClientConfig)
                 .WithMessagePackSerialization()
                 .WithCurrentUICultureMessageHandler()
                 .WithMessageHandler<UserApiClientDelegatingHandler>();

            var unsecuredClientBuilder = services.AddUnsecureWebApiClients(Constants.UnsecureWebApiClientsName, httpClientConfig)
                .WithCurrentUICultureMessageHandler()
                .WithMessagePackSerialization();

            //in case we run in desktop process we can allow any https cert
            //in browser environment the cert acceptance will be done on browser level
            if (!IS_WEB_BROWSER)
            {
                httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
                    };
                });

                unsecuredClientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
                    };
                });
            }

            return services;
        }

        #endregion
    }
}
