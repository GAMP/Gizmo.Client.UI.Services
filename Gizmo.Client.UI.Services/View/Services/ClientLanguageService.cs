using Gizmo.Client.UI.View.States;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    public sealed class ClientLanguageService : ViewStateServiceBase<ClientConnectionViewState>
    {
        #region CONSTRUCTOR
        public ClientLanguageService(ClientConnectionViewState viewState, ILogger<ClientLanguageService> logger) : base(viewState, logger)
        {
        } 
        #endregion

        public Task SetCurrentLanguageAsync(int lcid)
        {
            if (lcid <= 0)
                Logger.LogError("Invalid language id {lcid} specified.", lcid);

            return Task.CompletedTask;
        }

        public override Task IntializeAsync(CancellationToken ct)
        {
            return base.IntializeAsync(ct);
        }
    }
}
