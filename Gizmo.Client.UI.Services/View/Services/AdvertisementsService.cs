﻿using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class AdvertisementsService : ViewStateServiceBase<AdvertisementsViewState>
    {
        #region CONSTRUCTOR
        public AdvertisementsService(
            ILogger<AdvertisementsService> logger,
            IServiceProvider serviceProvider,
            AdvertisementsViewState viewState,
            AdvertisementViewStateLookupService advertisementViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _advertisementViewStateLookupService = advertisementViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly AdvertisementViewStateLookupService _advertisementViewStateLookupService;
        #endregion

        #region FUNCTIONS

        public async Task<AdvertisementViewState> GetAdvertisementViewStateAsync(int id)
        {
            return await _advertisementViewStateLookupService.GetStateAsync(id);
        }

        public async Task LoadAdvertisementsAsync(CancellationToken cToken = default)
        {
            ViewState.Advertisements = await _advertisementViewStateLookupService.GetStatesAsync(cToken);
            ViewState.AdvertisementsCount = ViewState.Advertisements.Count();
            ViewState.RaiseChanged();
        }

        private async void OnLoadAdvertisementsAsync(object? _, EventArgs __) =>
            await LoadAdvertisementsAsync();

        #endregion

        #region OVERRIDES
        protected override async Task OnInitializing(CancellationToken cToken)
        {
            await base.OnInitializing(cToken);

            await LoadAdvertisementsAsync(cToken);

            _advertisementViewStateLookupService.Changed += OnLoadAdvertisementsAsync;
        }

        protected override void OnDisposing(bool isDisposing)
        {
            if (isDisposing)
                _advertisementViewStateLookupService.Changed -= OnLoadAdvertisementsAsync;

            base.OnDisposing(isDisposing);
        }
        #endregion
    }
}
