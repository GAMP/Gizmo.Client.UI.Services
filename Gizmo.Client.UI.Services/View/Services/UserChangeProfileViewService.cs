using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserChangeProfileViewService : ValidatingViewStateServiceBase<UserChangeProfileViewState>
    {
        #region CONSTRUCTOR
        public UserChangeProfileViewService(UserChangeProfileViewState viewState,
            ILogger<UserChangeProfileViewService> logger,
            IServiceProvider serviceProvider,
            IClientDialogService dialogService,
            IGizmoClient gizmoClient,
            IUserProfileService userProfileService,
            IPhoneValidationService phoneValidationService,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _dialogService = dialogService;
            _gizmoClient = gizmoClient;
            _userProfileService = userProfileService;
            _phoneValidationService = phoneValidationService;
            _localizationService = localizationService;
        }
        #endregion

        #region FIELDS
        private const int MIN_NATIONAL_PHONE_LENGTH = 7;

        private readonly IClientDialogService _dialogService;
        private readonly IGizmoClient _gizmoClient;
        private readonly IUserProfileService _userProfileService;
        private readonly IPhoneValidationService _phoneValidationService;
        private readonly ILocalizationService _localizationService;

        private UserProfileModel? _profile;
        private string? _mobileE164;
        #endregion

        #region FUNCTIONS

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

        public void SetMobilePhone(string? value)
        {
            ViewState.MobilePhone = value;
            _mobileE164 = null;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void SetPhoneRegionCode(string? value)
        {
            ViewState.PhoneRegionCode = value;
            _mobileE164 = null;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void SetPhone(string value)
        {
            ViewState.Phone = value;
            ValidateProperty(() => ViewState.Phone);
        }

        public void SetAddress(string value)
        {
            ViewState.Address = value;
            ValidateProperty(() => ViewState.Address);
        }

        public void SetCity(string value)
        {
            ViewState.City = value;
            ValidateProperty(() => ViewState.City);
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

        public Task StartAsync(CancellationToken cToken = default)
        {
            return StartCoreAsync(true, cToken);
        }

        public Task StartIfIncompleteAsync(CancellationToken cToken = default)
        {
            return StartCoreAsync(false, cToken);
        }

        private async Task StartCoreAsync(bool isPersonalInfoRequested, CancellationToken cToken)
        {
            try
            {
                await ResetAsync();

                UserModelRequiredInfo? required;
                UserProfileModel profile;

                try
                {
                    required = await _gizmoClient.UserGroupCurrentRequiredInfoGetAsync(cToken);
                    profile = await _gizmoClient.UserProfileGetAsync(cToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    Logger.LogError(ex, "Failed to load required info or profile.");
                    return;
                }

                if (required == null)
                    return;

                _profile = profile;

                ViewState.ShowFirstName = IsShown(isPersonalInfoRequested, required.FirstName, string.IsNullOrEmpty(profile.FirstName));
                ViewState.ShowLastName = IsShown(isPersonalInfoRequested, required.LastName, string.IsNullOrEmpty(profile.LastName));
                ViewState.ShowBirthDate = IsShown(isPersonalInfoRequested, required.BirthDate, profile.BirthDate == null);
                ViewState.ShowSex = IsShown(isPersonalInfoRequested, required.Sex, profile.Sex == Sex.Unspecified);
                ViewState.ShowEmail = IsShown(isPersonalInfoRequested, required.Email, string.IsNullOrEmpty(profile.Email));
                ViewState.ShowMobilePhone = IsShown(isPersonalInfoRequested, required.Mobile, string.IsNullOrEmpty(profile.MobilePhone));
                ViewState.ShowPhone = IsShown(isPersonalInfoRequested, required.Phone, string.IsNullOrEmpty(profile.Phone));
                ViewState.ShowAddress = IsShown(isPersonalInfoRequested, required.Address, string.IsNullOrEmpty(profile.Address));
                ViewState.ShowCity = IsShown(isPersonalInfoRequested, required.City, string.IsNullOrEmpty(profile.City));
                ViewState.ShowPostCode = IsShown(isPersonalInfoRequested, required.PostCode, string.IsNullOrEmpty(profile.PostCode));
                ViewState.ShowCountry = IsShown(isPersonalInfoRequested, required.Country, string.IsNullOrEmpty(profile.Country));

                if (!ViewState.ShowFirstName && !ViewState.ShowLastName && !ViewState.ShowBirthDate && !ViewState.ShowSex &&
                    !ViewState.ShowEmail && !ViewState.ShowMobilePhone && !ViewState.ShowPhone && !ViewState.ShowAddress &&
                    !ViewState.ShowCity && !ViewState.ShowPostCode && !ViewState.ShowCountry)
                {
                    return;
                }

                ViewState.FirstName = ViewState.ShowFirstName ? profile.FirstName : null;
                ViewState.LastName = ViewState.ShowLastName ? profile.LastName : null;
                ViewState.BirthDate = ViewState.ShowBirthDate ? profile.BirthDate : null;
                ViewState.Sex = ViewState.ShowSex ? profile.Sex : Sex.Unspecified;
                ViewState.Email = ViewState.ShowEmail ? profile.Email : null;
                ViewState.MobilePhone = ViewState.ShowMobilePhone ? profile.MobilePhone?.TrimStart('+') : null;
                ViewState.Phone = ViewState.ShowPhone ? profile.Phone : null;
                ViewState.Address = ViewState.ShowAddress ? profile.Address : null;
                ViewState.City = ViewState.ShowCity ? profile.City : null;
                ViewState.PostCode = ViewState.ShowPostCode ? profile.PostCode : null;
                ViewState.Country = ViewState.ShowCountry ? profile.Country : null;

                if (ViewState.ShowMobilePhone && !string.IsNullOrEmpty(ViewState.MobilePhone))
                    ViewState.PhoneRegionCode = await ResolvePhoneRegionCodeAsync(ViewState.MobilePhone, cToken);

                ValidateShownProperties();

                ViewState.RaiseChanged();

                var dialogResult = await _dialogService.ShowChangeProfileDialogAsync();
                if (dialogResult.Result == AddComponentResultCode.Opened)
                    _ = await dialogResult.WaitForResultAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to complete user profile.");
            }
        }

        public async Task SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            if (ViewState.ShowMobilePhone && string.IsNullOrEmpty(_mobileE164))
            {
                ValidateProperty(() => ViewState.MobilePhone);
                return;
            }

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            if (!await ValidateAvailabilityAsync())
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
                return;
            }

            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            try
            {
                var result = await _userProfileService.UpdateAsync(new UserProfileUpdate()
                {
                    FirstName = ViewState.ShowFirstName ? ViewState.FirstName ?? string.Empty : null,
                    LastName = ViewState.ShowLastName ? ViewState.LastName ?? string.Empty : null,
                    BirthDate = ViewState.ShowBirthDate ? ViewState.BirthDate : null,
                    Sex = ViewState.ShowSex ? UserProfileSexMapper.FromModel(ViewState.Sex) : null,
                    Email = ViewState.ShowEmail ? ViewState.Email ?? string.Empty : null,
                    MobilePhone = ViewState.ShowMobilePhone ? _mobileE164 : null,
                    Phone = ViewState.ShowPhone ? ViewState.Phone ?? string.Empty : null,
                    Address = ViewState.ShowAddress ? ViewState.Address ?? string.Empty : null,
                    City = ViewState.ShowCity ? ViewState.City ?? string.Empty : null,
                    PostCode = ViewState.ShowPostCode ? ViewState.PostCode ?? string.Empty : null,
                    Country = ViewState.ShowCountry ? ViewState.Country ?? string.Empty : null,
                });

                switch (result)
                {
                    case UserProfileUpdateCode.Success:
                        ViewState.IsComplete = true;
                        break;
                    case UserProfileUpdateCode.NonUniqueValue:
                        if (ViewState.ShowEmail)
                            ValidateProperty(() => ViewState.Email);
                        if (ViewState.ShowMobilePhone)
                            ValidateProperty(() => ViewState.MobilePhone);
                        SetGenericError();
                        break;
                    default:
                        SetGenericError();
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User profile update error.");
                SetGenericError();
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public Task ResetAsync()
        {
            _profile = null;
            _mobileE164 = null;

            ViewState.FirstName = null;
            ViewState.LastName = null;
            ViewState.BirthDate = null;
            ViewState.Sex = Sex.Unspecified;
            ViewState.Email = null;
            ViewState.MobilePhone = null;
            ViewState.PhoneRegionCode = null;
            ViewState.Phone = null;
            ViewState.Address = null;
            ViewState.City = null;
            ViewState.PostCode = null;
            ViewState.Country = null;

            ViewState.ShowFirstName = false;
            ViewState.ShowLastName = false;
            ViewState.ShowBirthDate = false;
            ViewState.ShowSex = false;
            ViewState.ShowEmail = false;
            ViewState.ShowMobilePhone = false;
            ViewState.ShowPhone = false;
            ViewState.ShowAddress = false;
            ViewState.ShowCity = false;
            ViewState.ShowPostCode = false;
            ViewState.ShowCountry = false;

            ViewState.IsComplete = false;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ResetValidationState();

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        private static bool IsShown(bool isPersonalInfoRequested, bool isRequired, bool isMissing)
        {
            return isRequired && (isPersonalInfoRequested || isMissing);
        }

        private void ValidateShownProperties()
        {
            if (ViewState.ShowFirstName) ValidateProperty(() => ViewState.FirstName);
            if (ViewState.ShowLastName) ValidateProperty(() => ViewState.LastName);
            if (ViewState.ShowBirthDate) ValidateProperty(() => ViewState.BirthDate);
            if (ViewState.ShowSex) ValidateProperty(() => ViewState.Sex);
            if (ViewState.ShowEmail) ValidateProperty(() => ViewState.Email);
            if (ViewState.ShowMobilePhone) ValidateProperty(() => ViewState.MobilePhone);
            if (ViewState.ShowPhone) ValidateProperty(() => ViewState.Phone);
            if (ViewState.ShowAddress) ValidateProperty(() => ViewState.Address);
            if (ViewState.ShowCity) ValidateProperty(() => ViewState.City);
            if (ViewState.ShowPostCode) ValidateProperty(() => ViewState.PostCode);
            if (ViewState.ShowCountry) ValidateProperty(() => ViewState.Country);
        }

        private async Task<string?> ResolvePhoneRegionCodeAsync(string phoneDigits, CancellationToken cancellationToken)
        {
            var countries = await _phoneValidationService.GetCountriesAsync(cancellationToken);

            var match = countries
                .Where(country => phoneDigits.StartsWith(country.CallingCode.TrimStart('+')))
                .OrderByDescending(country => country.CallingCode.Length)
                .FirstOrDefault();

            //Russia and Kazakhstan share the +7 calling code, Kazakhstan numbers start with 6 or 7 after it
            if (match?.CallingCode.TrimStart('+') == "7" && phoneDigits.Length > 1 && (phoneDigits[1] == '6' || phoneDigits[1] == '7'))
                return countries.FirstOrDefault(country => country.RegionCode == "KZ")?.RegionCode ?? match.RegionCode;

            return match?.RegionCode;
        }

        private static bool IsSamePhone(string e164, string? profilePhone)
        {
            var current = new string(e164.Where(char.IsDigit).ToArray());
            var original = new string((profilePhone ?? string.Empty).Where(char.IsDigit).ToArray()).TrimStart('0');

            //a suffix shorter than a national number would match unrelated numbers
            return original.Length >= MIN_NATIONAL_PHONE_LENGTH && current.EndsWith(original);
        }

        private bool IsEmailChecked => ViewState.ShowEmail
            && !string.IsNullOrEmpty(ViewState.Email)
            && !string.Equals(ViewState.Email, _profile?.Email, StringComparison.OrdinalIgnoreCase);

        private async Task<bool> ValidateAvailabilityAsync()
        {
            var isAvailable = true;

            if (IsEmailChecked)
            {
                var emailError = await GetEmailAvailabilityErrorAsync(default);
                if (emailError != null)
                {
                    AddError(() => ViewState.Email, emailError);
                    isAvailable = false;
                }
            }

            if (ViewState.ShowMobilePhone && !string.IsNullOrEmpty(_mobileE164))
            {
                var mobileError = await GetMobileAvailabilityErrorAsync(_mobileE164, default);
                if (mobileError != null)
                {
                    AddError(() => ViewState.MobilePhone, mobileError);
                    isAvailable = false;
                }
            }

            return isAvailable;
        }

        private async Task<string?> GetEmailAvailabilityErrorAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (await _userProfileService.EmailExistAsync(ViewState.Email!, cancellationToken))
                    return _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_EMAIL_ADDRESS_USED));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogError(ex, "Cannot validate email.");
                return _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_CANNOT_VALIDATE_EMAIL));
            }

            return null;
        }

        private async Task<string?> GetMobileAvailabilityErrorAsync(string e164, CancellationToken cancellationToken)
        {
            //the own number of the user is not a conflict, legacy profiles may store it in a non E164 form
            if (IsSamePhone(e164, _profile?.MobilePhone))
                return null;

            try
            {
                if (await _userProfileService.MobilePhoneExistAsync(e164.TrimStart('+'), cancellationToken))
                    return _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_MOBILE_PHONE_USED));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogError(ex, "Cannot validate phone.");
                return _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_CANNOT_VALIDATE_PHONE));
            }

            return null;
        }

        private void SetGenericError()
        {
            ViewState.HasError = true;
            ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
        }

        private void AddRequiredError<T>(System.Linq.Expressions.Expression<Func<T>> accessor)
        {
            AddError(accessor, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
        }

        #endregion

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.FirstName))
            {
                if (ViewState.ShowFirstName && string.IsNullOrEmpty(ViewState.FirstName))
                    AddRequiredError(() => ViewState.FirstName);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.LastName))
            {
                if (ViewState.ShowLastName && string.IsNullOrEmpty(ViewState.LastName))
                    AddRequiredError(() => ViewState.LastName);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.BirthDate))
            {
                if (ViewState.ShowBirthDate && !ViewState.BirthDate.HasValue)
                    AddRequiredError(() => ViewState.BirthDate);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Sex))
            {
                if (ViewState.ShowSex && ViewState.Sex == Sex.Unspecified)
                    AddRequiredError(() => ViewState.Sex);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Email))
            {
                if (ViewState.ShowEmail && string.IsNullOrEmpty(ViewState.Email))
                    AddRequiredError(() => ViewState.Email);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.MobilePhone))
            {
                if (ViewState.ShowMobilePhone && string.IsNullOrEmpty(ViewState.MobilePhone))
                    AddRequiredError(() => ViewState.MobilePhone);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Phone))
            {
                if (ViewState.ShowPhone && string.IsNullOrEmpty(ViewState.Phone))
                    AddRequiredError(() => ViewState.Phone);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Address))
            {
                if (ViewState.ShowAddress && string.IsNullOrEmpty(ViewState.Address))
                    AddRequiredError(() => ViewState.Address);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.City))
            {
                if (ViewState.ShowCity && string.IsNullOrEmpty(ViewState.City))
                    AddRequiredError(() => ViewState.City);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.PostCode))
            {
                if (ViewState.ShowPostCode && string.IsNullOrEmpty(ViewState.PostCode))
                    AddRequiredError(() => ViewState.PostCode);
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.Country))
            {
                if (ViewState.ShowCountry && string.IsNullOrEmpty(ViewState.Country))
                    AddRequiredError(() => ViewState.Country);
            }
        }

        protected override async Task<IEnumerable<string>> OnValidateAsync(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger, CancellationToken cancellationToken = default)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.Email) && IsEmailChecked)
            {
                var emailError = await GetEmailAvailabilityErrorAsync(cancellationToken);
                if (emailError != null)
                    return new string[] { emailError };
            }

            if (fieldIdentifier.FieldEquals(() => ViewState.MobilePhone)
                && ViewState.ShowMobilePhone
                && !string.IsNullOrEmpty(ViewState.MobilePhone))
            {
                if (string.IsNullOrEmpty(ViewState.PhoneRegionCode))
                    return new string[] { _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_SELECT_COUNTRY)) };

                var result = await _phoneValidationService.ValidateAsync(ViewState.MobilePhone, ViewState.PhoneRegionCode, cancellationToken);
                if (!result.IsValid)
                    return new string[] { _localizationService.GetString(result.ErrorKey ?? nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED)) };

                if (string.IsNullOrWhiteSpace(result.E164))
                    return new string[] { _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_CANNOT_VALIDATE_PHONE)) };

                cancellationToken.ThrowIfCancellationRequested();

                _mobileE164 = result.E164;

                var mobileError = await GetMobileAvailabilityErrorAsync(result.E164, cancellationToken);
                if (mobileError != null)
                    return new string[] { mobileError };
            }

            return await base.OnValidateAsync(fieldIdentifier, validationTrigger, cancellationToken);
        }

        protected override AsyncValidatedDetermineResult OnDetermineIsAsyncPropertiesValidated()
        {
            if (ViewState.ShowMobilePhone
                && !string.IsNullOrEmpty(ViewState.MobilePhone)
                && !IsAsyncValidated(() => ViewState.MobilePhone))
            {
                return base.OnDetermineIsAsyncPropertiesValidated();
            }

            if (ViewState.ShowEmail
                && !string.IsNullOrEmpty(ViewState.Email)
                && !IsAsyncValidated(() => ViewState.Email))
            {
                return base.OnDetermineIsAsyncPropertiesValidated();
            }

            return AsyncValidatedDetermineResult.DefaultTrue;
        }
    }
}
