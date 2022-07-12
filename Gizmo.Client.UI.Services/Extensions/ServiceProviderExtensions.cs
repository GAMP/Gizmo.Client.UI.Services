using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Service provider extensions.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        #region FUNCTIONS
        
        /// <summary>
        /// Initializes client services.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public static async Task InitializeClientServices(this IServiceProvider serviceProvider, CancellationToken cancellation = default)
        {
            var services = serviceProvider.GetServices<IComponentDiscoveryService>();
            foreach (var service in services)
            {
                await service.InitializeAsync(cancellation);
            }
        }

        public static Task InitializeClientViewServices(this IServiceProvider serviceProvider, CancellationToken cancellation = default)
        {
           return Gizmo.UI.ServiceProviderExtensions.InitializeViewsServices(serviceProvider, cancellation);
        }

        #endregion
    }
}
