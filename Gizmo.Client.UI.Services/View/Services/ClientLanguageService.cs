using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ClientLanguageService : ViewStateServiceBase<ClientConnectionViewState>
    {
        #region CONSTRUCTOR
        public ClientLanguageService(ClientConnectionViewState viewState, 
            ILogger<ClientLanguageService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
        } 
        #endregion

        public Task SetCurrentLanguageAsync(int lcid)
        {
            if (lcid <= 0)
                Logger.LogError("Invalid language id {lcid} specified.", lcid);

            return Task.CompletedTask;
        }
    }
}
