using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationConfirmationService : ValidatingViewStateServiceBase<UserRegistrationConfirmationViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationConfirmationService(UserRegistrationConfirmationViewState viewState,
            ILogger<UserRegistrationConfirmationService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            UserRegistrationConfirmationMethodViewState userRegistrationConfirmationMethodViewState) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _userRegistrationConfirmationMethodViewState = userRegistrationConfirmationMethodViewState;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly UserRegistrationConfirmationMethodViewState _userRegistrationConfirmationMethodViewState;
        #endregion

        #region FUNCTIONS

        public async Task SetConfirmationCode(string value)
        {
            ViewState.ConfirmationCode = value;
            await ValidatePropertyAsync((x) => x.ConfirmationCode);
            DebounceViewStateChanged();

        }

        public void Clear()
        {
            ViewState.ConfirmationCode = string.Empty;
        }

        public async Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                if (!await _gizmoClient.TokenIsValidAsync(TokenType.CreateAccount, _userRegistrationConfirmationMethodViewState.Token, ViewState.ConfirmationCode))
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString("CONFIRMATION_CODE_IS_INVALID");
                    //TODO: AAA CLEAR ERROR WITH TIMER OR SOMETHING?
                    return;
                }

                NavigationService.NavigateTo(ClientRoutes.RegistrationBasicFieldsRoute);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Check create account token validity error.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = ex.ToString();
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnCustomValidationAsync(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            await base.OnCustomValidationAsync(fieldIdentifier, validationMessageStore);

            if (fieldIdentifier.FieldName == nameof(ViewState.ConfirmationCode))
            {
                if (ViewState.ConfirmationCode.Length != _userRegistrationConfirmationMethodViewState.CodeLength)
                {
                    validationMessageStore.Add(() => ViewState.ConfirmationCode, _localizationService.GetString("GIZ_CONFIRMATION_CODE_LENGTH_ERROR", _userRegistrationConfirmationMethodViewState.CodeLength));
                }
                //else
                //{
                //    if (!await _gizmoClient.TokenIsValidAsync(TokenType.CreateAccount, _userRegistrationConfirmationMethodViewState.Token, ViewState.ConfirmationCode))
                //    {
                //        validationMessageStore.Add(() => ViewState.ConfirmationCode, _localizationService.GetString("CONFIRMATION_CODE_IS_INVALID"));
                //    }
                //}
            }
        }

        #endregion
    }
}
