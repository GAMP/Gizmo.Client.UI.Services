using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UsageSessionViewState : ViewStateBase
    {
        #region PROPERTIES

        public Web.Api.Models.UsageType CurrentTimeProductType { get; internal set; } = Web.Api.Models.UsageType.None;

        public string CurrentTimeProductName { get; internal set; } = string.Empty;

        #endregion
    }
}
