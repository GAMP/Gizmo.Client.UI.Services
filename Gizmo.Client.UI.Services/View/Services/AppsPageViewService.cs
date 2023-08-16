﻿using Gizmo.Client.UI.View.States;
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
            IOptions<ClientAppsOptions> clientAppsOptions) : base(viewState, logger, serviceProvider)
        {
            _debounceActionService = debounceActionService;
            _debounceActionService.DebounceBufferTime = 500;
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _categoryViewStateLookupService = categoryViewStateLookupService;
            _appViewStateLookupService = appViewStateLookupService;
            _appExeViewStateLookupService = appExeViewStateLookupService;
            _clientAppsOptions = clientAppsOptions;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly AppViewStateLookupService _appViewStateLookupService;
        private readonly AppExeViewStateLookupService _appExeViewStateLookupService;
        private readonly AppCategoryViewStateLookupService _categoryViewStateLookupService;
        private readonly DebounceActionAsyncService _debounceActionService;
        private readonly IGizmoClient _gizmoClient;
        private readonly IOptions<ClientAppsOptions> _clientAppsOptions;
        #endregion

        private void CreateFilters()
        {
            List<EnumFilterViewState<ApplicationSortingOption>> sortingOptions = new List<EnumFilterViewState<ApplicationSortingOption>>
            {
                new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Popularity, DisplayName = _localizationService.GetString("GIZ_APP_SORTING_OPTION_POPULARITY") },
                new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.Title, DisplayName = _localizationService.GetString("GIZ_APP_SORTING_OPTION_TITLE") },
                new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.AddDate, DisplayName = _localizationService.GetString("GIZ_APP_SORTING_OPTION_ADD_DATE") },
                new EnumFilterViewState<ApplicationSortingOption>() { Value = ApplicationSortingOption.ReleaseDate, DisplayName = _localizationService.GetString("GIZ_APP_SORTING_OPTION_RELEASE_DATE") }
            };

            ViewState.SortingOptions = sortingOptions;

            List<EnumFilterViewState<ApplicationModes>> executableModes = new List<EnumFilterViewState<ApplicationModes>>
            {
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.SinglePlayer, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_SINGLE_PLAYER") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Online, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_ONLINE") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Multiplayer, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_MULTIPLAYER") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Settings, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_SETTINGS") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Utility, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_UTILITY") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Game, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_GAME") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.Application, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_APPLICATION") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.FreeToPlay, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_FREE_TO_PLAY") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.RequiresSubscription, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_REQUIRES_SUBSCRIPTION") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.FreeTrial, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_FREE_TRIAL") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.SplitScreenMultiPlayer, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_SPLIT_SCREEN_MULTIPLAYER") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.CoOpLan, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_CO_OP") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.CoOpOnline, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_CO_OP_ONLINE") },
                new EnumFilterViewState<ApplicationModes>() { Value = ApplicationModes.OneTimePurchase, DisplayName = _localizationService.GetString("GIZ_EXECUTABLE_MODE_ONE_TIME_PURCHASE") }
            };

            ViewState.ExecutableModes = executableModes;
        }

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            //TODO te only reason we need to recreate the filters is due to localization, this could be improved
            CreateFilters();

            await RefilterRequest(cToken);
        }

        #endregion

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.DefaultSortingOption = _clientAppsOptions.Value.DefaultSortingOption;
            ViewState.SelectedSortingOption = _clientAppsOptions.Value.DefaultSortingOption;        

            return base.OnInitializing(ct);
        }

        private async Task RefilterRequest(CancellationToken cancellationToken)
        {
            try
            {
                ViewState.TotalFilters = 0;

                //get all app view states
                var allApplications = await _appViewStateLookupService.GetFilteredStatesAsync(cancellationToken);

                var filteredApplications = allApplications.AsQueryable();

                IEnumerable<AppExeViewState>? allExecutables = null;
                IEnumerable<AppExeViewState>? filteredExecutables = null;

                if (!string.IsNullOrEmpty(ViewState.SearchPattern))
                {
                    ViewState.TotalFilters += 1;
                    filteredApplications = filteredApplications.Where(app => app.Title.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase));

                    allExecutables = await _appExeViewStateLookupService.GetFilteredStatesAsync(cancellationToken);
                    filteredExecutables = allExecutables.Where(a => a.Caption.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase)).ToList();
                }

                if (ViewState.SelectedExecutableModes.Any())
                {
                    allExecutables ??= await _appExeViewStateLookupService.GetFilteredStatesAsync(cancellationToken);

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

                var filteredResult = filteredApplications.ToList();

                if (filteredExecutables != null)
                {
                    var alreadyIncludedIds = filteredResult.Select(a => a.ApplicationId).ToList();
                    var additionalIds = filteredExecutables.Where(a => !alreadyIncludedIds.Contains(a.ApplicationId)).Select(a => a.ApplicationId).Distinct();
                    filteredResult.AddRange(allApplications.Where(a => additionalIds.Contains(a.ApplicationId)));
                }

                if (ViewState.SelectedSortingOption != ViewState.DefaultSortingOption)
                {
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
                        filteredResult = filteredResult.OrderByDescending(a => applicationIds.IndexOf(a.ApplicationId)).ToList();

                        break;

                    case ApplicationSortingOption.Title:

                        filteredResult = filteredResult.OrderBy(a => a.Title).ToList();

                        break;

                    case ApplicationSortingOption.AddDate:

                        filteredResult = filteredResult.OrderByDescending(a => a.AddDate).ToList();

                        break;

                    case ApplicationSortingOption.ReleaseDate:

                        filteredResult = filteredResult.OrderByDescending(a => a.ReleaseDate).ToList();

                        break;
                }

                //create a list of applications that is not filtered by category
                var nonCategoryFilteredApps = filteredResult.ToList();

                //filter out any applications that dont belong to filtered app group
                if (ViewState.SelectedCategoryId.HasValue)
                {
                    filteredResult = filteredResult.Where(app => app.ApplicationCategoryId == ViewState.SelectedCategoryId).ToList();
                    ViewState.TotalFilters += 1;
                }

                //get all app categoreis
                var allCategories = await _categoryViewStateLookupService.GetStatesAsync(cancellationToken);

                //filter out any app category that does not contain any application passing current filter or app profile
                allCategories = allCategories
                    .Where(c => nonCategoryFilteredApps.Where(a => a.ApplicationCategoryId == c.AppCategoryId).Any())
                    .OrderBy(c => c.Name).ToList()
                    .ToList();

                //reset selected category
                var selectedCategory = ViewState.SelectedCategoryId;
                if (selectedCategory.HasValue && !allCategories.Where(c => c.AppCategoryId == selectedCategory.Value).Any())
                {
                    //it makes more sense to set to null and allow user to see all the filtered results
                    //and then filter them out by category instead of selecting first found category
                    ViewState.SelectedCategoryId = null;
                }

                ViewState.AppCategories = allCategories;
                ViewState.Applications = filteredResult;

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

            ViewState.SelectedSortingOption = ViewState.DefaultSortingOption;
            ViewState.SelectedCategoryId = null;
            ViewState.SelectedExecutableModes = Enumerable.Empty<ApplicationModes>();

            _debounceActionService.Debounce(RefilterRequest);

            return Task.CompletedTask;
        }

        public async Task NavigateWithSearchAsync(string searchPattern)
        {
            await ClearAllFilters();

            ViewState.SearchPattern = searchPattern;
            NavigationService.NavigateTo(ClientRoutes.ApplicationsRoute);
        }
    }
}
