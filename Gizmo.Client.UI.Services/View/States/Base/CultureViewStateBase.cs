using System.Globalization;
using Gizmo.UI.View.States;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Culture view state base.
    /// </summary>
    public abstract class CultureViewStateBase : ViewStateBase
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
