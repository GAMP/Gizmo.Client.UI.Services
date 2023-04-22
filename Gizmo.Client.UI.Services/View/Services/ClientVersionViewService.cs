using System.Reflection;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// Client version view state service.
    /// </summary>
    /// <remarks>
    /// The service is responsible of obtaining executing Gizmo Client version.
    /// </remarks>
    [Register()]
    public sealed class ClientVersionViewService : ViewStateServiceBase<ClientVersionViewState>
    {
        public ClientVersionViewService(ClientVersionViewState state,
            ILogger<ClientVersionViewService> logger,
            IServiceProvider serviceProvider)
            : base(state, logger, serviceProvider)
        { }

        protected override Task OnInitializing(CancellationToken ct)
        {
            try
            {
                //since the version will not change and available upon application startup
                //we can initialize the view state once here

                //create empty version by default
                ViewState.Version = new Version(0, 0, 0).ToString();

                //obtain version information from entry assembly, entry assembly will be the main executable
                //in desktop GizmoClient.exe in web blazor web host app
                var entryAssembly = Assembly.GetEntryAssembly();
                var version = entryAssembly?.GetName()?.Version;

                //update view state version
                if (version != null)
                    ViewState.Version = new Version(version.Major,version.Minor,version.Build).ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Could not obtain client version from entry assembly.");
            }

            return base.OnInitializing(ct);
        } 
    }
}
