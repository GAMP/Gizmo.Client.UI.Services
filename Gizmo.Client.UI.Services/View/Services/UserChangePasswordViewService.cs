using System.Text.RegularExpressions;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserChangePasswordViewService : ValidatingViewStateServiceBase<UserChangePasswordViewState>
    {
        #region CONSTRUCTOR
        public UserChangePasswordViewService(UserChangePasswordViewState viewState,
            ILogger<UserChangePasswordViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IClientDialogService dialogService,
            IGizmoClient gizmoClient,
            IOptions<PasswordValidationOptions> passwordValidationOptions) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _dialogService = dialogService;
            _gizmoClient = gizmoClient;
            _passwordValidationOptions = passwordValidationOptions;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IClientDialogService _dialogService;
        private readonly IGizmoClient _gizmoClient;
        private readonly IOptions<PasswordValidationOptions> _passwordValidationOptions;
        #endregion

        #region FUNCTIONS

        public void SetOldPassword(string value)
        {
            ViewState.OldPassword = value;
            ValidateProperty(() => ViewState.OldPassword);
        }

        public void SetNewPassword(string value)
        {
            ViewState.NewPassword = value;
            CheckPasswordRules(ViewState.NewPassword);
            ValidateProperty(() => ViewState.NewPassword);
        }

        public void SetRepeatPassword(string value)
        {
            ViewState.RepeatPassword = value;
            ValidateProperty(() => ViewState.RepeatPassword);
        }

        public async Task StartAsync(bool showOldPassword, CancellationToken cToken = default)
        {
            try
            {
                await ResetAsync();

                ViewState.ShowOldPassword = showOldPassword;

                var s = await _dialogService.ShowChangePasswordDialogAsync(cToken);
                if (s.Result == AddComponentResultCode.Opened)
                    _ = await s.WaitForResultAsync(cToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to update user password.");
            }
        }

        public async Task SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                await _gizmoClient.UserPasswordUpdateAsync(ViewState.OldPassword, ViewState.NewPassword);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User password update error.");

                ViewState.HasError = true;

                if (ex.Message == "WRONG_PASSWORD")
                {
                    ViewState.ErrorMessage = _localizationService.GetString("GIZ_CHANGE_PASSWORD_WRONG_PASSWORD_ERROR_MESSAGE");
                }
                else
                {
                    ViewState.ErrorMessage = _localizationService.GetString("GIZ_CHANGE_PASSWORD_ERROR_MESSAGE");
                }
            }
            finally
            {
                ViewState.IsComplete = true;
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public Task ResetAsync()
        {
            ViewState.ShowOldPassword = false;
            ViewState.OldPassword = string.Empty;
            ViewState.NewPassword = string.Empty;
            ViewState.RepeatPassword = string.Empty;

            ViewState.IsInitializing = false;
            ViewState.IsInitialized = null;
            ViewState.IsComplete = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ResetValidationState();

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        private void CheckPasswordRules(string password)
        {
            ViewState.PasswordTooltip.PassedRules = 0;
            ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_MESSAGE_TOO_SHORT");

            ViewState.PasswordTooltip.LengthRulePassed = false;
            ViewState.PasswordTooltip.LowerCaseCharactersRulePassed = false;
            ViewState.PasswordTooltip.UpperCaseCharactersRulePassed = false;
            ViewState.PasswordTooltip.NumbersRulePassed = false;

            if (string.IsNullOrEmpty(password))
                return;

            if (ViewState.PasswordTooltip.MinimumLengthRule > 0 || ViewState.PasswordTooltip.MaximumLengthRule > 0)
            {
                if (password.Length >= ViewState.PasswordTooltip.MinimumLengthRule && password.Length <= ViewState.PasswordTooltip.MaximumLengthRule)
                {
                    ViewState.PasswordTooltip.PassedRules += 1;
                    ViewState.PasswordTooltip.LengthRulePassed = true;
                }
                else
                {
                    if (password.Length >= ViewState.PasswordTooltip.MinimumLengthRule)
                    {
                        ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_MESSAGE_TOO_LONG");
                    }
                    else
                    {
                        ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_MESSAGE_TOO_SHORT");
                    }
                }
            }

            Regex lowerRulePassedRegex = new Regex("[A-Z]");
            Regex upperRuleRegex = new Regex("[a-z]");
            Regex numberRuleRegex = new Regex("[0-9]");

            if (ViewState.PasswordTooltip.HasLowerCaseCharactersRule)
            {
                if (lowerRulePassedRegex.Matches(password).Count > 0)
                {
                    ViewState.PasswordTooltip.PassedRules += 1;
                    ViewState.PasswordTooltip.LowerCaseCharactersRulePassed = true;
                }
                else
                {
                    ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_MESSAGE_TOO_EASY");
                }
            }

            if (ViewState.PasswordTooltip.HasUpperCaseCharactersRule)
            {
                if (upperRuleRegex.Matches(password).Count > 0)
                {
                    ViewState.PasswordTooltip.PassedRules += 1;
                    ViewState.PasswordTooltip.UpperCaseCharactersRulePassed = true;
                }
                else
                {
                    ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_MESSAGE_TOO_EASY");
                }
            }

            if (ViewState.PasswordTooltip.HasNumbersRule)
            {
                if (numberRuleRegex.Matches(password).Count > 0)
                {
                    ViewState.PasswordTooltip.PassedRules += 1;
                    ViewState.PasswordTooltip.NumbersRulePassed = true;
                }
                else
                {
                    ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_MESSAGE_TOO_EASY");
                }
            }

            if (ViewState.PasswordTooltip.PassedRules == ViewState.PasswordTooltip.TotalRules)
            {
                ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_MESSAGE_SECURE");
            }
        }

        #endregion

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.PasswordTooltip.MinimumLengthRule = _passwordValidationOptions.Value.MinimumLength;
            ViewState.PasswordTooltip.MaximumLengthRule = _passwordValidationOptions.Value.MaximumLength;
            ViewState.PasswordTooltip.HasLowerCaseCharactersRule = _passwordValidationOptions.Value.LowerCaseCharactersRequired;
            ViewState.PasswordTooltip.HasUpperCaseCharactersRule = _passwordValidationOptions.Value.UpperCaseCharactersRequired;
            ViewState.PasswordTooltip.HasNumbersRule = _passwordValidationOptions.Value.NumbersRequired;

            ViewState.PasswordTooltip.TotalRules = 0;

            if (ViewState.PasswordTooltip.MinimumLengthRule > 0 || ViewState.PasswordTooltip.MaximumLengthRule > 0)
                ViewState.PasswordTooltip.TotalRules += 1;

            if (ViewState.PasswordTooltip.HasLowerCaseCharactersRule)
                ViewState.PasswordTooltip.TotalRules += 1;

            if (ViewState.PasswordTooltip.HasUpperCaseCharactersRule)
                ViewState.PasswordTooltip.TotalRules += 1;

            if (ViewState.PasswordTooltip.HasNumbersRule)
                ViewState.PasswordTooltip.TotalRules += 1;

            CheckPasswordRules(ViewState.NewPassword);

            ViewState.PasswordTooltip.RaiseChanged();

            return base.OnInitializing(ct);
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (ViewState.ShowOldPassword && fieldIdentifier.FieldEquals(() => ViewState.OldPassword) && string.IsNullOrEmpty(ViewState.OldPassword))
            {
                AddError(() => ViewState.OldPassword, _localizationService.GetString("GIZ_USER_CHANGE_PASSWORD_VE_OLD_PASSWORD_IS_REQUIRED"));
            }
            if (fieldIdentifier.FieldEquals(() => ViewState.NewPassword) || fieldIdentifier.FieldEquals(() => ViewState.RepeatPassword))
            {
                if (fieldIdentifier.FieldEquals(() => ViewState.NewPassword))
                {
                    if (ViewState.PasswordTooltip.PassedRules < ViewState.PasswordTooltip.TotalRules)
                    {
                        AddError(() => ViewState.NewPassword, ViewState.PasswordTooltip.ErrorMessage);
                    }
                }

                ClearError(() => ViewState.RepeatPassword);
                if (!string.IsNullOrEmpty(ViewState.NewPassword) && !string.IsNullOrEmpty(ViewState.RepeatPassword) && string.Compare(ViewState.NewPassword, ViewState.RepeatPassword) != 0)
                {
                    AddError(() => ViewState.RepeatPassword, _localizationService.GetString("GIZ_GEN_PASSWORDS_DO_NOT_MATCH"));
                }
            }
        }
    }
}
