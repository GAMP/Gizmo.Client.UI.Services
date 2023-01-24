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
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
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

            //TODO: A UPDATE PROFILE

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