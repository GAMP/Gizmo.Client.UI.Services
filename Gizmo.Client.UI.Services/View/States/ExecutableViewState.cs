using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class ExecutableViewState : ViewStateBase
    {
        #region FIELDS
        private int _id;
        private string _name = string.Empty;
        private string _image = string.Empty;
        private int _state;
        private decimal _statePercentage;
        #endregion

        #region PROPERTIES

        public int Id
        {
            get { return _id; }
            internal set { SetProperty(ref _id, value); }        
        }

        public string Caption
        {
            get { return _name; }
            internal set { SetProperty(ref _name, value); }
        }

        public string Image
        {
            get { return _image; }
            internal set { SetProperty(ref _image, value); }
        }

        public int State
        {
            get { return _state; }
            internal set { SetProperty(ref _state, value); }
        }

        public decimal StatePercentage
        {
            get { return _statePercentage; }
            internal set { SetProperty(ref _statePercentage, value); }
        }

        #endregion
    }
}
