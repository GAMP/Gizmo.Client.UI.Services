using Gizmo.Shared.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Service provider extensions.
    /// </summary>
    public static partial class Extensions
    {
        #region FUNCTIONS

        /// <summary>
        /// Initializes all client services.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Associated task.</returns>
        public static async Task InitializeClientServices(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            await InitializeClientUIServices(serviceProvider, cancellationToken);
            await InitializeClientViewServices(serviceProvider, cancellationToken);
        }

        /// <summary>
        /// Initializes client services.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Associated task.</returns>
        private static async Task InitializeClientUIServices(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            var services = serviceProvider.GetServices<IComponentDiscoveryService>();
            foreach (var service in services)
            {
                await service.InitializeAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Initializes client view services.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Associated task.</returns>
        private static Task InitializeClientViewServices(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            return Gizmo.UI.ServiceProviderExtensions.InitializeViewsServices(serviceProvider, cancellationToken);
        }

        #endregion
    }
}
