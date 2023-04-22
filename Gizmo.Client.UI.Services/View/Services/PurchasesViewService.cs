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

            var purchases = new List<PurchaseViewState>();
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 1, OrderDate = new DateTime(2020, 1, 2), Total = 2.50m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Redbull 330ml", Quantity = 2, OrderDate = new DateTime(2020, 5, 12), Total = 4.59m, PaymentMethod = "Credit Card" });
            purchases.Add(new PurchaseViewState() { ProductName = "Panini Turkey Croissant", Quantity = 1, OrderDate = new DateTime(2020, 5, 16), Total = 1.99m, PaymentMethod = "Credit Card" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 2, OrderDate = new DateTime(2020, 7, 28), Total = 5.00m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 1, OrderDate = new DateTime(2020, 7, 30), Total = 2.50m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 1, OrderDate = new DateTime(2020, 8, 8), Total = 2.50m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Service: Printing", Quantity = 1, OrderDate = new DateTime(2020, 10, 19), Total = 4.99m, PaymentMethod = "Credit Card" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 1, OrderDate = new DateTime(2020, 10, 20), Total = 2.50m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Fredo Espresso Coffe", Quantity = 1, OrderDate = new DateTime(2020, 11, 2), Total = 3.20m, PaymentMethod = "Balance" });

            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 1, OrderDate = new DateTime(2020, 1, 2), Total = 2.50m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Redbull 330ml", Quantity = 2, OrderDate = new DateTime(2020, 5, 12), Total = 4.59m, PaymentMethod = "Credit Card" });
            purchases.Add(new PurchaseViewState() { ProductName = "Panini Turkey Croissant", Quantity = 1, OrderDate = new DateTime(2020, 5, 16), Total = 1.99m, PaymentMethod = "Credit Card" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 2, OrderDate = new DateTime(2020, 7, 28), Total = 5.00m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 1, OrderDate = new DateTime(2020, 7, 30), Total = 2.50m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 1, OrderDate = new DateTime(2020, 8, 8), Total = 2.50m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Service: Printing", Quantity = 1, OrderDate = new DateTime(2020, 10, 19), Total = 4.99m, PaymentMethod = "Credit Card" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 1, OrderDate = new DateTime(2020, 10, 20), Total = 2.50m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Fredo Espresso Coffe", Quantity = 1, OrderDate = new DateTime(2020, 11, 2), Total = 3.20m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 1, OrderDate = new DateTime(2020, 1, 2), Total = 2.50m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Redbull 330ml", Quantity = 2, OrderDate = new DateTime(2020, 5, 12), Total = 4.59m, PaymentMethod = "Credit Card" });
            purchases.Add(new PurchaseViewState() { ProductName = "Panini Turkey Croissant", Quantity = 1, OrderDate = new DateTime(2020, 5, 16), Total = 1.99m, PaymentMethod = "Credit Card" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 2, OrderDate = new DateTime(2020, 7, 28), Total = 5.00m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 1, OrderDate = new DateTime(2020, 7, 30), Total = 2.50m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 1, OrderDate = new DateTime(2020, 8, 8), Total = 2.50m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Service: Printing", Quantity = 1, OrderDate = new DateTime(2020, 10, 19), Total = 4.99m, PaymentMethod = "Credit Card" });
            purchases.Add(new PurchaseViewState() { ProductName = "Espresso Coffee", Quantity = 1, OrderDate = new DateTime(2020, 10, 20), Total = 2.50m, PaymentMethod = "Balance" });
            purchases.Add(new PurchaseViewState() { ProductName = "Fredo Espresso Coffe", Quantity = 1, OrderDate = new DateTime(2020, 11, 2), Total = 3.20m, PaymentMethod = "Balance" });

            foreach (var purchase in purchases)
            {
                purchase.OrderStatus = (OrderStatus)random.Next(0, 4);
            }

            ViewState.Purchases = purchases;
            //End Test

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
