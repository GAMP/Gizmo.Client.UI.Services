using System.Collections.Generic;
using System;
using System.Linq;
using Gizmo.Client.UI.Services;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class PasswordRecoveryViewState : ViewStateBase
    {
        public IReadOnlyList<PasswordRecoveryProvider> Providers { get; internal set; } = Array.Empty<PasswordRecoveryProvider>();

        public IReadOnlyList<PasswordRecoveryProvider> PriorityProviders =>
            Providers.Where(p => p.IsPrimary).ToList();

        public IReadOnlyList<PasswordRecoveryProvider> AltProviders =>
            Providers.Where(p => !p.IsPrimary).ToList();

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        public bool ShowAllProviders { get; internal set; }
    }
}
