using Gizmo.Client.UI.View.States;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Gizmo.Client.UI.View.Services
{
    public sealed class SystemLanguageService : ViewStateServiceBase<SystemLanguageViewState>
    {
        #region CONSTRUCTOR
        public SystemLanguageService(SystemLanguageViewState viewState, ILogger<SystemLanguageService> logger) : base(viewState, logger)
        {
        }
        #endregion

        #region FIELDS
        private readonly bool _isWebAssembly = RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY"));
        private System.Threading.Timer _timer;
        #endregion

        public Task SetCurrentLanguageAsync(int lcid)
        {
            if (_isWebAssembly)
                throw new NotSupportedException();

            if (lcid <= 0)
                Logger.LogError("Invalid language id {lcid} specified.", lcid);

            return Task.CompletedTask;
        }

        public override Task IntializeAsync(CancellationToken ct)
        {
            //since our application can run in web browser
            //we need to check if system language selection is possible based on the platform
            //and provide the information to the view state

            if(_isWebAssembly)
            {
                return Task.CompletedTask;
            }
            else
            {

            }
            _timer = new Timer(CB, null, 1000, 1000);
            return base.IntializeAsync(ct);
        }

        private void CB(object? s)
        {
            ViewState.SelectedLanguage = new LanguageViewState() { NativeName = DateTime.Now.ToString() };
            ViewState.RaiseChanged();
        }
    }
}
