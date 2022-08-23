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
        private bool _installing;
        private decimal _installingPercentage;
        #endregion

        #region PROPERTIES

        public int Id
        {
            get { return _id; }
            internal set { SetProperty(ref _id, value); }        
        }

        public string Name
        {
            get { return _name; }
            internal set { SetProperty(ref _name, value); }
        }

        public string Image
        {
            get { return _image; }
            internal set { SetProperty(ref _image, value); }
        }

        public bool Installing
        {
            get { return _installing; }
            internal set { SetProperty(ref _installing, value); }
        }

        public decimal InstallingPercentage
        {
            get { return _installingPercentage; }
            internal set { SetProperty(ref _installingPercentage, value); }
        }

        #endregion
    }
}
