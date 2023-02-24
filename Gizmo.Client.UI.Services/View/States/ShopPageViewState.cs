﻿using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class ShopPageViewState : ViewStateBase
    {

        #region PROPERTIES

        public int? SelectedProductGroupId { get; internal set; }

        public IEnumerable<UserProductGroupViewState> ProductGroups { get; internal set; } = Enumerable.Empty<UserProductGroupViewState>();

        public List<ProductViewState> Products { get; internal set; } = new();

        #endregion
    }
}
