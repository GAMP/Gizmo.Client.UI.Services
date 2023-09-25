using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class WallpaperViewService : ViewStateServiceBase<WallpaperViewState>
    {
        #region CONSTRUCTOR
        public WallpaperViewService(WallpaperViewState viewState,
            ILogger<WallpaperViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            IOptionsMonitor<ClientInterfaceOptions> clientUIOptions) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _clientUIOptions = clientUIOptions;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly IOptionsMonitor<ClientInterfaceOptions> _clientUIOptions;
        #endregion

        #region FUNCTIONS

        #endregion

        #region OVERRIDES

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.LoginStateChange -= OnLoginStateChange;
            base.OnDisposing(isDisposing);
        }

        private void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            if (e.State == LoginState.LoginCompleted)
            {
                try
                {
                    if (!string.IsNullOrEmpty(_clientUIOptions.CurrentValue.Background))
                    {
                        ViewState.Wallpaper = Path.Combine("https://", "static", Environment.ExpandEnvironmentVariables(_clientUIOptions.CurrentValue.Background))
                            .Replace('\\', '/');
                    }
                    else
                    {
                        //use default
                        ViewState.Wallpaper = "_content/Gizmo.Client.UI/img/background.jpg";
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to load wallpaper.");

                    //use default
                    ViewState.Wallpaper = "_content/Gizmo.Client.UI/img/background.jpg";
                }
            }

            DebounceViewStateChanged();
        }

        #endregion
    }
}
