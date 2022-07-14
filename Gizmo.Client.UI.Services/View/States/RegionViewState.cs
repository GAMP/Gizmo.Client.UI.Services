using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public class RegionViewState : ViewStateBase
    {
        #region FIELDS
        private string _englishName = DefaultValues.EMPTY_STRING_VALUE;
        private string _twoLetterISORegionName = DefaultValues.EMPTY_STRING_VALUE;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets country name.
        /// </summary>
        public string EnglishName
        {
            get { return _englishName; }
            internal set { SetProperty(ref _englishName ,value); }
        } 

        /// <summary>
        /// Gets two letter iso region name.
        /// </summary>
        public string TwoLetterISORegionName
        {
            get { return _twoLetterISORegionName; }
            internal set { SetProperty(ref _twoLetterISORegionName, value); }
        }

        #endregion
    }
}
