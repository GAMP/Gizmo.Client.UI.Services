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
            internal set { SetProperty(ref _number, value); }
        }

        public string Name
        {
            get { return _name; }
            internal set { SetProperty(ref _name, value); }
        }

        #endregion
    }
}
