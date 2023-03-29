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
    public sealed class CultureOutputService : ICultureService
    {
        private readonly ILocalizationService _localizationService;
        public CultureOutputService(ILocalizationService localizationService) => _localizationService = localizationService;

        public IEnumerable<CultureInfo> AveliableCultures => _localizationService.SupportedCultures;

        public Task SetCurrentCultureAsync(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            return Task.CompletedTask;
        }
        public CultureInfo GetCurrentCulture(string twoLetterISOLanguageName) =>
            AveliableCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName)
            ?? CultureInfo.CurrentUICulture;
    }
}
