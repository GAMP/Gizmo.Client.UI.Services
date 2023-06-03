using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    [Route(ClientRoutes.HomeRoute)]
    [Route(ClientRoutes.ApplicationsRoute)]
    [Route(ClientRoutes.ShopRoute)]
    public sealed class AdvertisementsViewService : ViewStateServiceBase<AdvertisementsViewState>
    {
        #region CONSTRUCTOR
        public AdvertisementsViewService(
            ILogger<AdvertisementsViewService> logger,
            IServiceProvider serviceProvider,
            IClientDialogService dialogService,
            AdvertisementsViewState viewState,
            AdvertisementViewStateLookupService advertisementViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _dialogService = dialogService;
            _advertisementViewStateLookupService = advertisementViewStateLookupService;
        }

        #endregion

        #region FIELDS
        private readonly IClientDialogService _dialogService;
        private readonly AdvertisementViewStateLookupService _advertisementViewStateLookupService;
        #endregion

        #region FUNCTIONS

        public void SetCollapsed(bool value)
        {
            ViewState.IsCollapsed = value;
            DebounceViewStateChanged();
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _advertisementViewStateLookupService.Changed += _advertisementViewStateLookupService_Changed;
            return base.OnInitializing(ct);
        }

        private async void _advertisementViewStateLookupService_Changed(object? sender, LookupServiceChangeArgs e)
        {
            await LoadAdvertisementsAsync();
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _advertisementViewStateLookupService.Changed -= _advertisementViewStateLookupService_Changed;
            base.OnDisposing(isDisposing);
        }

        public async Task ShowMediaSync(AdvertisementViewState advertisementViewState)
        {
            var dialog = await _dialogService.ShowMediaDialogAsync(new MediaDialogParameters()
            {
                Title = advertisementViewState.Title,
                MediaUrlType = advertisementViewState.MediaUrlType,
                MediaUrl = advertisementViewState.MediaUrl
            });
            if (dialog.Result == AddComponentResultCode.Opened)
                _ = await dialog.WaitForResultAsync();
        }
        public async Task<AdvertisementViewState> GetAdvertisementViewStateAsync(int id)
        {
            return await _advertisementViewStateLookupService.GetStateAsync(id);
        }
        public async Task LoadAdvertisementsAsync(CancellationToken cToken = default)
        {
            ViewState.Advertisements = await _advertisementViewStateLookupService.GetStatesAsync(cToken);
            ViewState.RaiseChanged();
        }
        private async void OnLoadAdvertisementsAsync(object? _, EventArgs __) =>
            await LoadAdvertisementsAsync();

        #endregion

        #region OVERRIDES
        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            _advertisementViewStateLookupService.Changed += OnLoadAdvertisementsAsync;

            if (navigationParameters.IsInitial)
                await LoadAdvertisementsAsync(cToken);
        }
        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            _advertisementViewStateLookupService.Changed -= OnLoadAdvertisementsAsync;
            return base.OnNavigatedOut(navigationParameters, cToken);
        }
        #endregion
    }
}
