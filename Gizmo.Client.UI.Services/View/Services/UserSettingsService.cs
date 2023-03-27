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
    public sealed class UserSettingsService : ValidatingViewStateServiceBase<UserSettingsViewState>
    {
        #region CONSTRUCTOR
        public UserSettingsService(UserSettingsViewState viewState,
            ILogger<UserSettingsService> logger,
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
            DebounceViewStateChanged();
        }

        public void SetFirstName(string value)
        {
            ViewState.FirstName = value;
            ValidateProperty(() => ViewState.FirstName);
            DebounceViewStateChanged();
        }

        public void SetLastName(string value)
        {
            ViewState.LastName = value;
            ValidateProperty(() => ViewState.LastName);
            DebounceViewStateChanged();
        }

        public void SetBirthDate(DateTime? value)
        {
            ViewState.BirthDate = value;
            ValidateProperty(() => ViewState.BirthDate);
            DebounceViewStateChanged();
        }

        public void SetSex(Sex value)
        {
            ViewState.Sex = value;
            ValidateProperty(() => ViewState.Sex);
            DebounceViewStateChanged();
        }

        public void SetCountry(string value)
        {
            ViewState.Country = value;
            ValidateProperty(() => ViewState.Country);
            DebounceViewStateChanged();
        }

        public void SetAddress(string value)
        {
            ViewState.Address = value;
            ValidateProperty(() => ViewState.Address);
            DebounceViewStateChanged();
        }

        public void SetEmail(string value)
        {
            ViewState.Email = value;
            ValidateProperty(() => ViewState.Email);
            DebounceViewStateChanged();
        }

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ValidateProperty(() => ViewState.MobilePhone);
            DebounceViewStateChanged();
        }

        public void SetPhone(string value)
        {
            ViewState.Phone = value;
            ValidateProperty(() => ViewState.Phone);
            DebounceViewStateChanged();
        }

        public Task LoadAsync(CancellationToken cToken = default)
        {
            var userViewState = ServiceProvider.GetRequiredService<UserViewState>();

            ViewState.Username = userViewState.Username;
            ViewState.FirstName = userViewState.FirstName;
            ViewState.LastName = userViewState.LastName;
            ViewState.BirthDate = userViewState.BirthDate;
            ViewState.Sex = userViewState.Sex;
            ViewState.Country = userViewState.Country;
            ViewState.Address = userViewState.Address;
            ViewState.Email = userViewState.Email;
            ViewState.Phone = userViewState.Phone;
            ViewState.MobilePhone = userViewState.MobilePhone;
            //TODO: A Service User POST CODE, IMAGE

            return Task.CompletedTask;
        }

        public Task SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            _gizmoClient.UserProfileUpdateAsync(new UserProfileModelUpdate()
            {
                Username = ViewState.Username,
                Email = ViewState.Email,
                FirstName = ViewState.FirstName,
                LastName = ViewState.LastName,
                BirthDate = ViewState.BirthDate,
                Address = ViewState.Address,
                Country = ViewState.Country,
                //PostCode = ,
                Phone = ViewState.Phone,
                MobilePhone = ViewState.MobilePhone,
                Sex = ViewState.Sex
            });

            //TODO: A Update loaded profile instantly or wait for event?
            
            var userViewState = ServiceProvider.GetRequiredService<UserViewState>();

            userViewState.Username = ViewState.Username;
            userViewState.FirstName = ViewState.FirstName;
            userViewState.LastName = ViewState.LastName;
            userViewState.BirthDate = ViewState.BirthDate;
            userViewState.Sex = ViewState.Sex;
            userViewState.Country = ViewState.Country;
            userViewState.Address = ViewState.Address;
            userViewState.Email = ViewState.Email;
            userViewState.Phone = ViewState.Phone;
            userViewState.MobilePhone = ViewState.MobilePhone;
			//TODO: A Service POST CODE, IMAGE



			ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            await LoadAsync(cToken);
        }
    }
}
