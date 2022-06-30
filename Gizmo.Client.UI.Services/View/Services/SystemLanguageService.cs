using Gizmo.Client.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class SystemLanguageService : ClientViewServiceBase<SystemLanguageViewState>
    {
        #region CONSTRUCTOR
        public SystemLanguageService(SystemLanguageViewState viewState,
            ILogger<SystemLanguageService> logger, 
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
        }
        #endregion

        #region FIELDS
        private readonly bool _isWebAssembly = RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY"));
        #endregion

        public Task SetCurrentLanguageAsync(int lcid)
        {
            if (_isWebAssembly)
                throw new NotSupportedException();

            if (lcid <= 0)
                Logger.LogError("Invalid language id {lcid} specified.", lcid);

            return Task.CompletedTask;
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            //since our application can run in web browser
            //we need to check if system language selection is possible based on the platform
            //and provide the information to the view state

            if (_isWebAssembly)
            {
                return Task.CompletedTask;
            }
            else
            {

            }
            return base.OnInitializing(ct);
        }
    }
}
