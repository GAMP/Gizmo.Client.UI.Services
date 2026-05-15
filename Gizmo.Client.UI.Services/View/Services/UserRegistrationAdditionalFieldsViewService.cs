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
    [Register()]
    [Route(ClientRoutes.RegistrationAdditionalFieldsRoute)]
    public sealed class UserRegistrationAdditionalFieldsViewService : ValidatingViewStateServiceBase<UserRegistrationAdditionalFieldsViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationAdditionalFieldsViewService(UserRegistrationAdditionalFieldsViewState viewState,
            ILogger<UserRegistrationAdditionalFieldsViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IUserRegistrationService registrationService,
            UserRegistrationViewState userRegistrationViewState) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _registrationService = registrationService;
            _userRegistrationViewState = userRegistrationViewState;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IUserRegistrationService _registrationService;
        private readonly UserRegistrationViewState _userRegistrationViewState;
        #endregion

        #region FUNCTIONS

        public void SetAddress(string value)
        {
            ViewState.Address = value;
            ValidateProperty(() => ViewState.Address);
        }

        public void SetPostCode(string value)
        {
            ViewState.PostCode = value;
            ValidateProperty(() => ViewState.PostCode);
        }

        public void SetCountry(string value)
        {
            ViewState.Country = value;
            ValidateProperty(() => ViewState.Country);
        }

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void Clear()
        {
            ViewState.Address = null;
            ViewState.PostCode = null;
            ViewState.Country = null;
            //ViewState.Prefix = null;
            ViewState.MobilePhone = null;

            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ResetValidationState();
            DebounceViewStateChanged();
        }

        public async Task SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            var userRegistrationIndexViewState = ServiceProvider.GetRequiredService<UserRegistrationIndexViewState>();

            var userRegistrationViewState = ServiceProvider.GetRequiredService<UserRegistrationViewState>();
            var userRegistrationConfirmationMethodViewState = ServiceProvider.GetRequiredService<UserRegistrationConfirmationMethodViewState>();
            var userRegistrationBasicFieldsViewState = ServiceProvider.GetRequiredService<UserRegistrationBasicFieldsViewState>();

            bool confirmationRequired = userRegistrationViewState.ConfirmationMethod != RegistrationVerificationMethod.None;

            try
            {
                string? country;
                string? mobilePhone;

                if (userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone)
                {
                    country = userRegistrationConfirmationMethodViewState.Country;
                    var rawPhone = userRegistrationConfirmationMethodViewState.MobilePhone;
                    mobilePhone = rawPhone?.StartsWith("+") == true ? rawPhone.Substring(1) : rawPhone;
                }
                else
                {
                    country = ViewState.Country;
                    var rawPhone = ViewState.MobilePhone;
                    mobilePhone = rawPhone?.StartsWith("+") == true ? rawPhone.Substring(1) : rawPhone;
                }

                var profile = new RegistrationProfile
                {
                    Username = userRegistrationBasicFieldsViewState.Username,
                    FirstName = userRegistrationBasicFieldsViewState.FirstName,
                    LastName = userRegistrationBasicFieldsViewState.LastName,
                    BirthDate = userRegistrationBasicFieldsViewState.BirthDate,
                    Sex = userRegistrationBasicFieldsViewState.Sex,
                    Email = userRegistrationBasicFieldsViewState.Email,
                    Address = ViewState.Address,
                    PostCode = ViewState.PostCode,
                    Country = country,
                    MobilePhone = mobilePhone
                };

                // TODO: agreements are collected in ViewState but not sent — new API does not accept them in registration request

                var result = await _registrationService.CompleteAsync(new RegistrationCompleteRequest
                {
                    Token = confirmationRequired ? userRegistrationConfirmationMethodViewState.Token : null,
                    Profile = profile,
                    Password = userRegistrationBasicFieldsViewState.Password
                });

                if (result != RegistrationCompleteCode.Success)
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_FAILED_MESSAGE));

                    return;
                }

                //TODO: AAA SUCCESS MESSAGE?
                NavigationService.NavigateTo(ClientRoutes.LoginRoute);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User create complete error.");

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

        #endregion

        #region OVERRIDES

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.Country))
            {
                if (_userRegistrationViewState.DefaultUserGroupRequiredInfo?.Country == true && string.IsNullOrEmpty(ViewState.Country))
                {
                    AddError(() => ViewState.Country, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Address))
            {
                if (_userRegistrationViewState.DefaultUserGroupRequiredInfo?.Address == true && string.IsNullOrEmpty(ViewState.Address))
                {
                    AddError(() => ViewState.Address, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.PostCode))
            {
                if (_userRegistrationViewState.DefaultUserGroupRequiredInfo?.PostCode == true && string.IsNullOrEmpty(ViewState.PostCode))
                {
                    AddError(() => ViewState.PostCode, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                }
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.MobilePhone))
            {
                if (_userRegistrationViewState.ConfirmationMethod != RegistrationVerificationMethod.MobilePhone)
                {
                    if (_userRegistrationViewState.DefaultUserGroupRequiredInfo?.Mobile == true && string.IsNullOrEmpty(ViewState.MobilePhone))
                    {
                        AddError(() => ViewState.MobilePhone, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                    }
                }
            }
        }

        #endregion

    }
}
