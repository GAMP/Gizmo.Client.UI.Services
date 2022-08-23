using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class ApplicationViewState : ViewStateBase
    {
        #region FIELDS
        private int _id;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _image = string.Empty;
        private int _ratings;
        private decimal _rate;
        private DateTime? _releaseDate;
        private DateTime _dateAdded;

        private List<ExecutableViewState> _executables = new();
        #endregion

        #region PROPERTIES

        public int Id
        {
            get { return _id; }
            internal set { SetProperty(ref _id, value); }
        }

        public string Title
        {
            get { return _name; }
            internal set { SetProperty(ref _name, value); }
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

        public int Ratings
        {
            get { return _ratings; }
            internal set { _ratings = value; }
        }

        public decimal Rate
        {
            get { return _rate; }
            internal set { _rate = value; }
        }

        public DateTime? ReleaseDate
        {
            get { return _releaseDate; }
            internal set { _releaseDate = value; }
        }

        public DateTime DateAdded
        {
            get { return _dateAdded; }
            internal set { _dateAdded = value; }
        }

        public List<ExecutableViewState> Executables
        {
            get { return _executables; }
            internal set { _executables = value; }
        }

        #endregion
    }
}
