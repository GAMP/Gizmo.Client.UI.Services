using System.Web;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.ApplicationsRoute)]
    public sealed class AppsPageService : ViewStateServiceBase<AppsPageViewState>
    {
        #region CONSTRUCTOR
        public AppsPageService(AppsPageViewState viewState,
            ILogger<AppsPageService> logger,
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

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            List<ApplicationFilterViewState> sortingOptions = new List<ApplicationFilterViewState>();

            sortingOptions.Add(new ApplicationFilterViewState() { Id = 0, Name = "Popularity" });
            sortingOptions.Add(new ApplicationFilterViewState() { Id = 1, Name = "Add date" });
            sortingOptions.Add(new ApplicationFilterViewState() { Id = 2, Name = "Title" });
            sortingOptions.Add(new ApplicationFilterViewState() { Id = 3, Name = "Use" });
            sortingOptions.Add(new ApplicationFilterViewState() { Id = 4, Name = "Rating" });
            sortingOptions.Add(new ApplicationFilterViewState() { Id = 5, Name = "Release Date" });

            ViewState.SortingOptions = sortingOptions;

            List<ApplicationFilterViewState> executableModes = new List<ApplicationFilterViewState>();

            executableModes.Add(new ApplicationFilterViewState() { Id = 1, Name = "Access" });
            executableModes.Add(new ApplicationFilterViewState() { Id = 2, Name = "Rating" });
            executableModes.Add(new ApplicationFilterViewState() { Id = 3, Name = "Type" });
            executableModes.Add(new ApplicationFilterViewState() { Id = 4, Name = "Rating" });
            executableModes.Add(new ApplicationFilterViewState() { Id = 5, Name = "Player mode" });

            ViewState.ExecutableModes = executableModes;
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            if (Uri.TryCreate(NavigationService.GetUri(), UriKind.Absolute, out var uri))
            {
                string? searchPattern = HttpUtility.ParseQueryString(uri.Query).Get("SearchPattern");
                if (!string.IsNullOrEmpty(searchPattern))
                {
                    ViewState.SearchPattern = searchPattern;
                }
            }

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

            if (!string.IsNullOrEmpty(ViewState.SearchPattern))
            {
                allApplications = allApplications.Where(app => app.Title.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase));
                ViewState.TotalFilters += 1;
            }

            if (ViewState.SelectedCategoryId.HasValue)
            {
                allApplications = allApplications.Where(app => app.ApplicationCategoryId == ViewState.SelectedCategoryId);
                ViewState.TotalFilters += 1;
            }

            if (ViewState.SelectedExecutableModes.Count() > 0)
            {
                ViewState.TotalFilters += 1;
            }

            ViewState.Applications = allApplications.ToList();

            ViewState.RaiseChanged();
        }

        public async Task SetSelectedSortingOption(int value)
        {
            ViewState.SelectedSortingOption = value;

            await RefilterRequest(default);

            DebounceViewStateChange();
        }

        public async Task SetSelectedApplicationCategory(int? value)
        {
            ViewState.SelectedCategoryId = value;

            await RefilterRequest(default);

            DebounceViewStateChange();
        }

        public async Task SetSelectedSelectedExecutableModes(IEnumerable<int> value)
        {
            ViewState.SelectedExecutableModes = value;

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

            ViewState.SelectedSortingOption = 0;
            ViewState.SelectedCategoryId = null;
            ViewState.SelectedExecutableModes = Enumerable.Empty<int>();

            await RefilterRequest(default);

            DebounceViewStateChange();
        }
    }
}
