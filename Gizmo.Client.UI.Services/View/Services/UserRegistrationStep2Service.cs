﻿using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationStep2Service : ValidatingViewStateServiceBase<UserRegistrationStep2ViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationStep2Service(UserRegistrationStep2ViewState viewState,
            ILogger<UserRegistrationMethodService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
        }
        #endregion

        public Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            NavigationService.NavigateTo("/registrationstep2");
            return Task.CompletedTask;
        }
    }
}
