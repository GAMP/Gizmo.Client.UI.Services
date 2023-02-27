﻿using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class ShopPageViewState : ViewStateBase
    {

        #region PROPERTIES

        public int? SelectedUserProductGroupId { get; internal set; }

        public IEnumerable<UserProductGroupViewState> UserProductGroups { get; internal set; } = Enumerable.Empty<UserProductGroupViewState>();

        public IEnumerable<ProductViewState> Products { get; internal set; } = Enumerable.Empty<ProductViewState>();

        #endregion
    }
}
