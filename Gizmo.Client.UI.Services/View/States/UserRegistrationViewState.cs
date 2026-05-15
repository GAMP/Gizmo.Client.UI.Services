using Gizmo.Client.UI.Services;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        [Obsolete("Replaced by SelectedProvider. Remove when separate pages per method implemented.")]
        public RegistrationVerificationMethod ConfirmationMethod { get; internal set; } = RegistrationVerificationMethod.None;

        public RegistrationProvider? SelectedProvider { get; internal set; }

        public RegistrationRequiredInfo? DefaultUserGroupRequiredInfo { get; internal set; }

        #endregion
    }
}
