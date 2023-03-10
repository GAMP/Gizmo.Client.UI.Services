using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class GlobalSearchService : ViewStateServiceBase<GlobalSearchViewState>
    {
        #region FIELDS
        private readonly UserProductViewStateLookupService _userProductStateLookupService;
        private readonly IGizmoClient _gizmoClient;
        private bool _ignoreLocationChange = false;
        #endregion

        #region CONSTRUCTOR
        public GlobalSearchService(
            GlobalSearchViewState viewState,
            UserProductViewStateLookupService userProductStateLookupService,
            ILogger<GlobalSearchService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            this._userProductStateLookupService = userProductStateLookupService;
            _gizmoClient = gizmoClient;
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
            _ignoreLocationChange = true;

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

            //_ignoreLocationChange = false;
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
                        //TODO: A
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
                        var userCartService = ServiceProvider.GetRequiredService<UserCartService>();
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

                //Test
                //Simulate service call.
                await Task.Delay(500);

                if (!searchResultTypes.HasValue || searchResultTypes.Value == SearchResultTypes.Applications)
                {
                    var applications = await _gizmoClient.UserApplicationsGetAsync(new UserApplicationsFilter());
                    var tmpApplications = applications.Data.Select(a => new AppViewState()
                    {
                        ApplicationId = a.Id,
                        ApplicationCategoryId = a.ApplicationCategoryId,
                        Title = a.Title,
                        Description = a.Description,
                        PublisherId = a.PublisherId,
                        ReleaseDate = a.ReleaseDate,
                        ImageId = a.ImageId
                    }).ToList();

                    var tmp = new List<GlobalSearchResultViewState>();

                    foreach (var app in tmpApplications.Where(a => a.Title.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        tmp.Add(new GlobalSearchResultViewState()
                        {
                            Type = SearchResultTypes.Applications,
                            Id = app.ApplicationId,
                            Name = app.Title,
                            ImageId = app.ImageId,
                            //TODO: A
                            Category = "Apps"
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
                            //TODO: A
                            Category = "Coffee"
                        });
                    }

                    ViewState.ProductResults = tmp;
                }
                //End Test

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
            //TODO: A THIS DOES NOT SEEM SAFE.
            if (_ignoreLocationChange)
            {
                _ignoreLocationChange = false;
                return;
            }

            await ClearResultsAsync();
            await CloseSearchAsync();
        }
    }
}
