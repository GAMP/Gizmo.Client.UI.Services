﻿using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Gizmo.Client.UI.View.Services
{
    public abstract class ValidatingViewServiceBase<TViewState> : ClientViewServiceBase<TViewState> where TViewState : IValidatingViewState
    {
        #region CONSTRUCTOR
        public ValidatingViewServiceBase(TViewState viewState,
            ILogger logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _editContext = new EditContext(viewState);
            _validationMessageStore = new ValidationMessageStore(_editContext);
            _fieldChangedObservable = Observable.FromEventPattern<FieldChangedEventArgs>(e => _editContext.OnFieldChanged += e,
                e => _editContext.OnFieldChanged -= e)
                .Throttle(TimeSpan.FromMilliseconds(1000));
        }
        #endregion

        #region READ ONLY FIELDS
        private readonly EditContext _editContext;
        private readonly ValidationMessageStore _validationMessageStore;
        private readonly IObservable<System.Reactive.EventPattern<FieldChangedEventArgs>> _fieldChangedObservable;
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

        #region PROTECTED FUNCTIONS
        
        /// <summary>
        /// Resets current validation state.
        /// The method will do the following operation.<br></br>
        /// * Clear validation error message store.<br></br>
        /// * Mark edit context as unmodified.<br></br>
        /// * Call <see cref="EditContext.NotifyValidationStateChanged"/> function on current edit context.<br></br>
        /// </summary>
        protected void ResetValidationErrors()
        {
            _validationMessageStore.Clear();
            _editContext.MarkAsUnmodified();
            _editContext.NotifyValidationStateChanged();
        }

        /// <summary>
        /// Validates all validation participating properties on current <see cref="ViewStateServiceBase.ViewState"/>.
        /// </summary>
        protected void ValidateProperties()
        {
            //get validation information from the view state
            var validationObjects = ObjectPropertyCache.GetValidationObjects(ViewState);

            //process each validating object instance
            foreach (var validationObject in validationObjects)
            {
                foreach (var property in validationObject.Properties)
                {
                    if (validationObject.Instance == null)
                        continue;

                    //validate each individual property on the object instance
                    ValidateProperty(new FieldIdentifier(validationObject.Instance, property.Name));
                }
            }

            //once we have validated the properties raise validation state change event            
            _editContext.NotifyValidationStateChanged();
        }

        protected void ValidateProperty(in FieldIdentifier fieldIdentifier)
        {
            //the field identifier will have the property name and obect

            //since we revalidating we need to remove the messages associated with the field
            _validationMessageStore.Clear(fieldIdentifier);

            //data annotation validation
            UI.Services.Extensions.DataAnnotationsValidator.Validate(fieldIdentifier, _validationMessageStore);

            //custom validation
        }

        /// <summary>
        /// Mark specified property as modified and returns associated <see cref="FieldIdentifier"/>.
        /// </summary>
        /// <param name="model">Owning model.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Filed identifier.</returns>
        protected FieldIdentifier MarkModified(object model, string propertyName)
        {
            //get property field, here we could use some kind of caching <object,property>
            var fieldIdentifier = new FieldIdentifier(model, propertyName);

            //check if property is marked as modified already
            //this should only occur if the property was updated from an InputComponent that is aware of EditContext
            if (!EditContext.IsModified(fieldIdentifier))
            {
                //mark as modified and raise FieldChanged on EditContext
                //if for some reason we need to have consistent FieldChanged event this migtht need to be called every time
                EditContext.NotifyFieldChanged(fieldIdentifier);
            }

            return fieldIdentifier;
        } 

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Handles edit context validation request.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Args.</param>
        /// <remarks>
        /// This method will only be invoked after <see cref="EditContext.Validate"/> is called.
        /// </remarks>
        private void OnEditContextValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            //when validation is requested on the context all view state properties that participate in validation should be revalidated

            //it should not be required to call ResetValidationErrors() since each individual property will be re-validated

            //revalidate all properties
            ValidateProperties();
        }

        private void OnEditContextValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
        {
            //here we could have an state object that would indicate that an async validation is currently running so it would help determine
            //if the state is valid or not, just a example for now
            _editContext.Properties.TryGetValue("IsAsyncValidationRunning", out object? value);

            using (ViewState.PropertyChangedLock())
            {
                ViewState.IsValid = !EditContext.GetValidationMessages().Any();
                ViewState.IsDirty = EditContext.IsModified();
            }

            ViewStateChanged();
        }

        private void OnEditContextFieldChangeDebounced(FieldChangedEventArgs args)
        {
            //technically we are not relying on this event and its currently only prsent for experimentation
        }

        #endregion

        #region OVERRIDES

        protected override void OnViewStatePropertyChangedDebounced(object sender, IEnumerable<PropertyChangedEventArgs> propertyChangedArgs)
        {
            base.OnViewStatePropertyChangedDebounced(sender, propertyChangedArgs);

            foreach (var property in propertyChangedArgs)
            {
                //check is required to avoid nullable compilation errors
                if (!string.IsNullOrEmpty(property.PropertyName))
                {
                    //mark the field as modified
                    var fieldIdentifier = MarkModified(sender, property.PropertyName);

                    //validate property
                    ValidateProperty(fieldIdentifier);
                }
            }

            EditContext.NotifyValidationStateChanged();
            ViewStateChanged();
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _fieldChangedObservable.Subscribe(e => OnEditContextFieldChangeDebounced(e.EventArgs));

            _editContext.OnValidationStateChanged += OnEditContextValidationStateChanged;
            _editContext.OnValidationRequested += OnEditContextValidationRequested;

            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool dis)
        {
            base.OnDisposing(dis);
            _editContext.OnValidationStateChanged -= OnEditContextValidationStateChanged;
            _editContext.OnValidationRequested -= OnEditContextValidationRequested;
        }

        #endregion
    }
}
