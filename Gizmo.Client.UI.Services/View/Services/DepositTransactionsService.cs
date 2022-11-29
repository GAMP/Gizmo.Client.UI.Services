using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class DepositTransactionsService : ViewStateServiceBase<DepositTransactionsViewState>
    {
        #region CONSTRUCTOR
        public DepositTransactionsService(DepositTransactionsViewState viewState,
            ILogger<DepositTransactionsService> logger,
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

        public async Task LoadDepositTransactionsAsync()
        {
            Random random = new Random();

            var transactions = Enumerable.Range(0, 18).Select(i => new DepositTransactionViewState()
            {
                TransactionDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, random.Next(1, 28)),
                DepositTransactionType = (DepositTransactionType)random.Next(0, 4),
                Amount = random.Next(0, 100)
            }).ToList();

            ViewState.DepositTransactions = transactions;

            ViewState.RaiseChanged();
        }

        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);


        }
    }
}