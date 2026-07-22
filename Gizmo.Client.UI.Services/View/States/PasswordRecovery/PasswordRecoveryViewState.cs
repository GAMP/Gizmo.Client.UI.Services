using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using Gizmo.Client.UI.Services;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class PasswordRecoveryViewState : ValidatingViewStateBase
    {
        public IReadOnlyList<PasswordRecoveryProvider> AvailableProviders { get; internal set; } = Array.Empty<PasswordRecoveryProvider>();

        public Guid? ActiveProviderPublicId { get; internal set; }

        public PasswordRecoveryChannel Channel { get; internal set; }

        [ValidatingProperty]
        public string MatchValue { get; internal set; } = string.Empty;

        [ValidatingProperty]
        public string? Country { get; internal set; }

        [ValidatingProperty]
        public string? RegionCode { get; internal set; }

        [ValidatingProperty(IsAsync = true)]
        public string? MobilePhone { get; internal set; }

        public string? PhoneE164 { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;
    }
}
