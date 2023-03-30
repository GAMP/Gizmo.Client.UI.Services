using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class SystemIdViewState : ViewStateBase
    {
        #region FIELDS
        private int _number;
        private string _name = string.Empty;
        #endregion

        #region PROPERTIES

        public int Number
        {
            get { return _number; }
            internal set { _number = value; }
        }

        public string Name
        {
            get { return _name; }
            internal set { _name = value; }
        }

        #endregion
    }
}
