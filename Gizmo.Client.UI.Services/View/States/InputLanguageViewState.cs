using System.Globalization;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class InputLanguageViewState : ViewStateBase
    {
        #region PROPERTIES

        /// <summary>
        /// Gets aveliable cultures.
        /// </summary>
        public IEnumerable<CultureInfo> AvailableInputLanguages { get; internal set; } = Enumerable.Empty<CultureInfo>();

        /// <summary>
        /// Gets current culture.
        /// </summary>
        public CultureInfo? CurrentInputLanguage { get; internal set; }

        #endregion
    }
}
