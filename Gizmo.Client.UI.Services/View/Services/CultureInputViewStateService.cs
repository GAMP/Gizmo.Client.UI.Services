using Gizmo.Client.UI.Services;
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
        CultureInputService cultureService,
        ILogger<CultureInputViewStateService> logger,
        IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
    {
        _cultureService = cultureService;
    }
    #endregion

    #region FIELDS
    private readonly CultureInputService _cultureService;
    #endregion

    protected override async Task OnInitializing(CancellationToken ct)
    {
        ViewState.AveliableCultures = _cultureService.AveliableCultures;

        ViewState.CurrentCulture = _cultureService.GetCurrentCulture("ru");

        await _cultureService.SetCurrentCultureAsync(ViewState.CurrentCulture);

        await base.OnInitializing(ct);
    }

    public async Task SetCurrentInputCultureAsync(string twoLetterISOLanguageName)
    {
        ViewState.CurrentCulture = _cultureService.GetCurrentCulture(twoLetterISOLanguageName);

        await _cultureService.SetCurrentCultureAsync(ViewState.CurrentCulture);

        ViewState.RaiseChanged();
    }
}
