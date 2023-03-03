using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class AdvertisementsService : ViewStateServiceBase<AdvertisementsViewState>
    {
        #region CONSTRUCTOR
        public AdvertisementsService(AdvertisementsViewState viewState,
            ILogger<AdvertisementsService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
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

        public async Task LoadAdvertisementsAsync()
        {
            //TODO: A Load advertisments on user login?

            //Test
            Random random = new Random();

            var tmp = new List<AdvertisementViewState>();

            tmp.Add(new AdvertisementViewState()
            {
                Id = 1,
                Title = $"#Title 1",
                Body = "<div style=\"max-width: 40.0rem; margin: 8.6rem 3.2rem 6.5rem 3.2rem\">#1 Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</div>",
                ThumbnailUrl = $"carousel_1.jpg"
            });

            tmp.Add(new AdvertisementViewState()
            {
                Id = 2,
                Title = $"#Title 2",
                Body = "#2 Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                ThumbnailUrl = $"carousel_2.jpg"
            });

            tmp.Add(new AdvertisementViewState()
            {
                Id = 3,
                Title = $"#Title 3",
                Body = "#3 Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                ThumbnailUrl = $"carousel_3.jpg"
            });
            //End Test

            ViewState.Advertisements = tmp;

            ViewState.RaiseChanged();
        }

        #endregion
    }
}
