using System.Globalization;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Default input culture service implementation.
    /// </summary>
    /// <remarks>
    /// This implementation should be used in hosts that dont support changing input language such as web browser.
    /// </remarks>
    public sealed class CultureInputService : ICultureInputService
    {
        private readonly ILocalizationService _localizationService;
        public CultureInputService(ILocalizationService localizationService) => _localizationService = localizationService;

        public IEnumerable<CultureInfo> AvailableInputCultures => _localizationService.SupportedCultures;

        public Task SetCurrentInputCultureAsync(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            return Task.CompletedTask;
        }

        public CultureInfo GetCurrentInputCulture(IEnumerable<CultureInfo> cultures, string? twoLetterISOLanguageName) =>
            cultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName)
            ?? CultureInfo.CurrentCulture;
    }
}
