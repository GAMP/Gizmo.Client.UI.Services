using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserRegistrationRedirectViewState : ViewStateBase
    {
        #region PROPERTIES

        public string? RedirectUrl { get; internal set; }

        public string? QrCode { get; internal set; }

        public bool IsQrExpired { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
