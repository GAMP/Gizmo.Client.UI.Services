using System.Globalization;
using Gizmo.UI;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Default culture service implementaion.
    /// </summary>
    /// <remarks>
    /// This implementation should be used in hosts that dont need any extra culture handling for setting culture globally such as single thread blazor web assembly web hosts.
    /// </remarks>
    public sealed class CultureService : ICultureService
    {
        private readonly ILocalizationService _localizationService;
        public CultureService(ILocalizationService localizationService) => _localizationService = localizationService;

        public IEnumerable<CultureInfo> AveliableClientCultures => _localizationService.SupportedCultures;

        public Task SetCurrentUICultureAsync(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            return Task.CompletedTask;
        }
        public CultureInfo GetCurrentUICulture(IEnumerable<CultureInfo> cultures, string twoLetterISOLanguageName) =>
            cultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName)
            ?? CultureInfo.CurrentUICulture;
    }
}
