using System.Globalization;

using Gizmo.UI;

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
        public IEnumerable<CultureInfo> AvailableCultures => Enumerable.Empty<CultureInfo>();

        public Task SetCurrentCultureAsync(CultureInfo culture)
        {
            return Task.CompletedTask;
        }
        public CultureInfo GetCulture(IEnumerable<CultureInfo> cultures, string? twoLetterISOLanguageName) =>
            cultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName)
            ?? CultureInfo.CurrentCulture;
    }
}
