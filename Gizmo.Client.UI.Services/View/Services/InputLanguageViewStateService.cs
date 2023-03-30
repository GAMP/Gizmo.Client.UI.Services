using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services;

[Register]
public sealed class InputLanguageViewStateService : ViewStateServiceBase<InputLanguageViewState>
{
    #region CONSTRUCTOR
    public InputLanguageViewStateService(
        InputLanguageViewState viewState,
        IInputLanguageService inputLanguageService,
        ILogger<InputLanguageViewStateService> logger,
        IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
    {
        _inputLanguageService = inputLanguageService;
    }
    #endregion

    #region FIELDS
    private readonly IInputLanguageService _inputLanguageService;
    #endregion

    protected override async Task OnInitializing(CancellationToken ct)
    {
        ViewState.AvailableInputLaguages = _inputLanguageService.AvailableLaguages;

        ViewState.CurrentInputLanguage = _inputLanguageService.GetLanguage("en");

        if (ViewState.CurrentInputLanguage is not null)
            await _inputLanguageService.SetCurrentInputLanguageAsync(ViewState.CurrentInputLanguage);

        await base.OnInitializing(ct);
    }

    public async Task SetCurrentInputLanguageAsync(string twoLetterISOLanguageName)
    {
        ViewState.CurrentInputLanguage = _inputLanguageService.GetLanguage(twoLetterISOLanguageName);

        if (ViewState.CurrentInputLanguage is not null)
            await _inputLanguageService.SetCurrentInputLanguageAsync(ViewState.CurrentInputLanguage);

        ViewState.RaiseChanged();
    }
}
