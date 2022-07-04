using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserLoginService : ClientViewServiceBase<UserLoginViewState>, IDisposable
    {
        #region CONSTRUCTOR
        public UserLoginService(UserLoginViewState viewState,
            ILogger<UserLoginService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            PropertyChangedDebounceBufferTime = 5000;
            _editContext = new EditContext(viewState);
            _validationMessageStore = new ValidationMessageStore(_editContext);
            _fieldChangedObservable = Observable.FromEventPattern<FieldChangedEventArgs>(e => _editContext.OnFieldChanged += e,
                e => _editContext.OnFieldChanged -= e)
                .Throttle(TimeSpan.FromMilliseconds(100));
        }
        #endregion

        #region READ ONLY FIELDS
        private readonly EditContext _editContext;
        private readonly ValidationMessageStore _validationMessageStore;
        private readonly IObservable<System.Reactive.EventPattern<FieldChangedEventArgs>> _fieldChangedObservable;
        private IDisposable? _dataAnnotationsRegistration;
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

        int count;
        public async Task SubmitAsync()
        {
            if (ViewState.IsValid == null)
            {
                ViewState.IsValid = EditContext.Validate();
            }

            //model validation is pending, we cant proceed
            if (ViewState.IsValid != true)
                return;

            await Task.Delay(0);

            using (ViewState.PropertyChangedLock())
            {
                ViewState.LoginName = $"Count {count++}";
            }

            DebounceViewStateChange();
        }

        private void OnEditContextValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            _validationMessageStore.Clear();

            if (string.IsNullOrWhiteSpace(ViewState.LoginName))
            {
                _validationMessageStore.Add(() => ViewState.LoginName, "Add some username mate!");
            }
            if (string.IsNullOrWhiteSpace(ViewState.Password))
            {
                _validationMessageStore.Add(() => ViewState.Password, "Add some password mate!");
            }
        }

        private void OnEditContextValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
        {
            //check if validation have occured
            if (ViewState.IsValid == null)
                return;

            var currentErrorMessage = EditContext.GetValidationMessages();
            ViewState.IsValid = !currentErrorMessage.Any();
            ViewState.RaiseChanged();
        }

        protected override void OnViewStatePropertyChangedDebounced(object sender, PropertyChangedEventArgs e)
        {
            base.OnViewStatePropertyChangedDebounced(sender, e);

              //once a property changes state looses its validity and becomes pending
            ViewState.IsValid = EditContext.Validate();
   
            if (e.PropertyName == nameof(ViewState.LoginName) && ViewState.LoginName?.Length > 5)
            {
                var newName = ViewState.LoginName[..5];
                ViewState.LoginName = newName;                
            }

            DebounceViewStateChange();
        }

        protected override void OnViewStatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnViewStatePropertyChanged(sender, e);          
        }

        private void FieldChange(FieldChangedEventArgs args)
        {
            ////once a property changes state looses its validity and becomes pending
            //ViewState.IsValid = EditContext.Validate();
            //DebounceViewStateChange();
        }

        protected override void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            Logger.LogInformation("Navigated to {base}", new Uri(e.Location).LocalPath);
            base.OnLocationChanged(sender, e);
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _dataAnnotationsRegistration = _editContext.EnableDataAnnotationsValidation();
            _fieldChangedObservable.Subscribe(e => FieldChange(e.EventArgs));

            _editContext.OnValidationStateChanged += OnEditContextValidationStateChanged;
            _editContext.OnValidationRequested += OnEditContextValidationRequested;

            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool dis)
        {
            base.OnDisposing(dis);

            _dataAnnotationsRegistration?.Dispose();
            _editContext.OnValidationStateChanged -= OnEditContextValidationStateChanged;
            _editContext.OnValidationRequested -= OnEditContextValidationRequested;
        }
    }
}
