﻿using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services;

[Register]
public sealed class CultureInputViewStateService : ViewStateServiceBase<CultureInputViewState>
{
    #region CONSTRUCTOR
    public CultureInputViewStateService(
        CultureInputViewState viewState,
        ICultureInputService cultureService,
        ILogger<CultureInputViewStateService> logger,
        IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
    {
        _cultureService = cultureService;
    }
    #endregion

    #region FIELDS
    private readonly ICultureInputService _cultureService;
    #endregion

    protected override async Task OnInitializing(CancellationToken ct)
    {
        ViewState.AvailableCultures = _cultureService.AvailableCultures.ToList();

        ViewState.CurrentCulture = _cultureService.GetCulture(ViewState.AvailableCultures, "ru");

        await _cultureService.SetCurrentCultureAsync(ViewState.CurrentCulture);

        await base.OnInitializing(ct);
    }

    public async Task SetCurrentInputCultureAsync(string twoLetterISOLanguageName)
    {
        ViewState.CurrentCulture = _cultureService.GetCulture(ViewState.AvailableCultures, twoLetterISOLanguageName);

        await _cultureService.SetCurrentCultureAsync(ViewState.CurrentCulture);

        ViewState.RaiseChanged();
    }
}
