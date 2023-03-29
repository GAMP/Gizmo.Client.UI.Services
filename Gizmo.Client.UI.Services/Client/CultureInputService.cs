using System.Globalization;
using Gizmo.UI;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Default input culture service implementation.
    /// </summary>
    /// <remarks>
    /// This implementation should be used in hosts that dont support changing input language such as web browser.
    /// </remarks>
    public sealed class CultureInputService : ICultureService
    {
        private readonly ILocalizationService _localizationService;
        public CultureInputService(ILocalizationService localizationService) => _localizationService = localizationService;

        public IEnumerable<CultureInfo> AveliableCultures => _localizationService.SupportedCultures;

        public Task SetCurrentCultureAsync(CultureInfo culture)
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            return Task.CompletedTask;
        }
        public CultureInfo GetCurrentCulture(string? twoLetterISOLanguageName) =>
            AveliableCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName)
            ?? CultureInfo.CurrentCulture;
    }
}
