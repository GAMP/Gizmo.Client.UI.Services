using System.Linq;
using System.Web;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
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
            ILocalizationService localizationService,
            ILogger<AppsPageService> logger,
            IServiceProvider serviceProvider,
            AppCategoryViewStateLookupService categoryViewStateLookupService,
            AppViewStateLookupService appViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _categoryViewStateLookupService = categoryViewStateLookupService;
            _appViewStateLookupService = appViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly AppViewStateLookupService _appViewStateLookupService;
        private readonly AppCategoryViewStateLookupService _categoryViewStateLookupService;
        #endregion

        #region OVERRIDES

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            ViewState.SortingOptions = Enum.GetValues(typeof(ApplicationSortingOption)).OfType<ApplicationSortingOption>().Select(a => new EnumFilterViewState<ApplicationSortingOption>()
            {
                Value = a,
                DisplayName = _localizationService.GetString(a)
            }).ToList();

            ViewState.ExecutableModes = Enum.GetValues(typeof(ApplicationModes)).OfType<ApplicationModes>().Select(a => new EnumFilterViewState<ApplicationModes>()
            {
                Value = a,
                DisplayName = _localizationService.GetString(a)
            }).ToList();
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

            if (navigationParameters.IsInitial)
            {
                ViewState.AppCategories = await _categoryViewStateLookupService.GetStatesAsync(cToken);
            }

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

        public async Task SetSelectedSortingOption(ApplicationSortingOption value)
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

        public async Task SetSelectedSelectedExecutableModes(IEnumerable<ApplicationModes> value)
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
            ViewState.SelectedExecutableModes = Enumerable.Empty<ApplicationModes>();

            await RefilterRequest(default);

            DebounceViewStateChange();
        }
    }
}
