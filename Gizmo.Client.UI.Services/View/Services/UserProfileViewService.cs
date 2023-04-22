using System.Web;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.UserProfileRoute)]
    public sealed class UserProfileViewService : ViewStateServiceBase<UserProfileViewState>, IDisposable
    {
        public UserProfileViewService(UserProfileViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<UserProfileViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            var profile = await _gizmoClient.UserProfileGetAsync(cToken);

            ViewState.Id = profile.Id;
            ViewState.Username = profile.Username;
            ViewState.FirstName = profile.FirstName;
            ViewState.LastName = profile.LastName;
            ViewState.BirthDate = profile.BirthDate;
            ViewState.Sex = profile.Sex;
            ViewState.Country = profile.Country;
            ViewState.Address = profile.Address;
            ViewState.Email = profile.Email;
            ViewState.Phone = profile.Phone;
            ViewState.MobilePhone = profile.MobilePhone;
            //TODO: A ViewState.RegistrationDate = profile.RegistrationDate;
            //ViewState.Picture = profile.Picture;

            ViewState.RaiseChanged();
        }
    }
}
