using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AdvertisementViewState : ViewStateBase
    {
        #region FIELDS
        private int _id;
        private int _type;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _image = string.Empty;
        #endregion

        #region PROPERTIES

        public int Id
        {
            get { return _id; }
            internal set { SetProperty(ref _id, value); }
        }

        public int Type
        {
            get { return _type; }
            internal set { SetProperty(ref _type, value); }
        }

        public string Title
        {
            get { return _title; }
            internal set { SetProperty(ref _title, value); }
        }

        public string Description
        {
            get { return _description; }
            internal set { SetProperty(ref _description, value); }
        }

        public string Image
        {
            get { return _image; }
            internal set { SetProperty(ref _image, value); }
        }

        #endregion
    }
}
