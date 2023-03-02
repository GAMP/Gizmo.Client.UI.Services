using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class SearchService : ViewStateServiceBase<SearchViewState>
    {
        #region FIELDS
        private readonly UserProductViewStateLookupService _userProductStateLookupService;
        private readonly IGizmoClient _gizmoClient;
        private bool _ignoreLocationChange = false;
        #endregion

        #region CONSTRUCTOR
        public SearchService(
            SearchViewState viewState,
            UserProductViewStateLookupService userProductStateLookupService,
            ILogger<SearchService> logger,
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
            ViewState.ProductResults.Clear();
            ViewState.ApplicationResults.Clear();

            ViewState.OpenDropDown = false;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public async Task ViewAllResultsAsync(SearchResultTypes searchResultTypes)
        {
            _ignoreLocationChange = true;

            ViewState.AppliedSearchPattern = ViewState.SearchPattern;

            if (searchResultTypes == SearchResultTypes.Applications)
            {
                NavigationService.NavigateTo(ClientRoutes.ApplicationsRoute);
            }
            else
            {
                NavigationService.NavigateTo(ClientRoutes.ShopRoute);
            }

            ViewState.AppliedApplicationResults.Clear();
            ViewState.AppliedApplicationResults.AddRange(ViewState.ApplicationResults);
            ViewState.AppliedProductResults.Clear();
            ViewState.AppliedProductResults.AddRange(ViewState.ProductResults);

            ViewState.ShowAll = true;

            ViewState.RaiseChanged();

            await CloseSearchAsync();

            //_ignoreLocationChange = false;
        }

        public async Task ProcessEnterAsync()
        {
            if (ViewState.ApplicationResults.Count > 0 && ViewState.ProductResults.Count > 0)
            {
                //Found results on both categories. Do nothing.
            }
            else
            {
                if (ViewState.ApplicationResults.Count > 0)
                {
                    //Results found only in applications.
                    if (ViewState.ApplicationResults.Count == 1)
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

                if (ViewState.ProductResults.Count > 0)
                {
                    //Results found only in applications.
                    if (ViewState.ProductResults.Count == 1)
                    {
                        //Only one result execute action.
                        var userCartService = ServiceProvider.GetRequiredService<UserCartService>();
                        await userCartService.AddUserCartProductAsync(ViewState.ProductResults[0].Id);
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
            ViewState.ProductResults.Clear();
            ViewState.ApplicationResults.Clear();

            //Clear applied search.
            ViewState.ShowAll = false;

            ViewState.AppliedSearchPattern = string.Empty;
            ViewState.AppliedApplicationResults.Clear();
            ViewState.AppliedProductResults.Clear();

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public async Task SearchAsync(SearchResultTypes? searchResultTypes = null)
        {
            ViewState.ProductResults.Clear();
            ViewState.ApplicationResults.Clear();

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
                    var applications = await _gizmoClient.ApplicationsGetAsync(new ApplicationsFilter());
                    var tmpApplications = applications.Data.Select(a => new ApplicationViewState()
                    {
                        Id = a.Id,
                        ApplicationGroupId = a.ApplicationCategoryId,
                        Title = a.Title,
                        Description = a.Description,
                        PublisherId = a.PublisherId,
                        ReleaseDate = a.ReleaseDate,
                        //TODO: A
                        ImageId = null,
                        ApplicationGroupName = "Shooter"
                    }).ToList();

                    foreach (var app in tmpApplications.Where(a => a.Title.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ViewState.ApplicationResults.Add(new SearchResultViewState() { Type = SearchResultTypes.Applications, Id = app.Id, Name = app.Title, ImageId = app.ImageId });
                    }
                }

                if (!searchResultTypes.HasValue || searchResultTypes.Value == SearchResultTypes.Products)
                {
                    var productStates = await _userProductStateLookupService.GetStatesAsync();

                    foreach (var product in productStates.Where(a => a.Name.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ViewState.ProductResults.Add(new SearchResultViewState()
                        {
                            Type = SearchResultTypes.Products,
                            Id = product.Id,
                            Name = product.Name,
                            ImageId = product.ImageId
                        });
                    }
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
