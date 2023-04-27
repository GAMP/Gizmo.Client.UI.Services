using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
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
            ILogger<PurchasesViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion

        #region FIELDS
        #endregion

        #region FUNCTIONS

        public Task LoadAsync(CancellationToken cToken = default)
        {
            //Test
            Random random = new Random();

            List<string> productNames = new List<string>() { "Espresso Coffee", "Redbull 330ml", "Panini Turkey Croissant" };
            List<string> paymentMethodNames = new List<string>() { "Balance", "Credit Card" };

            var orders = new List<UserOrderViewState>();

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    UserOrderViewState userOrderViewState = new UserOrderViewState();

                    userOrderViewState.OrderDate = new DateTime(2020, 1, 2);
                    userOrderViewState.OrderStatus = (OrderStatus)random.Next(0, 4);
                    userOrderViewState.TotalPointsAward = random.Next(0, 100);
                    userOrderViewState.Notes = "Amet minim mollit non deserunt ullamco est sit aliqua dolor do amet sint. Velit officia consequat d...";

                    userOrderViewState.Invoice = new UserOrderInvoiceViewState();
                    userOrderViewState.Invoice.PaymentStatus = (InvoiceStatus)random.Next(0, 3);

                    var userOrderLineViewStates = new List<UserOrderLineViewState>();

                    for (int j = 0; j < 3; j++)
                    {
                        UserOrderLineViewState userOrderLineViewState = new UserOrderLineViewState();

                        userOrderLineViewState.ProductName = productNames[random.Next(0, 3)];
                        userOrderLineViewState.Quantity = random.Next(0, 5);
                        userOrderLineViewState.TotalPrice = (decimal)random.Next(0, 100) / 100;
                        userOrderLineViewState.TotalPointsPrice = random.Next(0, 100);

                        userOrderLineViewStates.Add(userOrderLineViewState);

                        userOrderViewState.ProductNames += userOrderLineViewState.ProductName + ", ";
                        userOrderViewState.TotalPrice += userOrderLineViewState.TotalPrice;
                        userOrderViewState.TotalPointsPrice += userOrderLineViewState.TotalPointsPrice;
                    }

                    userOrderViewState.OrderLines = userOrderLineViewStates;

                    orders.Add(userOrderViewState);
                }

                ViewState.Orders = orders;
                //End Test
            }
            catch (Exception ex)
            {

            }

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            await LoadAsync(cToken);
        }
    }
}
