using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.ApplicationsRoute)]
    public sealed class ApplicationsPageService : ViewStateServiceBase<ApplicationsPageViewState>
    {
        #region CONSTRUCTOR
        public ApplicationsPageService(ApplicationsPageViewState viewState,
            ILogger<ApplicationsPageService> logger,
            IServiceProvider serviceProvider,
            AppCategoryViewStateLookupService categoryViewStateLookupService,
            AppViewStateLookupService appViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _categoryViewStateLookupService = categoryViewStateLookupService;
            _appViewStateLookupService = appViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly AppViewStateLookupService _appViewStateLookupService;
        private AppCategoryViewStateLookupService _categoryViewStateLookupService;
        #endregion

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            //if (navigationParameters.IsInitial)
            //{
            ViewState.AppCategories = await _categoryViewStateLookupService.GetStatesAsync(cToken);
            //}

            await RefilterRequest(cToken);

        }

        #endregion

        private async Task RefilterRequest(CancellationToken cancellationToken)
        {
            ViewState.TotalFilters = 0;

            var allApplications = await _appViewStateLookupService.GetStatesAsync(cancellationToken);

            if (ViewState.SelectedCategoryId.HasValue)
            {
                allApplications = allApplications.Where(app => app.ApplicationCategoryId == ViewState.SelectedCategoryId);
                ViewState.TotalFilters += 1;
            }

            ViewState.Applications = allApplications.ToList();

            ViewState.RaiseChanged();
        }

        public async Task SetCurrentAppCategory(int? appCategoryId)
        {
            ViewState.SelectedCategoryId = appCategoryId;

            await RefilterRequest(default);

            DebounceViewStateChange();
        }

        public async Task ClearSearchPattern()
        {
            ViewState.SearchPattern = string.Empty;

            await RefilterRequest(default);

            DebounceViewStateChange();
        }

        public async Task ClearAllFilters()
        {
            ViewState.SearchPattern = string.Empty;
            ViewState.SelectedCategoryId = null;

            await RefilterRequest(default);

            DebounceViewStateChange();
        }
    }
}
