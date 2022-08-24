using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ApplicationsPageService : ViewStateServiceBase<ApplicationsPageViewState>
    {
        #region CONSTRUCTOR
        public ApplicationsPageService(ApplicationsPageViewState viewState,
            ILogger<ApplicationsPageService> logger,
            IServiceProvider serviceProvider, IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;

            Random random = new Random();

            viewState.Applications = _gizmoClient.GetApplications().Select(a => new ApplicationViewState()
            {
                Id = a.Id,
                Title = a.Title,
                Image = "Battle-net.png",
                Ratings = random.Next(0, 100),
                Rate = ((decimal)random.Next(1, 50)) / 10,
                ReleaseDate = new DateTime(2019, 10, 22),
                DateAdded = new DateTime(2021, 3, 12),
            }).ToList();

            foreach (var item in viewState.Applications)
            {
                if (item.Id > 1)
                {
                    item.Executables = _gizmoClient.GetExecutables().Select(a => new ExecutableViewState()
                    {
                        Id = a.Id,
                        Caption = a.Caption
                    }).ToList();
                }
            }
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
