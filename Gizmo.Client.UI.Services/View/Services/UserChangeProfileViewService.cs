using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
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
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _dialogService = dialogService;
            _gizmoClient = gizmoClient;
		}
        #endregion

        #region FIELDS
        private readonly IClientDialogService _dialogService;
        private readonly IGizmoClient _gizmoClient;
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

        public void SetPrefix(string value)
        {
            ViewState.Prefix = value;
            DebounceViewStateChanged();
        }

        public async Task StartAsync(CancellationToken cToken = default)
        {
            await ResetAsync();

            ViewState.IsInitializing = true;
            ViewState.IsInitialized = false;

            var s = await _dialogService.ShowChangeProfileDialogAsync();
            if (s.Result == DialogAddResult.Success)
            {
                //try
                //{
                //    var result = await s.WaitForDialogResultAsync();
                //}
                //catch (OperationCanceledException)
                //{
                //}
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
                ViewState.ErrorMessage = ex.ToString();

                ViewState.IsComplete = true;
            }
            finally
            {
                ViewState.IsInitializing = false;

                ViewState.RaiseChanged();
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
                ViewState.ErrorMessage = ex.ToString();
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
            ViewState.IsInitializing = false;
            ViewState.IsInitialized = null;
            ViewState.IsComplete = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion
    }
}
