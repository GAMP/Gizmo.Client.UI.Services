using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserPasswordRecoveryViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        public UserRecoveryMethod SelectedRecoveryMethod { get; internal set; }

        [ValidatingProperty()]
        public string MobilePhone { get; internal set; } = string.Empty;

        [ValidatingProperty()]
        public string Email { get; internal set; } = string.Empty;

        public string Destination { get; internal set; } = string.Empty;

        public string Token { get; internal set; } = string.Empty;

        public int CodeLength { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
