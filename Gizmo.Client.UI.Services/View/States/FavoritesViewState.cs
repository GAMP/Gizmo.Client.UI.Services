using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class FavoritesViewState : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<AppExeViewState> _executables = Enumerable.Empty<AppExeViewState>();
        #endregion

        #region PROPERTIES

        public IEnumerable<AppExeViewState> Executables
        {
            get { return _executables; }
            internal set { _executables = value; }
        }

        #endregion
    }
}
