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
        /// Gets current culture.
        /// </summary>
        public CultureInfo CurrentCulture { get; internal set; } = null!;

        #endregion
    }
}
