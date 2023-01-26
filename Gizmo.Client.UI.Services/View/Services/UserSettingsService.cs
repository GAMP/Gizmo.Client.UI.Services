using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
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
            ViewState.RaiseChanged();
        }

        public void SetFirstName(string value)
        {
            ViewState.FirstName = value;
            ViewState.RaiseChanged();
        }

        public void SetLastName(string value)
        {
            ViewState.LastName = value;
            ViewState.RaiseChanged();
        }

        public void SetBirthDate(DateTime? value)
        {
            ViewState.BirthDate = value;
            ViewState.RaiseChanged();
        }

        public void SetSex(Sex value)
        {
            ViewState.Sex = value;
            ViewState.RaiseChanged();
        }

        public void SetCountry(string value)
        {
            ViewState.Country = value;
            ViewState.RaiseChanged();
        }

        public void SetAddress(string value)
        {
            ViewState.Address = value;
            ViewState.RaiseChanged();
        }

        public void SetEmail(string value)
        {
            ViewState.Email = value;
            ViewState.RaiseChanged();
        }

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ViewState.RaiseChanged();
        }

        public void SetPhone(string value)
        {
            ViewState.Phone = value;
            ViewState.RaiseChanged();
        }

        public Task LoadAsync()
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
            //TODO: A POST CODE, IMAGE

            return Task.CompletedTask;
        }

        public Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            _gizmoClient.UpdateUserProfileAsync(new UserModelUpdate()
            {
                Id = 0000,
                //Password = ,
                Username = ViewState.Username,
                Email = ViewState.Email,
                UserGroupId = 0000,
                //IsNegativeBalanceAllowed = ,
                //IsPersonalInfoRequested = ,
                //EnableDate = ,
                //DisabledDate = ,
                FirstName = ViewState.FirstName,
                LastName = ViewState.LastName,
                BirthDate = ViewState.BirthDate,
                Address = ViewState.Address,
                //City = ,
                Country = ViewState.Country,
                //PostCode = ,
                Phone = ViewState.Phone,
                MobilePhone = ViewState.MobilePhone,
                Sex = ViewState.Sex,
                //IsDeleted = ,
                //IsDisabled = ,
                //SmartCardUid = ,
                //Identification = 
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
			//TODO: A POST CODE, IMAGE



			ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion
    }
}