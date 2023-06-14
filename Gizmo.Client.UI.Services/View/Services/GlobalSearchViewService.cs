using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class GlobalSearchViewService : ViewStateServiceBase<GlobalSearchViewState>
    {
        private const int GLOBAL_SEARCH_MINIMUM_CHARACTERS = 3;

        #region CONSTRUCTOR
        public GlobalSearchViewService(GlobalSearchViewState viewState,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService,
            AppViewStateLookupService appViewStateLookupService,
            AppExeViewStateLookupService appExeViewStateLookupService,
            UserProductViewStateLookupService userProductStateLookupService,
            ILogger<GlobalSearchViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _appViewStateLookupService = appViewStateLookupService;
            _appExeViewStateLookupService = appExeViewStateLookupService;
            _userProductStateLookupService = userProductStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        private readonly AppViewStateLookupService _appViewStateLookupService;
        private readonly AppExeViewStateLookupService _appExeViewStateLookupService;
        private readonly UserProductViewStateLookupService _userProductStateLookupService;
        #endregion

        #region FUNCTIONS

        public Task OpenSearchAsync()
        {
            ViewState.OpenDropDown = true;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public Task CloseSearchAsync()
        {
            //Clear current search.
            ViewState.SearchPattern = string.Empty;
            ViewState.ProductResults = Enumerable.Empty<GlobalSearchResultViewState>();
            ViewState.ExecutableResults = Enumerable.Empty<GlobalSearchResultViewState>();

            ViewState.EmptyResultTitle = _localizationService.GetString("GIZ_GLOBAL_SEARCH_NOT_ENOUGH_CHARACTERS_TITLE");
            ViewState.EmptyResultMessage = _localizationService.GetString("GIZ_GLOBAL_SEARCH_NOT_ENOUGH_CHARACTERS_MESSAGE", GLOBAL_SEARCH_MINIMUM_CHARACTERS);

            ViewState.OpenDropDown = false;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public async Task ViewAllResultsAsync(SearchResultTypes searchResultTypes)
        {
            if (searchResultTypes == SearchResultTypes.Executables)
            {
                NavigationService.NavigateTo(ClientRoutes.ApplicationsRoute + $"?SearchPattern={ViewState.SearchPattern}");
            }
            else
            {
                NavigationService.NavigateTo(ClientRoutes.ShopRoute + $"?SearchPattern={ViewState.SearchPattern}");
            }

            ViewState.RaiseChanged();

            await CloseSearchAsync();
        }

        public async Task ProcessEnterAsync()
        {
            if (ViewState.ExecutableResults.Count() > 0 && ViewState.ProductResults.Count() > 0)
            {
                //Found results on both categories. Do nothing.
            }
            else
            {
                if (ViewState.ExecutableResults.Count() > 0)
                {
                    //Results found only in executables.
                    if (ViewState.ExecutableResults.Count() == 1)
                    {
                        //Only one result execute action.
                        //TODO: A Execute application?
                    }
                    else
                    {
                        //More than one results open applications page.
                        await ViewAllResultsAsync(SearchResultTypes.Executables);
                    }
                }

                if (ViewState.ProductResults.Count() > 0)
                {
                    //Results found only in executables.
                    if (ViewState.ProductResults.Count() == 1)
                    {
                        //Only one result execute action.
                        var userCartService = ServiceProvider.GetRequiredService<UserCartViewService>();
                        await userCartService.AddUserCartProductAsync(ViewState.ProductResults.First().Id);
                    }
                    else
                    {
                        //More than one results open shop page.
                        await ViewAllResultsAsync(SearchResultTypes.Products);
                    }
                }
            }
        }

        public Task UpdateSearchPatternAsync(string searchPattern)
        {
            ViewState.SearchPattern = searchPattern;

            return Task.CompletedTask;
        }

        public Task ClearResultsAsync()
        {
            //Clear search.
            ViewState.IsLoading = false;

            ViewState.SearchPattern = string.Empty;
            ViewState.ProductResults = Enumerable.Empty<GlobalSearchResultViewState>();
            ViewState.ExecutableResults = Enumerable.Empty<GlobalSearchResultViewState>();

            ViewState.EmptyResultTitle = _localizationService.GetString("GIZ_GLOBAL_SEARCH_NOT_ENOUGH_CHARACTERS_TITLE");
            ViewState.EmptyResultMessage = _localizationService.GetString("GIZ_GLOBAL_SEARCH_NOT_ENOUGH_CHARACTERS_MESSAGE", GLOBAL_SEARCH_MINIMUM_CHARACTERS);

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public async Task SearchAsync(SearchResultTypes? searchResultTypes = null)
        {
            ViewState.ProductResults = Enumerable.Empty<GlobalSearchResultViewState>();
            ViewState.ExecutableResults = Enumerable.Empty<GlobalSearchResultViewState>();

            ViewState.EmptyResultTitle = _localizationService.GetString("GIZ_GLOBAL_SEARCH_NO_RESULTS_TITLE");
            ViewState.EmptyResultMessage = _localizationService.GetString("GIZ_GLOBAL_SEARCH_NO_RESULTS_MESSAGE");

            if (ViewState.SearchPattern.Length < GLOBAL_SEARCH_MINIMUM_CHARACTERS)
            {
                ViewState.IsLoading = false;

                ViewState.EmptyResultTitle = _localizationService.GetString("GIZ_GLOBAL_SEARCH_NOT_ENOUGH_CHARACTERS_TITLE");
                ViewState.EmptyResultMessage = _localizationService.GetString("GIZ_GLOBAL_SEARCH_NOT_ENOUGH_CHARACTERS_MESSAGE", GLOBAL_SEARCH_MINIMUM_CHARACTERS);

                ViewState.RaiseChanged();
            }
            else
            {
                ViewState.IsLoading = true;

                ViewState.RaiseChanged();

                if (!searchResultTypes.HasValue || searchResultTypes.Value == SearchResultTypes.Executables)
                {
                    var executableStates = await _appExeViewStateLookupService.GetFilteredStatesAsync();
                    var appStates = await _appViewStateLookupService.GetFilteredStatesAsync();

                    var tmp = new List<GlobalSearchResultViewState>();

                    foreach (var exe in executableStates.Where(a => a.Caption.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        tmp.Add(new GlobalSearchResultViewState()
                        {
                            Type = SearchResultTypes.Executables,
                            Id = exe.ExecutableId,
                            Name = exe.Caption,
                            ImageId = exe.ImageId
                        });
                    }

                    foreach (var app in appStates.Where(a => a.Title.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var appExecutables = executableStates.Where(a => a.ApplicationId == app.ApplicationId).ToList();

                        foreach (var appExe in appExecutables)
                        {
                            if (!tmp.Where(a => a.Id == appExe.ExecutableId).Any())
                            {
                                tmp.Add(new GlobalSearchResultViewState()
                                {
                                    Type = SearchResultTypes.Executables,
                                    Id = appExe.ExecutableId,
                                    Name = appExe.Caption,
                                    ImageId = appExe.ImageId
                                });
                            }
                        }
                    }

                    ViewState.ExecutableResults = tmp;
                }

                if (!searchResultTypes.HasValue || searchResultTypes.Value == SearchResultTypes.Products)
                {
                    var productStates = await _userProductStateLookupService.GetStatesAsync();

                    var tmp = new List<GlobalSearchResultViewState>();

                    foreach (var product in productStates.Where(a => a.Name.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        tmp.Add(new GlobalSearchResultViewState()
                        {
                            Type = SearchResultTypes.Products,
                            Id = product.Id,
                            Name = product.Name,
                            ImageId = product.DefaultImageId,
                            CategoryId = product.ProductGroupId
                        });
                    }

                    ViewState.ProductResults = tmp;
                }

                ViewState.IsLoading = false;

                DebounceViewStateChanged();
            }
        }

        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            NavigationService.LocationChanged += NavigationService_LocationChanged;
        }

        private async void NavigationService_LocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            await ClearResultsAsync();
            await CloseSearchAsync();
        }
    }
}
