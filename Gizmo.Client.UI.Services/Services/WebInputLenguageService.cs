using System.Globalization;

using Gizmo.UI;

namespace Gizmo.Client.UI.Services
{
    public sealed class WebInputLenguageService : IInputLanguageService
    {

        public IEnumerable<CultureInfo> AvailableInputLanguages => Enumerable.Empty<CultureInfo>();


        public Task SetCurrentInputLanguageAsync(CultureInfo culture)
        {
            return Task.CompletedTask;
        }

        public CultureInfo GetLanguage(string twoLetterISOLanguageName)
        {
            return new CultureInfo(twoLetterISOLanguageName);
        }
    }
}
