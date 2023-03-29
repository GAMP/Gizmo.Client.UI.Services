using Gizmo.Client.UI.View.States;
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
        ViewState.AveliableCultures = _cultureService.AvailableInputCultures;

        ViewState.CurrentCulture = _cultureService.GetCurrentInputCulture(ViewState.AveliableCultures, "en");

        await _cultureService.SetCurrentInputCultureAsync(ViewState.CurrentCulture);

        await base.OnInitializing(ct);
    }

    public async Task SetCurrentInputCultureAsync(string twoLetterISOLanguageName)
    {
        ViewState.CurrentCulture = _cultureService.GetCurrentInputCulture(ViewState.AveliableCultures, twoLetterISOLanguageName);

        await _cultureService.SetCurrentInputCultureAsync(ViewState.CurrentCulture);

        ViewState.RaiseChanged();
    }
}
