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
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _dialogService = dialogService;
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
        }
        #endregion

        #region FIELDS
        private readonly IClientDialogService _dialogService;
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
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

        public void SetCountry(string value)
        {
            ViewState.Country = value;
            ValidateProperty(() => ViewState.Country);
        }

        public async Task StartAsync(CancellationToken cToken = default)
        {
            try
            {
                await ResetAsync();

                ViewState.IsInitializing = true;
                ViewState.IsInitialized = false;

                var s = await _dialogService.ShowChangeProfileDialogAsync();
                if (s.Result == AddComponentResultCode.Opened)
                {
                    //_ = await s.WaitForDialogResultAsync();
                }

                try
                {
                    var profile = await _gizmoClient.UserProfileGetAsync(cToken);

                    ViewState.Username = profile.Username;
                    ViewState.FirstName = profile.FirstName;
                    ViewState.LastName = profile.LastName;
                    ViewState.BirthDate = profile.BirthDate;
                    ViewState.Sex = profile.Sex;
                    ViewState.Country = profile.Country;

                    ViewState.IsInitialized = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "User profile get error.");

                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED");

                    ViewState.IsComplete = true;

                    throw;
                }
                finally
                {
                    ViewState.IsInitializing = false;

                    ViewState.RaiseChanged();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to update user profile.");
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
                await _gizmoClient.UserProfileUpdateAsync(new UserProfileModelUpdate()
                {
                    FirstName = ViewState.FirstName,
                    LastName = ViewState.LastName,
                    BirthDate = ViewState.BirthDate,
                    Sex = ViewState.Sex,
                    Country = ViewState.Country
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User profile update error.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED");
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
            ViewState.Username = string.Empty;
            ViewState.FirstName = string.Empty;
            ViewState.LastName = string.Empty;
            ViewState.BirthDate = null;
            ViewState.Sex = Sex.Unspecified;
            ViewState.Country = string.Empty;
            ViewState.Picture = string.Empty;

            ViewState.IsInitializing = false;
            ViewState.IsInitialized = null;
            ViewState.IsComplete = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ResetValidationState();

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion

        //protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        //{
        //    if (ViewState.RequiredUserInformation?.HasFlag(UserInfoTypes.FirstName) == true)
        //    {
        //        if (fieldIdentifier.FieldEquals(() => ViewState.FirstName) && string.IsNullOrEmpty(ViewState.FirstName))
        //        {
        //            AddError(() => ViewState.FirstName, _localizationService.GetString("GIZ_GEN_VE_REQUIRED_FIELD"));
        //        }
        //    }

        //    if (ViewState.RequiredUserInformation?.HasFlag(UserInfoTypes.LastName) == true)
        //    {
        //        if (fieldIdentifier.FieldEquals(() => ViewState.LastName) && string.IsNullOrEmpty(ViewState.LastName))
        //        {
        //            AddError(() => ViewState.LastName, _localizationService.GetString("GIZ_GEN_VE_REQUIRED_FIELD"));
        //        }
        //    }

        //    if (ViewState.RequiredUserInformation?.HasFlag(UserInfoTypes.BirthDate) == true)
        //    {
        //        if (fieldIdentifier.FieldEquals(() => ViewState.BirthDate) && !ViewState.BirthDate.HasValue)
        //        {
        //            AddError(() => ViewState.BirthDate, _localizationService.GetString("GIZ_GEN_VE_REQUIRED_FIELD"));
        //        }
        //    }

        //    if (ViewState.RequiredUserInformation?.HasFlag(UserInfoTypes.Country) == true)
        //    {
        //        if (fieldIdentifier.FieldEquals(() => ViewState.Country) && string.IsNullOrEmpty(ViewState.Country))
        //        {
        //            AddError(() => ViewState.Country, _localizationService.GetString("GIZ_GEN_VE_REQUIRED_FIELD"));
        //        }
        //    }

        //    if (ViewState.RequiredUserInformation?.HasFlag(UserInfoTypes.Sex) == true)
        //    {
        //        if (fieldIdentifier.FieldEquals(() => ViewState.Sex) && ViewState.Sex == Sex.Unspecified)
        //        {
        //            AddError(() => ViewState.Sex, _localizationService.GetString("GIZ_GEN_VE_REQUIRED_FIELD"));
        //        }
        //    }
        //}
    }
}
