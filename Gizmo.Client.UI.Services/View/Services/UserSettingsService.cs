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

        public Task LoadAsync()
        {
            UserModelBase tmp = new UserModelBase();

            ViewState.Username = tmp.Username;
            ViewState.FirstName = tmp.FirstName;
            ViewState.LastName = tmp.LastName;
            ViewState.BirthDate = tmp.BirthDate;
            ViewState.Sex = tmp.Sex;
            ViewState.Country = tmp.Country;
            ViewState.Address = tmp.Address;
            ViewState.Email = tmp.Email;
            ViewState.Phone = tmp.Phone;
            ViewState.MobilePhone = tmp.MobilePhone;
            //TODO: A IMAGE

            return Task.CompletedTask;
        }

        public Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            //TODO: A UPDATE PROFILE

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }
    }
}