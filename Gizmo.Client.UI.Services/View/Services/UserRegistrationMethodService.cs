﻿using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationMethodService : ViewStateServiceBase<UserRegistrationConfirmationMethodViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationMethodService(UserRegistrationConfirmationMethodViewState viewState,
            ILogger<UserRegistrationMethodService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
        }   
        #endregion

        public Task SetMethodAsync(UserRegistrationMethod method)
        {
            ViewState.ConfirmationMethod = method;
            return Task.CompletedTask;
        }

    }
}
