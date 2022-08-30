using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class AdvertisementsViewState : ViewStateBase
    {
        #region FIELDS
        private List<AdvertisementViewState> _advertisements = new();
        #endregion

        #region PROPERTIES

        public List<AdvertisementViewState> Advertisements
        {
            get { return _advertisements; }
            internal set { _advertisements = value; }
        }

        #endregion
    }
}
