﻿using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ActiveApplicationsService : ViewStateServiceBase<ActiveApplicationsViewState>
    {
        #region CONSTRUCTOR
        public ActiveApplicationsService(ActiveApplicationsViewState viewState,
            ILogger<ActiveApplicationsService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion
    }
}
