using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Component discovery service used in desktop host.
    /// </summary>
    public class DesktopComponentDiscoveryService : ComponentDiscoveryServiceBase
    {
        #region CONSTRUCTOR
        public DesktopComponentDiscoveryService(IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILogger<DesktopComponentDiscoveryService> logger) : base(configuration, logger, serviceProvider)
        {
        }
        #endregion

        #region OVERRIDES
        
        protected override IEnumerable<string> GetRoutes(Type type)
        {
            return type.GetCustomAttributes<RouteAttribute>().Select(attribute => attribute.Template).ToArray();
        } 

        #endregion
    }
}
