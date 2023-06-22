using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.LoginRoute)]
    public sealed class LoginWallpaperViewService : ViewStateServiceBase<LoginWallpaperViewState>
    {
        #region CONSTRUCTOR
        public LoginWallpaperViewService(LoginWallpaperViewState viewState,
            ILogger<LoginWallpaperViewService> logger,
            IServiceProvider serviceProvider,
            IOptions<ClientInterfaceOptions> clientUIOptions) : base(viewState, logger, serviceProvider)
        {
            _clientUIOptions = clientUIOptions;
        }
        #endregion

        #region FIELDS
        private readonly IOptions<ClientInterfaceOptions> _clientUIOptions;
        #endregion

        #region FUNCTIONS

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            if (!string.IsNullOrEmpty(_clientUIOptions.Value.Background))
            {
                ViewState.Wallpaper = Path.Combine("https://", "static", Environment.ExpandEnvironmentVariables(_clientUIOptions.Value.Background))
                    .Replace('\\', '/');
            }
            else
            {
                ViewState.Wallpaper = "_content/Gizmo.Client.UI/img/login-background.jpg";
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
