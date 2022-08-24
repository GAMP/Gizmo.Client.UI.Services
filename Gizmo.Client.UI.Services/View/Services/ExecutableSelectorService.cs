using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ExecutableSelectorService : ViewStateServiceBase<ExecutableSelectorViewState>
    {
        #region CONSTRUCTOR
        public ExecutableSelectorService(ExecutableSelectorViewState viewState,
            ILogger<ExecutableSelectorService> logger,
            IServiceProvider serviceProvider, IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public void SetApplication(int id)
        {
            Random random = new Random();

            ViewState.Application = _gizmoClient.GetApplications().Where(a => a.Id == id).Select(a => new ApplicationViewState()
            {
                Id = a.Id,
                Title = a.Title,
                Image = "Battle-net.png",
                Ratings = random.Next(0, 100),
                Rate = ((decimal)random.Next(1, 50)) / 10,
                ReleaseDate = new DateTime(2019, 10, 22),
                DateAdded = new DateTime(2021, 3, 12),
            }).FirstOrDefault();

            ViewState.Application.Executables = _gizmoClient.GetExecutables().Select(a=> new ExecutableViewState()
            {
                Id = a.Id,
                Caption = a.Caption,
                Image = "_content/Gizmo.Client.UI/img/Chrome-icon 1.png"
            }).ToList();
        }

        #endregion
    }
}