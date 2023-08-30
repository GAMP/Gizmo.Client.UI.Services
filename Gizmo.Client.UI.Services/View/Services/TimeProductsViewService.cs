﻿using System;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.UserProductsRoute)]
    public sealed class TimeProductsViewService : ViewStateServiceBase<TimeProductsViewState>
    {
        #region CONSTRUCTOR
        public TimeProductsViewService(TimeProductsViewState viewState,
            ILogger<TimeProductsViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService,
            UserProductViewStateLookupService userProductStateLookupService,
            UserHostGroupViewStateLookupService userHostGroupViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _userProductStateLookupService = userProductStateLookupService;
            _userHostGroupViewStateLookupService = userHostGroupViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        private readonly UserProductViewStateLookupService _userProductStateLookupService;
        private readonly UserHostGroupViewStateLookupService _userHostGroupViewStateLookupService;
        #endregion

        #region FUNCTIONS

        private async Task<List<TimeProductViewState>> TransformResults(List<UserUsageTimeLevelModel> timeProducts, CancellationToken cToken = default)
        {
            var timeProductsViewStates = new List<TimeProductViewState>();

            foreach (var timeProduct in timeProducts)
            {
                var timeProductViewState = new TimeProductViewState();

                timeProductViewState.TimeProductType = timeProduct.UsageType;

                switch (timeProductViewState.TimeProductType)
                {
                    case UsageType.Rate:

                        if (timeProduct.Rate != null)
                        {
                            timeProductViewState.TimeProductName = $"{_localizationService.GetString("GIZ_USAGE_TYPE_RATE")} {timeProduct.Rate.HourlyRate.ToString("C")}/hour";
                        }
                        else
                        {
                            timeProductViewState.TimeProductName = _localizationService.GetString("GIZ_USAGE_TYPE_RATE");
                        }
                        break;

                    case UsageType.TimeFixed:

                        timeProductViewState.TimeProductName = "Time Fixed"; //TODO: AAA NOT EXISTS IN MODEL; ProductId
                        break;

                    case UsageType.TimeOffer:

                        if (timeProduct.TimeOffer != null)
                        {
                            var product = await _userProductStateLookupService.GetStateAsync(timeProduct.TimeOffer.ProductId, false, cToken);

                            timeProductViewState.TimeProductName = product.Name;

                            var hostGroups = await _userHostGroupViewStateLookupService.GetStatesAsync(cToken);

                            timeProductViewState.AvailableHostGroups = hostGroups.Where(a => !product.TimeProduct.DisallowedHostGroups.Contains(a.Id)).Select(a => a.Id);
                        }

                        break;
                }

                timeProductViewState.ActivationOrder = timeProduct.ActivationOrder;
                timeProductViewState.PurchaseDate = DateTime.Now; //TODO: AAA NOT EXISTS IN MODEL; InvoiceLineId
                timeProductViewState.Source = $"Test"; //TODO: AAA NOT EXISTS IN MODEL; InvoiceLineId

                if (timeProduct.AvailableMinutes.HasValue)
                {
                    timeProductViewState.Time = TimeSpan.FromMinutes(timeProduct.AvailableMinutes.Value);
                }

                timeProductsViewStates.Add(timeProductViewState);
            }

            return timeProductsViewStates;
        }

        public async Task LoadAsync(CancellationToken cToken = default)
        {
            List<UserUsageTimeLevelModel> timeProductsList = await _gizmoClient.UserUsageTimeLevelsGetAsync(cToken);
            var userTimeProductsViewStates = await TransformResults(timeProductsList);

            ViewState.TimeProducts = userTimeProductsViewStates;

            ViewState.RaiseChanged();
        }

        #endregion

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            await LoadAsync(cToken);
        }
    }
}
