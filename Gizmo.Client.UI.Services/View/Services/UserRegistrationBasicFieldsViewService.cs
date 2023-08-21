using System.Text.RegularExpressions;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.RegistrationBasicFieldsRoute)]
    public sealed class UserRegistrationBasicFieldsViewService : ValidatingViewStateServiceBase<UserRegistrationBasicFieldsViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationBasicFieldsViewService(UserRegistrationBasicFieldsViewState viewState,
            ILogger<UserRegistrationBasicFieldsViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            IOptions<PasswordValidationOptions> passwordValidationOptions) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _passwordValidationOptions = passwordValidationOptions;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly IOptions<PasswordValidationOptions> _passwordValidationOptions;
        #endregion

        #region FUNCTIONS

        public void SetUsername(string value)
        {
            ViewState.Username = value;
            ValidateProperty(() => ViewState.Username);
        }

        public void SetPassword(string value)
        {
            ViewState.Password = value;
            CheckPasswordRules(ViewState.Password);
            ValidateProperty(() => ViewState.Password);
        }

        public void SetRepeatPassword(string value)
        {
            ViewState.RepeatPassword = value;
            ValidateProperty(() => ViewState.RepeatPassword);
        }

        public void SetFirstName(string value)
        {
            ViewState.FirstName = value;
            ValidateProperty(() => ViewState.FirstName);
        }

        public void SetLastName(string value)
        {
            ViewState.LastName = value;
            ValidateProperty(() => ViewState.LastName);
        }

        public void SetBirthDate(DateTime? value)
        {
            ViewState.BirthDate = value;
            ValidateProperty(() => ViewState.BirthDate);
        }

        public void SetSex(Sex value)
        {
            ViewState.Sex = value;
            ValidateProperty(() => ViewState.Sex);
        }

        public void SetEmail(string value)
        {
            ViewState.Email = value;
            ValidateProperty(() => ViewState.Email);
        }

        public void Clear()
        {
            ViewState.Username = string.Empty;
            ViewState.Password = string.Empty;
            ViewState.RepeatPassword = string.Empty;
            ViewState.FirstName = null;
            ViewState.LastName = null;
            ViewState.BirthDate = null;
            ViewState.Sex = Sex.Unspecified;
            ViewState.Email = null;

            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ResetValidationState();
            DebounceViewStateChanged();
        }

        public async Task SubmitAsync()
        {
            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

                return;
            }

            var userRegistrationIndexViewState = ServiceProvider.GetRequiredService<UserRegistrationIndexViewState>();

            var userRegistrationViewState = ServiceProvider.GetRequiredService<UserRegistrationViewState>();
            var userRegistrationConfirmationMethodViewState = ServiceProvider.GetRequiredService<UserRegistrationConfirmationMethodViewState>();

            bool confirmationRequired = userRegistrationViewState.ConfirmationMethod != RegistrationVerificationMethod.None;
            bool confirmationWithMobilePhone = userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone; //TODO: A If both methods are available then get user selection.

            if (userRegistrationViewState.DefaultUserGroupRequiredInfo.Country ||
                userRegistrationViewState.DefaultUserGroupRequiredInfo.Address ||
                userRegistrationViewState.DefaultUserGroupRequiredInfo.PostCode ||
                (userRegistrationViewState.DefaultUserGroupRequiredInfo.Mobile && !confirmationWithMobilePhone))
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

                //If any of the additional fields is required open the next page.
                NavigationService.NavigateTo(ClientRoutes.RegistrationAdditionalFieldsRoute);
            }
            else
            {
                //If no additional fields are required then proceed with sign up.

                try
                {
                    var profile = new Web.Api.Models.UserProfileModelCreate()
                    {
                        Username = ViewState.Username,
                        FirstName = ViewState.FirstName,
                        LastName = ViewState.LastName,
                        BirthDate = ViewState.BirthDate,
                        Sex = ViewState.Sex
                    };

                    if (userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.Email)
                    {
                        profile.Email = userRegistrationConfirmationMethodViewState.Email;
                    }
                    else
                    {
                        profile.Email = ViewState.Email;
                    }

                    var userAgreements = userRegistrationIndexViewState.UserAgreementStates.Select(a => new UserAgreementModelState()
                    {
                        UserAgreementId = a.Id,
                        AcceptState = a.AcceptState
                    }).ToList();

                    if (!confirmationRequired)
                    {
                        var result = await _gizmoClient.UserCreateCompleteAsync(profile, ViewState.Password, userAgreements);

                        if (result.Result != AccountCreationCompleteResultCode.Success)
                        {
                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("GIZ_REGISTRATION_FAILED_MESSAGE");

                            return;
                        }
                    }
                    else
                    {
                        var result = await _gizmoClient.UserCreateByTokenCompleteAsync(userRegistrationConfirmationMethodViewState.Token, profile, ViewState.Password, userAgreements);

                        if (result.Result != AccountCreationByTokenCompleteResultCode.Success)
                        {
                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("GIZ_REGISTRATION_FAILED_MESSAGE");

                            return;
                        }
                    }

                    NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "User create complete error.");

                    ViewState.HasError = true;
                    ViewState.ErrorMessage = ex.ToString(); //TODO: AAA TRANSLATE
                }
                finally
                {
                    ViewState.IsLoading = false;
                    ViewState.RaiseChanged();
                }
            }
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
                if ((!ViewState.PasswordTooltip.MinimumLengthRule.HasValue || password.Length >= ViewState.PasswordTooltip.MinimumLengthRule) &&
                    (!ViewState.PasswordTooltip.MaximumLengthRule.HasValue || password.Length <= ViewState.PasswordTooltip.MaximumLengthRule))
                {
                    ViewState.PasswordTooltip.PassedRules += 1;
                    ViewState.PasswordTooltip.LengthRulePassed = true;
                }
                else
                {
                    if (password.Length >= ViewState.PasswordTooltip.MaximumLengthRule)
                    {
                        ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_MESSAGE_TOO_LONG");
                    }
                    else
                    {
                        ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString("GIZ_PASSWORD_MESSAGE_TOO_SHORT");
                    }
                }
            }

            Regex lowerRulePassedRegex = new Regex("\\p{Ll}");
            Regex upperRuleRegex = new Regex("\\p{Lu}");
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

        #region OVERRIDES

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

            CheckPasswordRules(ViewState.Password);

            ViewState.PasswordTooltip.RaiseChanged();

            return base.OnInitializing(ct);
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.Username))
            {
                if (!string.IsNullOrEmpty(ViewState.Username) && ViewState.Username.ToString().Any(c => char.IsWhiteSpace(c)))
                {
                    ClearError(() => ViewState.Username);
                    AddError(() => ViewState.Username, _localizationService.GetString("GIZ_REGISTRATION_VE_WHITE_SPACE_NOT_ALLOWED"));
                }
            }
            else if (fieldIdentifier.FieldEquals(() => ViewState.Password) || fieldIdentifier.FieldEquals(() => ViewState.RepeatPassword))
            {
                if (fieldIdentifier.FieldEquals(() => ViewState.Password))
                {
                    if (ViewState.PasswordTooltip.PassedRules < ViewState.PasswordTooltip.TotalRules)
                    {
                        AddError(() => ViewState.Password, ViewState.PasswordTooltip.ErrorMessage);
                    }
                }

                ClearError(() => ViewState.RepeatPassword);
                if (!string.IsNullOrEmpty(ViewState.Password) && !string.IsNullOrEmpty(ViewState.RepeatPassword) && string.Compare(ViewState.Password, ViewState.RepeatPassword) != 0)
                {
                    AddError(() => ViewState.RepeatPassword, _localizationService.GetString("GIZ_GEN_PASSWORDS_DO_NOT_MATCH"));
                }
            }
        }

        protected override async Task<IEnumerable<string>> OnValidateAsync(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger, CancellationToken cancellationToken = default)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.Username))
            {
                if (!string.IsNullOrEmpty(ViewState.Username))
                {
                    try
                    {
                        if (await _gizmoClient.UserExistAsync(ViewState.Username))
                        {
                            return new string[] { _localizationService.GetString("GIZ_REGISTRATION_VE_USERNAME_IN_USE") };
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Cannot validate username.");
                        return new string[] { _localizationService.GetString("GIZ_REGISTRATION_VE_CANNOT_VALIDATE_USERNAME") };
                    }
                }
            }

            return await base.OnValidateAsync(fieldIdentifier, validationTrigger, cancellationToken);
        }

        #endregion

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
        }
    }
}
