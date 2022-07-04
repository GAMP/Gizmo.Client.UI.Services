using Gizmo.Client.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Gizmo.Client.UI.Services
{
    public static class ServicesExtensions
    {
        /// <summary>
        /// Registers view states in the di container.
        /// </summary>
        /// <param name="services">Services instance.</param>
        /// <returns>Service collection.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="services"/> equals to null.</exception>
        public static IServiceCollection AddViewStates(this IServiceCollection services)
        {
            return AddViewStates(services,Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Registers view states in the di container.
        /// </summary>
        /// <param name="services">Services instance.</param>
        /// <param name="assembly">Source assembly.</param>
        /// <returns>Service collection.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="assembly"/> or <paramref name="services"/> equals to null.</exception>
        public static IServiceCollection AddViewStates(this IServiceCollection services, Assembly assembly)
        {
            if(services==null)
                throw new ArgumentNullException(nameof(services));

            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var viewStates = assembly
                .GetTypes()
                .Where(type => type.IsAbstract == false && type.GetInterfaces().Contains(typeof(IViewState)))
                .Select(type=> new { Type= type, Attribute= type.GetCustomAttribute<RegisterAttribute>()})
                .Where(result=>result.Attribute!=null)
                .ToList();

            foreach(var viewState in viewStates)
            {
                if(viewState?.Attribute?.Scope== RegisterScope.Scoped)
                {
                    services.AddScoped(viewState.Type);
                }
                else if (viewState?.Attribute?.Scope == RegisterScope.Singelton)
                {
                    services.AddSingleton(viewState.Type);
                }
                else if (viewState?.Attribute?.Scope == RegisterScope.Transient)
                {
                    services.AddTransient(viewState.Type);
                }
            }

            return services;
        }

        /// <summary>
        /// Registers view services in the di container.
        /// </summary>
        /// <param name="services">Services instance.</param>
        /// <param name="assembly">Source assembly.</param>
        /// <returns>Service collection.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="assembly"/> or <paramref name="services"/> equals to null.</exception>
        public static IServiceCollection AddViewServices(this IServiceCollection services, Assembly assembly)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var viewServices = assembly
                .GetTypes()
                .Where(type => type.IsAbstract == false && type.GetInterfaces().Contains(typeof(IViewService)))
                .Select(type => new { Type = type, Attribute = type.GetCustomAttribute<RegisterAttribute>() })
                .Where(result => result.Attribute != null)
                .ToList();

            foreach (var viewService in viewServices)
            {
                if (viewService?.Attribute?.Scope == RegisterScope.Scoped)
                {
                    services.AddScoped(viewService.Type);
                    services.AddScoped(sp => (IViewService)sp.GetRequiredService(viewService.Type));
                }
                else if (viewService?.Attribute?.Scope == RegisterScope.Singelton)
                {
                    services.AddSingleton(viewService.Type);
                    services.AddSingleton(sp => (IViewService)sp.GetRequiredService(viewService.Type));
                }
                else if (viewService?.Attribute?.Scope == RegisterScope.Transient)
                {
                    services.AddTransient(viewService.Type);
                    services.AddTransient(sp=>(IViewService)sp.GetRequiredService(viewService.Type));
                }
            }

            return services;
        }

        /// <summary>
        /// Registers view services in the di container.
        /// </summary>
        /// <param name="services">Services instance.</param>
        /// <returns>Service collection.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="services"/> equals to null.</exception>
        public static IServiceCollection AddViewServices(this IServiceCollection services)
        {
            return AddViewServices(services, Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Initializes view services.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Associated task.</returns>
        public static async Task InitializeViewsServices(this IServiceProvider serviceProvider,CancellationToken ct=default)
        {
            //get view services
            var viewServices = serviceProvider.GetServices<IViewService>();

            //initialize view services
            foreach (var service in viewServices)
            {
                await service.IntializeAsync(ct);
            }
        }

    }
}
