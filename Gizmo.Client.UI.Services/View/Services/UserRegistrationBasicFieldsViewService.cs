using System.Text.RegularExpressions;
using Gizmo.Web.Api.Models;
using Gizmo.Client.Options;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
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
            IUserRegistrationService registrationService,
            IOptions<PasswordValidationOptions> passwordValidationOptions,
            UserRegistrationViewState userRegistrationViewState,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _registrationService = registrationService;
            _passwordValidationOptions = passwordValidationOptions;
            _userRegistrationViewState = userRegistrationViewState;
            _registrationSession = registrationSession;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IUserRegistrationService _registrationService;
        private readonly IOptions<PasswordValidationOptions> _passwordValidationOptions;
        private readonly UserRegistrationViewState _userRegistrationViewState;
        private readonly IRegistrationSessionService _registrationSession;
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

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ValidateProperty(() => ViewState.MobilePhone);
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
            ViewState.MobilePhone = null;

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

            ValidateProperty(() => ViewState.FirstName);
            ValidateProperty(() => ViewState.LastName);
            ValidateProperty(() => ViewState.BirthDate);
            ValidateProperty(() => ViewState.Sex);
            ValidateProperty(() => ViewState.Email);

            Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

                return;
            }

            var sessionEmail = _registrationSession.Flow == RegistrationFlow.Email
                ? _registrationSession.ActualContact
                : ViewState.Email;

            _registrationSession.SetProfileBasics(
                ViewState.Username, ViewState.Password,
                ViewState.FirstName, ViewState.LastName,
                ViewState.BirthDate, ViewState.Sex, sessionEmail);

            if (_registrationSession.Flow == RegistrationFlow.Email)
                _registrationSession.SetMobilePhone(ViewState.MobilePhone);

            ViewState.IsLoading = false;
            ViewState.RaiseChanged();

            NavigationService.NavigateTo(ClientRoutes.RegistrationAdditionalFieldsRoute);
        }

        private void CheckPasswordRules(string password)
        {
            ViewState.PasswordTooltip.PassedRules = 0;
            ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_MESSAGE_TOO_SHORT));

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
                        ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_MESSAGE_TOO_LONG));
                    }
                    else
                    {
                        ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_MESSAGE_TOO_SHORT));
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
                    ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_MESSAGE_TOO_EASY));
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
                    ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_MESSAGE_TOO_EASY));
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
                    ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_MESSAGE_TOO_EASY));
                }
            }

            if (ViewState.PasswordTooltip.PassedRules == ViewState.PasswordTooltip.TotalRules)
            {
                ViewState.PasswordTooltip.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_MESSAGE_SECURE));
            }
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_registrationSession.Token) && _registrationSession.Flow != RegistrationFlow.None)
            {
                NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

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
                    AddError(() => ViewState.Username, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_WHITE_SPACE_NOT_ALLOWED)));
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

                if(validationTrigger == ValidationTrigger.Input)
                {
                    //only clear reeat password errors on input
                    ClearError(() => ViewState.RepeatPassword);
                    if (!string.IsNullOrEmpty(ViewState.Password) && !string.IsNullOrEmpty(ViewState.RepeatPassword) && string.Compare(ViewState.Password, ViewState.RepeatPassword) != 0)
                    {
                        AddError(() => ViewState.RepeatPassword, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_PASSWORDS_DO_NOT_MATCH)));
                    }
                }               
            }


            if (fieldIdentifier.FieldEquals(() => ViewState.FirstName))
            {
                if (_userRegistrationViewState.DefaultUserGroupRequiredInfo?.FirstName == true && string.IsNullOrEmpty(ViewState.FirstName))
                {
                    AddError(() => ViewState.FirstName, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.LastName))
            {
                if (_userRegistrationViewState.DefaultUserGroupRequiredInfo?.LastName == true && string.IsNullOrEmpty(ViewState.LastName))
                {
                    AddError(() => ViewState.LastName, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.BirthDate))
            {
                if (_userRegistrationViewState.DefaultUserGroupRequiredInfo?.BirthDate == true && !ViewState.BirthDate.HasValue)
                {
                    AddError(() => ViewState.BirthDate, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Sex))
            {
                if (_userRegistrationViewState.DefaultUserGroupRequiredInfo?.Sex == true && ViewState.Sex == Sex.Unspecified)
                {
                    AddError(() => ViewState.Sex, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Email))
            {
                if (_registrationSession.Flow != RegistrationFlow.Email)
                {
                    if (_userRegistrationViewState.DefaultUserGroupRequiredInfo?.Email == true && string.IsNullOrEmpty(ViewState.Email))
                    {
                        AddError(() => ViewState.Email, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                    }
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
                        if (await _registrationService.ExistsAsync(ViewState.Username, cancellationToken))
                        {
                            return new string[] { _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_USERNAME_IN_USE)) };
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Cannot validate username.");
                        return new string[] { _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_CANNOT_VALIDATE_USERNAME)) };
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
