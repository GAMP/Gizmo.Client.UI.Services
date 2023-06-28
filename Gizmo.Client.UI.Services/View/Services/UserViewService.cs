using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserViewService : ViewStateServiceBase<UserViewState>, IDisposable
    {
        public UserViewService(UserViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<UserViewService> logger,
            IServiceProvider serviceProvider,
            IClientDialogService dialogService,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _dialogService = dialogService;
            _localizationService = localizationService;
        }

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly IClientDialogService _dialogService;
        private readonly ILocalizationService _localizationService;
        #endregion

        public async Task LogoutWithConfirmationAsync()
        {
            try
            {
                var s = await _dialogService.ShowAlertDialogAsync(_localizationService.GetString("GIZ_LOGOUT_VERIFICATION_TITLE"), _localizationService.GetString("GIZ_LOGOUT_VERIFICATION_MESSAGE"), AlertDialogButtons.YesNo);
                if (s.Result == AddComponentResultCode.Opened)
                {
                    var result = await s.WaitForResultAsync();

                    if (s.Result == AddComponentResultCode.Ok && result!.Button == AlertDialogResultButton.Yes)
                        await _gizmoClient.UserLogoutAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User initiated logout failed.");
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _gizmoClient.UserLogoutAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User initiated logout failed.");
            }
        }
    }
}
