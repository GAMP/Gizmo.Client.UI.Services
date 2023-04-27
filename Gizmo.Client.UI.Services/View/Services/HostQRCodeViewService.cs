using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class HostQRCodeViewService : ViewStateServiceBase<HostQRCodeViewState>
    {
        public HostQRCodeViewService(HostQRCodeViewState viewState,ILogger<HostQRCodeViewService> logger,
            IGizmoClient gizmoClient,
            IServiceProvider serviceProvider, 
            IOptionsMonitor<HostQRCodeOptions> hostQRCodeOptions) : base(viewState, logger, serviceProvider) 
        { 
            _gizmoClient = gizmoClient;
            _hostQRCodeOptions = hostQRCodeOptions;        
        }

       private readonly IGizmoClient _gizmoClient;
       private readonly IOptionsMonitor<HostQRCodeOptions> _hostQRCodeOptions;

        protected override async Task OnInitializing(CancellationToken ct)
        {
            ViewState.IsEnabled = _hostQRCodeOptions.CurrentValue.Enabled;

            if(_hostQRCodeOptions.CurrentValue.Enabled)
            {
                try
                {
                    //generate host qr code
                    var generateResult = await _gizmoClient.HostQRCodeGeneratAsync(ct);

                    //use with view state
                    ViewState.HostQRCode = generateResult.QRCode;

                    DebounceViewStateChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Host QR Code generation failed.");                   
                }
            }

            await base.OnInitializing(ct);
        }
    }
}
