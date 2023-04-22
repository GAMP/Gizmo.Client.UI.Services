using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class GlobalSearchViewService : ViewStateServiceBase<GlobalSearchViewState>
    {
        #region FIELDS
        private readonly AppViewStateLookupService _appViewStateLookupService;
        private readonly UserProductViewStateLookupService _userProductStateLookupService;
        #endregion

        #region CONSTRUCTOR
        public GlobalSearchViewService(
            GlobalSearchViewState viewState,
            AppViewStateLookupService appViewStateLookupService,
            UserProductViewStateLookupService userProductStateLookupService,
            ILogger<GlobalSearchViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _appViewStateLookupService = appViewStateLookupService;
            _userProductStateLookupService = userProductStateLookupService;
        }
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
            ViewState.ApplicationResults = Enumerable.Empty<GlobalSearchResultViewState>();

            ViewState.OpenDropDown = false;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public async Task ViewAllResultsAsync(SearchResultTypes searchResultTypes)
        {
            if (searchResultTypes == SearchResultTypes.Applications)
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
            if (ViewState.ApplicationResults.Count() > 0 && ViewState.ProductResults.Count() > 0)
            {
                //Found results on both categories. Do nothing.
            }
            else
            {
                if (ViewState.ApplicationResults.Count() > 0)
                {
                    //Results found only in applications.
                    if (ViewState.ApplicationResults.Count() == 1)
                    {
                        //Only one result execute action.
                        //TODO: A Execute application?
                    }
                    else
                    {
                        //More than one results open applications page.
                        await ViewAllResultsAsync(SearchResultTypes.Applications);
                    }
                }

                if (ViewState.ProductResults.Count() > 0)
                {
                    //Results found only in applications.
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
            ViewState.ApplicationResults = Enumerable.Empty<GlobalSearchResultViewState>();

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public async Task SearchAsync(SearchResultTypes? searchResultTypes = null)
        {
            ViewState.ProductResults = Enumerable.Empty<GlobalSearchResultViewState>();
            ViewState.ApplicationResults = Enumerable.Empty<GlobalSearchResultViewState>();

            if (ViewState.SearchPattern.Length == 0)
            {
                ViewState.IsLoading = false;

                ViewState.RaiseChanged();
            }
            else
            {
                ViewState.IsLoading = true;

                ViewState.RaiseChanged();

                if (!searchResultTypes.HasValue || searchResultTypes.Value == SearchResultTypes.Applications)
                {
                    var applicationStates = await _appViewStateLookupService.GetStatesAsync();

                    var tmp = new List<GlobalSearchResultViewState>();

                    foreach (var app in applicationStates.Where(a => a.Title.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        tmp.Add(new GlobalSearchResultViewState()
                        {
                            Type = SearchResultTypes.Applications,
                            Id = app.ApplicationId,
                            Name = app.Title,
                            ImageId = app.ImageId,
                            CategoryId = app.ApplicationCategoryId
                        });
                    }

                    ViewState.ApplicationResults = tmp;
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

                ViewState.RaiseChanged();
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
