using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class GlobalSearchViewService : ViewStateServiceBase<GlobalSearchViewState>
    {
        private const int GLOBAL_SEARCH_MINIMUM_CHARACTERS = 3;

        #region CONSTRUCTOR
        public GlobalSearchViewService(GlobalSearchViewState viewState,
            IOptionsMonitor<ClientShopOptions> shopOptions,
            ILocalizationService localizationService,
            AppsPageViewService appsPageViewService,
            ProductsPageViewService productsPageViewService,
            AppViewStateLookupService appViewStateLookupService,
            AppExeViewStateLookupService appExeViewStateLookupService,
            UserProductViewStateLookupService userProductStateLookupService,
            ILogger<GlobalSearchViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _shopOptions = shopOptions;
            _localizationService = localizationService;
            _appsPageViewService = appsPageViewService;
            _productsPageViewService = productsPageViewService;
            _appViewStateLookupService = appViewStateLookupService;
            _appExeViewStateLookupService = appExeViewStateLookupService;
            _userProductStateLookupService = userProductStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly IOptionsMonitor<ClientShopOptions> _shopOptions;
        private readonly ILocalizationService _localizationService;
        private readonly AppsPageViewService _appsPageViewService;
        private readonly ProductsPageViewService _productsPageViewService;
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
                await _appsPageViewService.NavigateWithSearchAsync(ViewState.SearchPattern);
            }
            else
            {
                await _productsPageViewService.NavigateWithSearchAsync(ViewState.SearchPattern);
            }

            //TODO: Pestunov: Why is this here?
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

            DebounceViewStateChanged();

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

                //only produce search results for producst if shop is enabled
                if (!_shopOptions.CurrentValue.Disabled)
                {
                    if (!searchResultTypes.HasValue || searchResultTypes.Value == SearchResultTypes.Products)
                    {
                        var productStates = await _userProductStateLookupService.GetFilteredStatesAsync(ViewState.SearchPattern);

                        var tmp = new List<GlobalSearchResultViewState>();

                        foreach (var product in productStates)
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
                }

                ViewState.IsLoading = false;

                DebounceViewStateChanged();
            }
        }

        #endregion

        protected override async Task OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            //TODO we dont constantly need to raise view state change, its not a peformance problem but some
            //cleaner approach should be used
            await ClearResultsAsync();
            await CloseSearchAsync();
        }
    }
}
