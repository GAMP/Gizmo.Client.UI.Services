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
    public sealed class AppsPageViewStateService : ViewStateServiceBase<AppsPageViewState>
    {
        #region CONSTRUCTOR
        public AppsPageViewStateService(AppsPageViewState viewState,
            DebounceActionAsyncService debounceActionService,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService,
            ILogger<AppsPageViewStateService> logger,
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

            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Popularity, DisplayName = _localizationService.GetString("APPLICATION_SORTING_OPTION_POPULARITY") });
            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Title, DisplayName = _localizationService.GetString("APPLICATION_SORTING_OPTION_TITLE") });
            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.AddDate, DisplayName = _localizationService.GetString("APPLICATION_SORTING_OPTION_ADD_DATE") });
            sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.ReleaseDate, DisplayName = _localizationService.GetString("APPLICATION_SORTING_OPTION_RELEASE_DATE") });
            //sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Use, DisplayName = _localizationService.GetString("APPLICATION_SORTING_OPTION_USE") });
            //sortingOptions.Add(new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Rating, DisplayName = _localizationService.GetString("APPLICATION_SORTING_OPTION_RATING") });

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

            if (navigationParameters.IsInitial)
                await RefilterRequest(cToken);
        }

        #endregion

        private async Task RefilterRequest(CancellationToken cancellationToken)
        {
            ViewState.TotalFilters = 0;

            //get all app view states
            var allApplications = await _appViewStateLookupService.GetStatesAsync(cancellationToken);

            //filter out any applications that passes current app profile
            allApplications = allApplications.Where(app => _gizmoClient.AppCurrentProfilePass(app.ApplicationId));

            if (ViewState.SelectedSortingOption != ApplicationSortingOption.Popularity)
            {
                ViewState.TotalFilters += 1;
            }

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
                var allExecutables = await _appExeViewStateLookupService.GetStatesAsync(cancellationToken);

                ExecutableOptionType mode = ExecutableOptionType.None;

                foreach (var item in ViewState.SelectedExecutableModes)
                {
                    mode |= (ExecutableOptionType)item;
                }

                var applicationWithModes = allExecutables.Where(a => ((int)a.Modes & (int)mode) > 0).Select(a => a.ApplicationId).ToList();

                allApplications = allApplications.Where(app => applicationWithModes.Contains(app.ApplicationId));

                ViewState.TotalFilters += 1;

                ViewState.RaiseChanged();
            }

            switch (ViewState.SelectedSortingOption)
            {
                case ApplicationSortingOption.Popularity:

                    var popularApplications = await _gizmoClient.UserPopularApplicationsGetAsync(new Web.Api.Models.UserPopularApplicationsFilter()
                    {
                        Limit = _popularItemsOptions.Value.PopularApplications
                    });

                    var tmp = new List<AppViewState>();

                    var applicationIds = popularApplications.Select(a => a.Id).ToList();

                    foreach (var id in applicationIds)
                    {
                        var current = allApplications.Where(a => a.ApplicationId == id).FirstOrDefault();
                        if (current != null)
                            tmp.Add(current);
                    }

                    allApplications = tmp;

                    break;

                case ApplicationSortingOption.Title:

                    allApplications = allApplications.OrderBy(a => a.Title);

                    break;

                case ApplicationSortingOption.AddDate:

                    allApplications = allApplications.OrderByDescending(a => a.AddDate);

                    break;

                case ApplicationSortingOption.ReleaseDate:

                    allApplications = allApplications.OrderByDescending(a => a.ReleaseDate);

                    break;
            }

            ViewState.Applications = allApplications.ToList();

            ViewState.RaiseChanged();
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
