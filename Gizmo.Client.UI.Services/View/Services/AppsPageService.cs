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

            List<EnumFilterViewState<ApplicationSortingOption>> sortingOptions = new List<EnumFilterViewState<ApplicationSortingOption>>();

            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Popularity, DisplayName = _localizationService.GetString("APPLICATION_SORTING_OPTION_POPULARITY") });
            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.AddDate, DisplayName = _localizationService.GetString("APPLICATION_SORTING_OPTION_ADD_DATE") });
            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Title, DisplayName = _localizationService.GetString("APPLICATION_SORTING_OPTION_TITLE") });
            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Use, DisplayName = _localizationService.GetString("APPLICATION_SORTING_OPTION_USE") });
            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Rating, DisplayName = _localizationService.GetString("APPLICATION_SORTING_OPTION_RATING") });
            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.ReleaseDate, DisplayName = _localizationService.GetString("APPLICATION_SORTING_OPTION_RELEASE_DATE") });

            ViewState.SortingOptions = sortingOptions;

            List<EnumFilterViewState<ApplicationModes>> executableModes = new List<EnumFilterViewState<ApplicationModes>>();
            //executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.DefaultMode, DisplayName = "DefaultMode" });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.SinglePlayer, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_SINGLE_PLAYER") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Online, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_ONLINE") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Multiplayer, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_MULTIPLAYER") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Settings, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_SETTINGS") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Utility, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_UTILITY") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Game, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_GAME") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Application, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_APPLICATION") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.FreeToPlay, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_FREE_TO_PLAY") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.RequiresSubscription, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_REQUIRES_SUBSCRIPTION") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.FreeTrial, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_FREE_TRIAL") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.SplitScreenMultiPlayer, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_SPLIT_SCREEN_MULTIPLAYER") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.CoOpLan, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_CO_OP") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.CoOpOnline, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_CO_OP_ONLINE") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.OneTimePurchase, DisplayName = _localizationService.GetString("EXECUTABLE_MODE_ONE_TIME_PURCHASE") });

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
