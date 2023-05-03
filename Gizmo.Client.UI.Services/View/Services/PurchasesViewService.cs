﻿using System;
using System.Linq;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.UserPurchasesRoute)]
    public sealed class PurchasesViewService : ViewStateServiceBase<PurchasesViewState>
    {
        #region CONSTRUCTOR
        public PurchasesViewService(PurchasesViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<PurchasesViewService> logger,
            IServiceProvider serviceProvider,
            UserProductViewStateLookupService userProductViewStateLookupService,
            PaymentMethodViewStateLookupService paymentMethodViewStateLookupService
            ) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _userProductViewStateLookupService = userProductViewStateLookupService;
            _paymentMethodViewStateLookupService = paymentMethodViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly UserProductViewStateLookupService _userProductViewStateLookupService;
        private readonly PaymentMethodViewStateLookupService _paymentMethodViewStateLookupService;
        #endregion

        #region FUNCTIONS

        public async Task LoadPrevious()
        {
            if (ViewState.PrevCursor != null)
                await LoadCursor(ViewState.PrevCursor, true);
        }

        public async Task LoadNext()
        {
            if (ViewState.NextCursor != null)
                await LoadCursor(ViewState.NextCursor, false);
        }

        public async Task LoadCursor(PaginationCursor? cursor, bool prev, CancellationToken cToken = default)
        {
            var filters = new Web.Api.Models.UserOrdersFilter();
            filters.Pagination.Limit = 4;

            filters.Pagination.Cursor = new Web.Api.Models.PaginationCursor()
            {
                Name = "CreatedTime" //Sord by CreatedTime descending. //TODO: AAA CHANGE TO MODEL FIELD NAME.
            };

            if (cursor == null)
            {
                //Initial load.
                filters.Pagination.Cursor.IsForward = false; //THIS IS HOW WE SET DESCENDING NOW.
            }
            else
            {
                //Navigation by buttons.
                if (!prev)
                {
                    //GET NEXT PAGE
                    filters.Pagination.Cursor.Id = cursor.Id;
                    filters.Pagination.Cursor.Value = cursor.Value;
                    filters.Pagination.Cursor.IsForward = false; //WE SET DESCENDING TO LOAD THE NEXT PAGE.
                }
                else
                {
                    //GET PREVIOUS PAGE
                    filters.Pagination.Cursor.Id = cursor.Id;
                    filters.Pagination.Cursor.Value = cursor.Value;
                    filters.Pagination.Cursor.IsForward = true; //WE SET ASCENDING TO LOAD THE PREVIOUS PAGE.
                }
            }

            var ordersList = await _gizmoClient.UserOrdersGetAsync(filters, cToken);

            var userOrderViewStates = new List<UserOrderViewState>();

            foreach (var order in ordersList.Data)
            {
                var userOrderViewState = new UserOrderViewState();

                userOrderViewState.OrderDate = order.Date;
                userOrderViewState.OrderStatus = order.Status;
                userOrderViewState.TotalPrice = order.Total;
                userOrderViewState.TotalPointsPrice = order.PointsTotal;
                userOrderViewState.TotalPointsAward = order.PointsAwardTotal;
                userOrderViewState.Notes = order.UserNote;

                if (order.Invoice != null)
                {
                    userOrderViewState.Invoice = new UserOrderInvoiceViewState();

                    userOrderViewState.Invoice.PaymentStatus = order.Invoice.Status;
                    userOrderViewState.Invoice.IsVoided = order.Invoice.IsVoided;

                    List<string> paymentMethodNames = new List<string>();

                    foreach (var payment in order.Invoice.InvoicePayments)
                    {
                        var paymentMethod = await _paymentMethodViewStateLookupService.GetStateAsync(payment.PaymentMethodId, false, cToken);
                        paymentMethodNames.Add(paymentMethod.Name);
                    }

                    userOrderViewState.Invoice.PaymentMethodNames = string.Join(", ", paymentMethodNames);

                    if (userOrderViewState.TotalPointsPrice > 0)
                    {
                        userOrderViewState.Invoice.PaymentMethodNames += " & Points";//TODO: A TRANSLATE
                    }
                }

                var userOrderLineViewStates = new List<UserOrderLineViewState>();

                List<string> productNames = new List<string>();

                foreach (var orderLine in order.OrderLines)
                {
                    var userOrderLineViewState = new UserOrderLineViewState();

                    userOrderLineViewState.Id = orderLine.Id;
                    userOrderLineViewState.LineType = orderLine.LineType;
                    userOrderLineViewState.PayType = orderLine.PayType;
                    userOrderLineViewState.Quantity = orderLine.Quantity;
                    userOrderLineViewState.TotalPrice = orderLine.Total;
                    userOrderLineViewState.TotalPointsPrice = orderLine.PointsTotal;
                    userOrderLineViewState.ProductId = orderLine.ProductId;

                    userOrderLineViewState.ProductName = orderLine.ProductName;
                    productNames.Add(userOrderLineViewState.ProductName);

                    userOrderLineViewStates.Add(userOrderLineViewState);
                }

                userOrderViewState.OrderLines = userOrderLineViewStates;

                userOrderViewState.ProductNames = string.Join(", ", productNames);

                userOrderViewStates.Add(userOrderViewState);
            }

            if (prev)
            {
                //IF WE LOAD THE PREVIOUS PAGE THEN WE HAVE TO SORT THE DATA CLIENT SIDE.
                ViewState.Orders = userOrderViewStates.OrderByDescending(a => a.OrderDate);
            }
            else
            {
                ViewState.Orders = userOrderViewStates;
            }

            if (prev)
            {
                //IF WE LOAD THE PREVIOUS PAGE THEN WE HAVE TO REVERSE THE CURSORS.
                ViewState.PrevCursor = ordersList.NextCursor;
                ViewState.NextCursor = ordersList.PrevCursor;
            }
            else
            {
                ViewState.PrevCursor = ordersList.PrevCursor;
                ViewState.NextCursor = ordersList.NextCursor;
            }

            ViewState.RaiseChanged();
        }

        public Task LoadAsync(CancellationToken cToken = default)
        {
            return LoadCursor(null, false, cToken);
        }

        #endregion

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            await LoadAsync(cToken);
        }
    }
}
