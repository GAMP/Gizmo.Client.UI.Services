using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.UserSettingsRoute)]
    public sealed class UserChangeProfileViewStateService : ValidatingViewStateServiceBase<UserChangeProfileViewState>
    {
        #region CONSTRUCTOR
        public UserChangeProfileViewStateService(UserChangeProfileViewState viewState,
            ILogger<UserChangeProfileViewStateService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
		{
			_gizmoClient = gizmoClient;
		}
		#endregion

		#region FIELDS
		private readonly IGizmoClient _gizmoClient;
		#endregion

		#region FUNCTIONS

		public void SetUsername(string value)
        {
            ViewState.Username = value;
            ValidateProperty(() => ViewState.Username);
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

        public async Task LoadAsync(CancellationToken cToken = default)
        {
            var profile = await _gizmoClient.UserProfileGetAsync(cToken);

            ViewState.Username = profile.Username;
            ViewState.FirstName = profile.FirstName;
            ViewState.LastName = profile.LastName;
            ViewState.BirthDate = profile.BirthDate;
            ViewState.Sex = profile.Sex;
            ViewState.Country = profile.Country;

            ViewState.RaiseChanged();
        }

        public async Task SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            try
            {
                await _gizmoClient.UserProfileUpdateAsync(new UserProfileModelUpdate()
                {
                    Username = ViewState.Username,
                    FirstName = ViewState.FirstName,
                    LastName = ViewState.LastName,
                    BirthDate = ViewState.BirthDate,
                    Sex = ViewState.Sex,
                    Country = ViewState.Country
                });

                //TODO: A Update loaded profile instantly or wait for event?

                var userViewState = ServiceProvider.GetRequiredService<UserViewState>();

                userViewState.Username = ViewState.Username;
                userViewState.FirstName = ViewState.FirstName;
                userViewState.LastName = ViewState.LastName;
                userViewState.BirthDate = ViewState.BirthDate;
                userViewState.Sex = ViewState.Sex;
                userViewState.Country = ViewState.Country;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User profile update error.");

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

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            await LoadAsync(cToken);
        }
    }
}
