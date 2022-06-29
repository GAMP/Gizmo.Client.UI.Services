using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    public sealed class UserLoginService : ViewStateServiceBase<UserLoginViewState>, IDisposable
    {
        #region CONSTRUCTOR
        public UserLoginService(UserLoginViewState viewState,ILogger<UserLoginService> logger):base(viewState, logger)
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

        public override Task IntializeAsync(CancellationToken ct)
        {
            ViewState.PropertyChanged += ViewState_PropertyChanged;
            return base.IntializeAsync(ct);
        }

        private void ViewState_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            EditContext.Validate();
        }

        public override void Dispose()
        {
            ViewState.PropertyChanged += ViewState_PropertyChanged;
            _editContext.OnFieldChanged -= _editContext_OnFieldChanged;
            _editContext.OnValidationStateChanged -= _editContext_OnValidationStateChanged;
            _editContext.OnValidationRequested -= _editContext_OnValidationRequested;
        }
    }
}
