using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class ExecutableViewState : ViewStateBase
    {
        #region FIELDS
        private int _id;
        private string _caption = string.Empty;
        private int? _image;
        private ExecutableState _state;
        private decimal _statePercentage;

        private IEnumerable<string> _personalFiles = Enumerable.Empty<string>();
        #endregion

        #region PROPERTIES

        public int Id
        {
            get { return _id; }
            internal set { SetProperty(ref _id, value); }        
        }

        public string Caption
        {
            get { return _caption; }
            internal set { SetProperty(ref _caption, value); }
        }

        public int? ImageId
        {
            get { return _image; }
            internal set { SetProperty(ref _image, value); }
        }

        public ExecutableState State
        {
            get { return _state; }
            internal set { SetProperty(ref _state, value); }
        }

        public decimal StatePercentage
        {
            get { return _statePercentage; }
            internal set { SetProperty(ref _statePercentage, value); }
        }

        public IEnumerable<string> PersonalFiles
        {
            get { return _personalFiles; }
            internal set { _personalFiles = value; }
        }

        #endregion
    }
}
