using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserService : ViewStateServiceBase<UserViewState>, IDisposable
    {
        #region CONSTRUCTOR
        public UserService(UserViewState viewState,
            ILogger<UserService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            viewState.Username = "Infidel 06";
            viewState.RegistrationDate = new DateTime(2020, 3, 4);
            viewState.Balance = 10.76m;
            viewState.CurrentTimeProduct = "Six Hours (6) for 10$ Pack";
            viewState.Time = new TimeSpan(6, 36, 59);
            viewState.Points = 416;
            viewState.Picture = "_content/Gizmo.Client.UI/img/Cyber Punks.png";
        }
        #endregion

        public Task LogοutAsync()
        {
            NavigationService.NavigateTo("/");
            return Task.CompletedTask;
        }
    }
}
