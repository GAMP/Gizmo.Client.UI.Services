using Gizmo.Client.UI.Services;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserRegistrationProvidersViewState : ViewStateBase
    {
        #region PROPERTIES

        public IReadOnlyList<RegistrationProvider> Providers { get; internal set; } = [];

        public IReadOnlyList<RegistrationProvider> PriorityProviders =>
            Providers.Where(p => p.Priority).ToList();

        public IReadOnlyList<RegistrationProvider> AltProviders =>
            Providers.Where(p => !p.Priority).ToList();

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        public Guid? FailedChannelGuid { get; internal set; }

        public bool ShowAllProviders { get; internal set; }

        #endregion
    }
}
