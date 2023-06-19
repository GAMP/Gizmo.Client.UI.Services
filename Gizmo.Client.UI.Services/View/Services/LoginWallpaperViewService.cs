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
            IOptions<ClientUIOptions> clientUIOptions) : base(viewState, logger, serviceProvider)
        {
            _clientUIOptions = clientUIOptions;
        }
        #endregion

        #region FIELDS
        private readonly IOptions<ClientUIOptions> _clientUIOptions;
        #endregion

        #region FUNCTIONS

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            ViewState.Wallpaper = "_content/Gizmo.Client.UI/img/" + _clientUIOptions.Value.Background;

            return Task.CompletedTask;
        }

        #endregion
    }
}
