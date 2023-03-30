using System.Globalization;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class ClientLocalizationViewState : ViewStateBase
    {
        #region PROPERTIES

        /// <summary>
        /// Gets aveliable cultures.
        /// </summary>
        public IEnumerable<CultureInfo> AvailableCultures { get; internal set; } = Enumerable.Empty<CultureInfo>();

        /// <summary>
        /// Gets current culture.
        /// </summary>
        public CultureInfo CurrentCulture { get; internal set; } = null!;

        #endregion
    }
}
