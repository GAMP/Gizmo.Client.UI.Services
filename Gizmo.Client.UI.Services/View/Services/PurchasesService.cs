using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class PurchasesService : ViewStateServiceBase<PurchasesViewState>
    {
        #region CONSTRUCTOR
        public PurchasesService(PurchasesViewState viewState,
            ILogger<PurchasesService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public async Task LoadPurchasesAsync()
        {
            //TODO: A Load user purchases on page loading.

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
        }

        #endregion
    }
}