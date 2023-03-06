using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class SearchResultViewState : ViewStateBase
    {
        #region FIELDS
        private SearchResultTypes _type;
        private int _id;
        private string _category = string.Empty;
        private string _name = string.Empty;
        private int? _image;
        #endregion

        #region PROPERTIES

        public SearchResultTypes Type
        {
            get { return _type; }
            internal set { _type = value; }
        }

        public int Id
        {
            get { return _id; }
            internal set { _id = value; }
        }

        public string Category
        {
            get { return _category; }
            internal set { _category = value; }
        }

        public string Name
        {
            get { return _name; }
            internal set { _name = value; }
        }

        public int? ImageId
        {
            get { return _image; }
            internal set { _image = value; }
        }

        #endregion
    }
}
