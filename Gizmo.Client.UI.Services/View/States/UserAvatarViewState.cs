using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserAvatarViewState : ViewStateBase
    {
        #region FIELDS
        private string? _avatar;
        #endregion

        #region PROPERTIES
        
        /// <summary>
        /// Gets user avatar.
        /// </summary>
        public string? Avatar
        {
            get { return _avatar; }
            internal set { _avatar = value; }
        } 

        #endregion
    }
}
