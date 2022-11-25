using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class SearchService : ViewStateServiceBase<SearchViewState>
    {
        #region CONSTRUCTOR
        public SearchService(SearchViewState viewState,
            ILogger<SearchService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public Task LoadAllResultsAsync(SearchResultTypes searchResultTypes)
        {
            if (searchResultTypes == SearchResultTypes.Application)
            {
                NavigationService.NavigateTo(ClientRoutes.ApplicationsRoute);
            }
            else
            {
                NavigationService.NavigateTo(ClientRoutes.ShopRoute);
            }

            ViewState.ShowAll = true;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public Task LoadAllResultsLocallyAsync()
        {
            ViewState.ShowAllLocally = true;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public Task ClearResultsAsync()
        {
            ViewState.ShowAll = false;
            ViewState.ShowAllLocally = false;
            ViewState.IsLoading = false;
            ViewState.SearchPattern = string.Empty;
            ViewState.ApplicationResults.Clear();
            ViewState.ProductResults.Clear();

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public async Task SearchAsync(string searchPattern, SearchResultTypes? searchResultTypes = null)
        {
            ViewState.IsLoading = true;
            ViewState.SearchPattern = searchPattern;

            ViewState.RaiseChanged();

            await Task.Delay(500);

            Random random = new Random();

            if (!searchResultTypes.HasValue || searchResultTypes.Value == SearchResultTypes.Application)
            {
                var applications = await _gizmoClient.GetApplicationsAsync(new ApplicationsFilter());
                var tmpApplications = applications.Data.Select(a => new ApplicationViewState()
                {
                    Id = a.Id,
                    ApplicationCategoryId = a.ApplicationCategoryId,
                    Title = a.Title,
                    Description = a.Description,
                    PublisherId = a.PublisherId,
                    ReleaseDate = a.ReleaseDate,
                    //TODO: A
                    ImageId = null
                }).ToList();

                foreach (var app in tmpApplications.Where(a => a.Title.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ViewState.ApplicationResults.Add(new SearchResultViewState() { Type = SearchResultTypes.Application, Id = app.Id, Name = app.Title, ImageId = app.ImageId });
                }
            }

            if (!searchResultTypes.HasValue || searchResultTypes.Value == SearchResultTypes.Product)
            {
                var products = await _gizmoClient.GetProductsAsync(new ProductsFilter());
                var tmpProducts = products.Data.Select(a => new ProductViewState()
                {
                    Id = a.Id,
                    ProductGroupId = a.ProductGroupId,
                    Name = a.Name,
                    Description = a.Description,
                    ProductType = a.ProductType,
                    //TODO: A Get image.
                    ImageId = null
                }).ToList();

                foreach (var product in tmpProducts.Where(a => a.Name.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ViewState.ProductResults.Add(new SearchResultViewState() { Type = SearchResultTypes.Product, Id = product.Id, Name = product.Name, ImageId = product.ImageId });
                }
            }

            ViewState.IsLoading = false;

            ViewState.RaiseChanged();
        }

        #endregion

    }
}