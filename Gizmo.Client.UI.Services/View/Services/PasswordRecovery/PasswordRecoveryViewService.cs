using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public sealed class PasswordRecoveryViewService : ValidatingViewStateServiceBase<PasswordRecoveryViewState>
    {
        private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly IPasswordRecoverySessionService _session;
        private readonly ILocalizationService _localizationService;

        public PasswordRecoveryViewService(
            PasswordRecoveryViewState viewState,
            ILogger<PasswordRecoveryViewService> logger,
            IServiceProvider serviceProvider,
            IPasswordRecoveryService passwordRecoveryService,
            IPasswordRecoverySessionService session,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _passwordRecoveryService = passwordRecoveryService;
            _session = session;
            _localizationService = localizationService;
        }

        public void SetMatchValue(string value)
        {
            ViewState.MatchValue = value;
            ValidateProperty(() => ViewState.MatchValue);
        }

        public async Task SubmitAsync()
        {
            if (ViewState.IsLoading)
                return;

            ViewState.IsLoading = true;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
                return;
            }

            try
            {
                var result = await _passwordRecoveryService.StartAsync(new PasswordRecoveryStartRequest
                {
                    MatchValue = ViewState.MatchValue
                });

                switch (result.Result)
                {
                    case PasswordRecoveryStartCode.Success:
                        _session.SetMatchValue(ViewState.MatchValue);
                        _session.SetStartResult(
                            result.Token ?? string.Empty,
                            result.Destination ?? string.Empty,
                            result.CodeLength,
                            result.ExpiresInSeconds);
                        NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryConfirmationRoute);
                        break;

                    case PasswordRecoveryStartCode.NoRouteForDelivery:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CONFIRMATION_ERROR_PROVIDER_NO_ROUTE_FOR_DELIVERY));
                        break;

                    default:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RECOVERY_PASSWORD_RESET_FAILED_MESSAGE));
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Password recovery start error.");
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            _session.Clear();
            ViewState.MatchValue = string.Empty;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ResetValidationState();
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }
    }
}
