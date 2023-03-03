using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class AdvertisementsViewState : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<AdvertisementViewState> _advertisements = Enumerable.Empty<AdvertisementViewState>();
        #endregion

        #region PROPERTIES

        public IEnumerable<AdvertisementViewState> Advertisements
        {
            get { return _advertisements; }
            internal set { _advertisements = value; }
        }

        #endregion
    }
}
