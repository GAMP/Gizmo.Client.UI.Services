using System.Globalization;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Default input language service implementation.
    /// </summary>
    /// <remarks>
    /// This implementation should be used in hosts that dont support changing input language such as web browser.
    /// </remarks>
    public sealed class InputLanguagesService : IInputLanguageService
    {
        public IEnumerable<CultureInfo> AvailableInputLanguages => Enumerable.Empty<CultureInfo>();

        public CultureInfo CurrentInputLanguage { get => CultureInfo.InvariantCulture; set => _ = value; }

        public Task SetCurrentLanguageAsync(CultureInfo inputLanguage)
        {
            return Task.CompletedTask;
        }
    }
}
