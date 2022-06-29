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
                .Where(type => type.GetInterfaces().Contains(typeof(IViewState)))
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
    }
}
