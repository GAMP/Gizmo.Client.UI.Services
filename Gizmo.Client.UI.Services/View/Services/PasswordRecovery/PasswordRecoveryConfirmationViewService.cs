using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    [Route(ClientRoutes.PasswordRecoveryConfirmationRoute)]
    public sealed class PasswordRecoveryConfirmationViewService : ValidatingViewStateServiceBase<PasswordRecoveryConfirmationViewState>
    {
        private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly IPasswordRecoverySessionService _session;
        private readonly ILocalizationService _localizationService;

        public PasswordRecoveryConfirmationViewService(
            PasswordRecoveryConfirmationViewState viewState,
            ILogger<PasswordRecoveryConfirmationViewService> logger,
            IServiceProvider serviceProvider,
            IPasswordRecoveryService passwordRecoveryService,
            IPasswordRecoverySessionService session,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _passwordRecoveryService = passwordRecoveryService;
            _session = session;
            _localizationService = localizationService;
        }

        public void SetConfirmationCode(string value)
        {
            ViewState.ConfirmationCode = value;
            ValidateProperty(() => ViewState.ConfirmationCode);
        }

        public async Task Confirm()
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
                var result = await _passwordRecoveryService.ConfirmCodeAsync(_session.Token, ViewState.ConfirmationCode);

                switch (result)
                {
                    case PasswordRecoveryConfirmCode.Success:
                        _session.SetCodeConfirmed(true);
                        NavigationService.NavigateTo(ClientRoutes.PasswordRecoverySetNewPasswordRoute);
                        break;

                    case PasswordRecoveryConfirmCode.InvalidToken:
                    case PasswordRecoveryConfirmCode.ExpiredToken:
                    case PasswordRecoveryConfirmCode.UsedToken:
                    case PasswordRecoveryConfirmCode.InvalidConfirmationCode:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CONFIRMATION_CONFIRMATION_CODE_IS_INVALID));
                        break;

                    default:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Confirm password recovery code error.");
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

        public void Clear()
        {
            ViewState.ConfirmationCode = string.Empty;
            ViewState.ConfirmationCodeMessage = string.Empty;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ResetValidationState();
            DebounceViewStateChanged();
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_session.Token))
            {
                NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryRoute);
                return Task.CompletedTask;
            }

            ViewState.ConfirmationCodeMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RECOVERY_PLEASE_ENTER_RECOVERY_CODE), _session.Destination);
            ViewState.ConfirmationCode = string.Empty;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ResetValidationState();
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.ConfirmationCode) &&
                ViewState.ConfirmationCode.Length != _session.CodeLength)
            {
                AddError(() => ViewState.ConfirmationCode, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_CONFIRMATION_CODE_LENGTH_ERROR), _session.CodeLength));
            }
        }
    }
}
