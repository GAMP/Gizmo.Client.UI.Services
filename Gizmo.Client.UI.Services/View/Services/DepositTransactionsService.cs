using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.UserDepositsRoute)]
    public sealed class DepositTransactionsService : ViewStateServiceBase<DepositTransactionsViewState>
    {
        #region CONSTRUCTOR
        public DepositTransactionsService(DepositTransactionsViewState viewState,
            ILogger<DepositTransactionsService> logger,
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

            var transactions = Enumerable.Range(0, 18).Select(i => new DepositTransactionViewState()
            {
                TransactionDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, random.Next(1, 28)),
                DepositTransactionType = (DepositTransactionType)random.Next(0, 4),
                Amount = random.Next(0, 100)
            }).ToList();

            ViewState.DepositTransactions = transactions;
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
