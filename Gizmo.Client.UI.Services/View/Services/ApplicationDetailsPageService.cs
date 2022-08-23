using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ApplicationDetailsPageService : ViewStateServiceBase<ApplicationDetailsPageViewState>
    {
        #region CONSTRUCTOR
        public ApplicationDetailsPageService(ApplicationDetailsPageViewState viewState,
            ILogger<ApplicationDetailsPageService> logger,
            IServiceProvider serviceProvider, IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;

            Random random = new Random();

            viewState.Application = _gizmoClient.GetApplications().Select(a => new ApplicationViewState()
            {
                Id = a.Id,
                Title = a.Title,
                Image = "Battle-net.png",
                Ratings = random.Next(0, 100),
                Rate = ((decimal)random.Next(1, 50)) / 10,
                ReleaseDate = new DateTime(2019, 10, 22),
                DateAdded = new DateTime(2021, 3, 12),
            }).FirstOrDefault();
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES


        #endregion

        #region FUNCTIONS

        #endregion
    }
}
