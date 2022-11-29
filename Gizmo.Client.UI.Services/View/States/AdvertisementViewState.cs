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
        private string _body = string.Empty;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private string _url = string.Empty;
        private string _videoUrl = string.Empty;
        private string _thumbnailUrl = string.Empty;
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

        public string Body
        {
            get { return _body; }
            internal set { SetProperty(ref _body, value); }
        }

        public DateTime? StartDate
        {
            get { return _startDate; }
            internal set { SetProperty(ref _startDate, value); }
        }

        public DateTime? EndDate
        {
            get { return _endDate; }
            internal set { SetProperty(ref _endDate, value); }
        }

        public string Url
        {
            get { return _url; }
            internal set { SetProperty(ref _url, value); }
        }

        public string VideoUrl
        {
            get { return _videoUrl; }
            internal set { SetProperty(ref _videoUrl, value); }
        }

        public string ThumbnailUrl
        {
            get { return _thumbnailUrl; }
            internal set { SetProperty(ref _thumbnailUrl, value); }
        }

        #endregion
    }
}
