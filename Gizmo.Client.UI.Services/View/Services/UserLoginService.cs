using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserLoginService : ClientViewServiceBase<UserLoginViewState>, IDisposable
    {
        #region CONSTRUCTOR
        public UserLoginService(UserLoginViewState viewState,
            ILogger<UserLoginService> logger,
            IServiceProvider serviceProvider) :base(viewState, logger,serviceProvider)
        {
            _editContext = new EditContext(viewState);
            _validationMessageStore = new ValidationMessageStore(_editContext);

            _editContext.OnFieldChanged += _editContext_OnFieldChanged;
            _editContext.OnValidationStateChanged += _editContext_OnValidationStateChanged;
            _editContext.OnValidationRequested += _editContext_OnValidationRequested;
        }      
        #endregion

        #region READ ONLY FIELDS
        private readonly EditContext _editContext;
        private readonly ValidationMessageStore _validationMessageStore;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets user login model edit context.
        /// </summary>
        public EditContext EditContext
        {
            get { return _editContext; }
        }
        
        #endregion

        public Task SubmitAsync()
        {
            //  Model.IsValid = EditContext.Validate();
            ViewState.LoginName = $"Count {count++}";
            DebounceViewStateChange();
            return Task.CompletedTask;
        }

        private void _editContext_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            
            //_validationMessageStore.Clear();
            //if(Model.LoginName == null)
            //{
            //    _validationMessageStore.Add(()=>Model.LoginName,"Add some username mate!");
            //}
            //else if(Model.Password == null)
            //{
            //    _validationMessageStore.Add(() => Model.Password, "Add some password mate!");
            //}
        }

        private void _editContext_OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
        {
        }

        private void _editContext_OnFieldChanged(object? sender, FieldChangedEventArgs e)
        {
            //Model.IsValid = EditContext.Validate();
        }
        int count;
        protected override void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            Logger.LogInformation("Navigated to {base}", new Uri(e.Location).LocalPath);
            base.OnLocationChanged(sender, e);

        }

        protected override void OnViewStatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnViewStatePropertyChanged(sender, e);

            EditContext.Validate();
        }

        protected override  void OnDisposing(bool dis)
        {
            base.OnDisposing(dis);

            _editContext.OnFieldChanged -= _editContext_OnFieldChanged;
            _editContext.OnValidationStateChanged -= _editContext_OnValidationStateChanged;
            _editContext.OnValidationRequested -= _editContext_OnValidationRequested;
        }
    }
}
