using System.Web;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.ApplicationsRoute)]
    public sealed class AppsPageViewService : ViewStateServiceBase<AppsPageViewState>
    {
        #region CONSTRUCTOR
        public AppsPageViewService(AppsPageViewState viewState,
            DebounceActionAsyncService debounceActionService,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService,
            ILogger<AppsPageViewService> logger,
            IServiceProvider serviceProvider,
            AppCategoryViewStateLookupService categoryViewStateLookupService,
            AppViewStateLookupService appViewStateLookupService,
            AppExeViewStateLookupService appExeViewStateLookupService,
            IOptions<PopularItemsOptions> popularItemsOptions) : base(viewState, logger, serviceProvider)
        {
            _debounceActionService = debounceActionService;
            _debounceActionService.DebounceBufferTime = 500;
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _categoryViewStateLookupService = categoryViewStateLookupService;
            _appViewStateLookupService = appViewStateLookupService;
            _appExeViewStateLookupService = appExeViewStateLookupService;
            _popularItemsOptions = popularItemsOptions;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly AppViewStateLookupService _appViewStateLookupService;
        private readonly AppExeViewStateLookupService _appExeViewStateLookupService;
        private readonly AppCategoryViewStateLookupService _categoryViewStateLookupService;
        private readonly DebounceActionAsyncService _debounceActionService;
        private readonly IGizmoClient _gizmoClient;
        private readonly IOptions<PopularItemsOptions> _popularItemsOptions;
        #endregion

        #region OVERRIDES

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            List<EnumFilterViewState<ApplicationSortingOption>> sortingOptions = new List<EnumFilterViewState<ApplicationSortingOption>>();

            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Popularity, DisplayName = _localizationService.GetString("GIZ_APP_SORTING_OPTION_POPULARITY") });
            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Title, DisplayName = _localizationService.GetString("GIZ_APP_SORTING_OPTION_TITLE") });
            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.AddDate, DisplayName = _localizationService.GetString("GIZ_APP_SORTING_OPTION_ADD_DATE") });
            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.ReleaseDate, DisplayName = _localizationService.GetString("GIZ_APP_SORTING_OPTION_RELEASE_DATE") });
            //sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Use, DisplayName = _localizationService.GetString("GIZ_APP_SORTING_OPTION_USE") });
            //sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Rating, DisplayName = _localizationService.GetString("GIZ_APP_SORTING_OPTION_RATING") });

            ViewState.SortingOptions = sortingOptions;

            List<EnumFilterViewState<ApplicationModes>> executableModes = new List<EnumFilterViewState<ApplicationModes>>();
            //executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.DefaultMode, DisplayName = "DefaultMode" });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.SinglePlayer, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_SINGLE_PLAYER") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Online, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_ONLINE") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Multiplayer, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_MULTIPLAYER") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Settings, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_SETTINGS") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Utility, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_UTILITY") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Game, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_GAME") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Application, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_APPLICATION") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.FreeToPlay, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_FREE_TO_PLAY") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.RequiresSubscription, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_REQUIRES_SUBSCRIPTION") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.FreeTrial, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_FREE_TRIAL") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.SplitScreenMultiPlayer, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_SPLIT_SCREEN_MULTIPLAYER") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.CoOpLan, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_CO_OP") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.CoOpOnline, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_CO_OP_ONLINE") });
            executableModes.Add(new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.OneTimePurchase, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_ONE_TIME_PURCHASE") });

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
                var allCategories = await _categoryViewStateLookupService.GetStatesAsync(cToken);

                ViewState.AppCategories = allCategories.OrderBy(c => c.Name).ToList();
            }

            if (navigationParameters.IsInitial)
                await RefilterRequest(cToken);
        }

        #endregion

        private async Task RefilterRequest(CancellationToken cancellationToken)
        {
            try
            {
                ViewState.TotalFilters = 0;

                //get all app view states
                var allApplications = await _appViewStateLookupService.GetStatesAsync(cancellationToken);

                //filter out any applications that passes current app profile
                var filteredApplications = allApplications.Where(app => _gizmoClient.AppCurrentProfilePass(app.ApplicationId));

                if (ViewState.SelectedSortingOption != ApplicationSortingOption.Popularity)
                {
                    ViewState.TotalFilters += 1;
                }

                if (ViewState.SelectedCategoryId.HasValue)
                {
                    filteredApplications = filteredApplications.Where(app => app.ApplicationCategoryId == ViewState.SelectedCategoryId);
                    ViewState.TotalFilters += 1;
                }

                IEnumerable<AppExeViewState>? allExecutables = null;
                IEnumerable<AppExeViewState>? filteredExecutables = null;

                if (!string.IsNullOrEmpty(ViewState.SearchPattern))
                {
                    filteredApplications = filteredApplications.Where(app => app.Title.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase));
                    ViewState.TotalFilters += 1;

                    allExecutables = await _appExeViewStateLookupService.GetFilteredStatesAsync(cancellationToken);
                    filteredExecutables = allExecutables.Where(a => a.Caption.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase)).ToList();
                }

                if (ViewState.SelectedExecutableModes.Count() > 0)
                {
                    if (allExecutables == null)
                        allExecutables = await _appExeViewStateLookupService.GetFilteredStatesAsync(cancellationToken);

                    ExecutableOptionType mode = ExecutableOptionType.None;

                    foreach (var item in ViewState.SelectedExecutableModes)
                    {
                        mode |= (ExecutableOptionType)item;
                    }

                    var applicationWithModes = allExecutables.Where(a => ((int)a.Modes & (int)mode) > 0).Select(a => a.ApplicationId).ToList();

                    filteredApplications = filteredApplications.Where(app => applicationWithModes.Contains(app.ApplicationId));

                    if (filteredExecutables != null)
                        filteredExecutables = filteredExecutables.Where(a => ((int)a.Modes & (int)mode) > 0);

                    ViewState.TotalFilters += 1;
                }

                switch (ViewState.SelectedSortingOption)
                {
                    case ApplicationSortingOption.Popularity:

                        var popularApplications = await _gizmoClient.UserPopularApplicationsGetAsync(new Web.Api.Models.UserPopularApplicationsFilter()
                        {
                            Limit = -1
                        }, cancellationToken);

                        var applicationIds = popularApplications.Select(a => a.Id).Reverse().ToList();

                        //Apps that are not included in popular have index -1 so we have to reverse the order and sort descending.
                        filteredApplications = filteredApplications.OrderByDescending(a => applicationIds.IndexOf(a.ApplicationId)).ToList();

                        break;

                    case ApplicationSortingOption.Title:

                        filteredApplications = filteredApplications.OrderBy(a => a.Title);

                        break;

                    case ApplicationSortingOption.AddDate:

                        filteredApplications = filteredApplications.OrderByDescending(a => a.AddDate);

                        break;

                    case ApplicationSortingOption.ReleaseDate:

                        filteredApplications = filteredApplications.OrderByDescending(a => a.ReleaseDate);

                        break;
                }

                var tmp = filteredApplications.ToList();

                if (filteredExecutables != null)
                {
                    var alreadyIncludedIds = tmp.Select(a => a.ApplicationId).ToList();
                    var additionalIds = filteredExecutables.Where(a => !alreadyIncludedIds.Contains(a.ApplicationId)).Select(a => a.ApplicationId).Distinct();
                    tmp.AddRange(allApplications.Where(a => additionalIds.Contains(a.ApplicationId)));
                }

                ViewState.Applications = tmp;

                DebounceViewStateChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to filter applications.");
            }
        }

        public Task SetSelectedSortingOption(ApplicationSortingOption value)
        {
            ViewState.SelectedSortingOption = value;

            _debounceActionService.Debounce(RefilterRequest);

            return Task.CompletedTask;
        }

        public Task SetSelectedApplicationCategory(int? value)
        {
            ViewState.SelectedCategoryId = value;

            _debounceActionService.Debounce(RefilterRequest);

            return Task.CompletedTask;
        }

        public Task SetSelectedSelectedExecutableModes(IEnumerable<ApplicationModes> value)
        {
            ViewState.SelectedExecutableModes = value;

            _debounceActionService.Debounce(RefilterRequest);

            return Task.CompletedTask;
        }

        public Task ClearSearchPattern()
        {
            ViewState.SearchPattern = string.Empty;

            _debounceActionService.Debounce(RefilterRequest);

            return Task.CompletedTask;
        }

        public Task ClearAllFilters()
        {
            ViewState.SearchPattern = string.Empty;

            ViewState.SelectedSortingOption = ApplicationSortingOption.Popularity;
            ViewState.SelectedCategoryId = null;
            ViewState.SelectedExecutableModes = Enumerable.Empty<ApplicationModes>();

            _debounceActionService.Debounce(RefilterRequest);

            return Task.CompletedTask;
        }
    }
}
