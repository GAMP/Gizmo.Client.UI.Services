using System.Globalization;
using Gizmo.UI;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Default output culture service implementation.
    /// </summary>
    /// <remarks>
    /// This implementation should be used in hosts that dont need any extra culture handling for setting culture globally such as single thread blazor web assembly web hosts.
    /// </remarks>
    public sealed class CultureOutputService : ICultureOutputService
    {
        private readonly ILocalizationService _localizationService;
        public CultureOutputService(ILocalizationService localizationService) => _localizationService = localizationService;

        public IEnumerable<CultureInfo> AvailableCultures => 
            _localizationService.SupportedCultures
                .DistinctBy(x => x.TwoLetterISOLanguageName)
                .Select(x => new CultureInfo(x.Name));

        public Task SetCurrentCultureAsync(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            return Task.CompletedTask;
        }
        public CultureInfo GetCulture(IEnumerable<CultureInfo> cultures, string twoLetterISOLanguageName) =>
            cultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName)
            ?? CultureInfo.CurrentUICulture;
    }
}
