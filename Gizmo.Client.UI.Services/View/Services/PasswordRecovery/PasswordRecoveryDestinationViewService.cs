using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    [Route(ClientRoutes.PasswordRecoveryDestinationRoute)]
    public sealed class PasswordRecoveryDestinationViewService : ValidatingViewStateServiceBase<PasswordRecoveryDestinationViewState>
    {
        private const int USERNAME_MAX_LENGTH = 30;
        private const int EMAIL_MAX_LENGTH = 254;

        private static readonly FileInvalidCharactersValidationAttribute _usernameValidation = new();
        private static readonly EmailNullEmptyValidationAttribute _emailValidation = new();

        private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly IPasswordRecoverySessionService _session;
        private readonly ILocalizationService _localizationService;
        private readonly IPhoneValidationService _phoneValidationService;

        public PasswordRecoveryDestinationViewService(
            PasswordRecoveryDestinationViewState viewState,
            ILogger<PasswordRecoveryDestinationViewService> logger,
            IServiceProvider serviceProvider,
            IPasswordRecoveryService passwordRecoveryService,
            IPasswordRecoverySessionService session,
            ILocalizationService localizationService,
            IPhoneValidationService phoneValidationService) : base(viewState, logger, serviceProvider)
        {
            _passwordRecoveryService = passwordRecoveryService;
            _session = session;
            _localizationService = localizationService;
            _phoneValidationService = phoneValidationService;
        }

        public int MatchValueMaxLength => ViewState.IdentifierKind == PasswordRecoveryIdentifierKind.Email
            ? EMAIL_MAX_LENGTH
            : USERNAME_MAX_LENGTH;

        public void SetMatchValue(string value)
        {
            ViewState.MatchValue = value;
            ValidateProperty(() => ViewState.MatchValue);
        }

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();
        }

        public void SetCountry(string? value)
        {
            ViewState.Country = value;
            ValidateProperty(() => ViewState.Country);
        }

        public void SetRegionCode(string? value)
        {
            ViewState.RegionCode = value;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void SetMobilePhone(string? value)
        {
            ViewState.MobilePhone = value;
            ViewState.PhoneE164 = null;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public async Task SubmitAsync()
        {
            if (ViewState.IsLoading)
                return;

            if (_session.IdentifierKind is null)
            {
                NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryKindRoute);
                return;
            }

            var identifierKind = _session.IdentifierKind.Value;

            ViewState.IsLoading = true;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
                return;
            }

            try
            {
                var matchValue = GetNormalizedMatchValue();

                var methods = await _passwordRecoveryService.GetMethodsAsync(identifierKind, matchValue);

                if (methods.Count == 0)
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RECOVERY_NO_METHODS_AVAILABLE));
                    return;
                }

                _session.SetMatchValue(matchValue, identifierKind);
                _session.SetAvailableMethods(methods);

                if (methods.Count > 1)
                {
                    NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryRoute);
                    return;
                }

                var provider = methods[0];
                _session.SetActiveProvider(provider);

                var startResult = await _passwordRecoveryService.StartAsync(new PasswordRecoveryStartRequest
                {
                    MethodId = provider.MethodId,
                    IdentifierKind = identifierKind,
                    Value = matchValue
                });

                var outcome = PasswordRecoveryStartOutcome.Apply(startResult, _session, _localizationService);

                switch (outcome)
                {
                    case PasswordRecoveryStartOutcome.Result.Started:
                        NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryConfirmationRoute);
                        break;

                    case PasswordRecoveryStartOutcome.Result.Failed failed:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = failed.Message;
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Password recovery destination discovery/start error.");
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (_session.IdentifierKind is null)
            {
                NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryKindRoute);
                return Task.CompletedTask;
            }

            ViewState.IdentifierKind = _session.IdentifierKind.Value;
            ClearInputState();
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ResetValidationState();
            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        private void ClearInputState()
        {
            ViewState.MatchValue = string.Empty;
            ViewState.Country = null;
            ViewState.RegionCode = null;
            ViewState.MobilePhone = null;
            ViewState.PhoneE164 = null;
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (ViewState.IdentifierKind is PasswordRecoveryIdentifierKind.Username or PasswordRecoveryIdentifierKind.Email)
            {
                if (fieldIdentifier.FieldEquals(() => ViewState.MatchValue))
                {
                    if (string.IsNullOrEmpty(ViewState.MatchValue))
                    {
                        AddError(() => ViewState.MatchValue, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                        return;
                    }

                    if (ViewState.MatchValue.Length > MatchValueMaxLength)
                    {
                        AddError(
                            () => ViewState.MatchValue,
                            _localizationService.GetString(
                                nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_MAX_LENGTH),
                                nameof(ViewState.MatchValue),
                                MatchValueMaxLength));
                    }

                    if (ViewState.IdentifierKind == PasswordRecoveryIdentifierKind.Username)
                    {
                        if (ViewState.MatchValue.Any(char.IsWhiteSpace))
                        {
                            AddError(
                                () => ViewState.MatchValue,
                                _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_WHITE_SPACE_NOT_ALLOWED)));
                        }

                        if (!_usernameValidation.IsValid(ViewState.MatchValue))
                        {
                            AddError(
                                () => ViewState.MatchValue,
                                _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_INVALID_FIELD)));
                        }
                    }
                    else if (!_emailValidation.IsValid(ViewState.MatchValue))
                    {
                        AddError(
                            () => ViewState.MatchValue,
                            _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_INVALID_FIELD)));
                    }
                }
            }
            else if (ViewState.IdentifierKind == PasswordRecoveryIdentifierKind.MobilePhone)
            {
                if (fieldIdentifier.FieldEquals(() => ViewState.Country))
                {
                    if (string.IsNullOrEmpty(ViewState.Country))
                        AddError(() => ViewState.Country, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                    else
                        ClearError(() => ViewState.Country);
                }

                if (fieldIdentifier.FieldEquals(() => ViewState.RegionCode))
                {
                    if (string.IsNullOrEmpty(ViewState.RegionCode))
                        AddError(() => ViewState.RegionCode, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_VE_REQUIRED_FIELD)));
                    else
                        ClearError(() => ViewState.RegionCode);
                }
            }
        }

        protected override async Task<IEnumerable<string>> OnValidateAsync(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger, CancellationToken cancellationToken = default)
        {
            if (ViewState.IdentifierKind == PasswordRecoveryIdentifierKind.MobilePhone &&
                fieldIdentifier.FieldEquals(() => ViewState.MobilePhone) &&
                !string.IsNullOrEmpty(ViewState.MobilePhone))
            {
                if (string.IsNullOrEmpty(ViewState.RegionCode))
                    return new string[] { _localizationService.GetString("GIZ_REGISTRATION_VE_SELECT_COUNTRY") };

                var formatResult = await _phoneValidationService.ValidateAsync(ViewState.MobilePhone, ViewState.RegionCode, cancellationToken);
                if (!formatResult.IsValid)
                    return new string[] { _localizationService.GetString(formatResult.ErrorKey ?? nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED)) };

                ViewState.PhoneE164 = formatResult.E164;
                return Array.Empty<string>();
            }

            return await base.OnValidateAsync(fieldIdentifier, validationTrigger, cancellationToken);
        }

        protected override AsyncValidatedDetermineResult OnDetermineIsAsyncPropertiesValidated()
        {
            if (ViewState.IdentifierKind == PasswordRecoveryIdentifierKind.MobilePhone)
            {
                if (IsAsyncValidated(() => ViewState.MobilePhone))
                    return AsyncValidatedDetermineResult.DefaultTrue;
                return base.OnDetermineIsAsyncPropertiesValidated();
            }
            return AsyncValidatedDetermineResult.DefaultTrue;
        }

        private string GetNormalizedMatchValue()
        {
            if (ViewState.IdentifierKind == PasswordRecoveryIdentifierKind.MobilePhone)
            {
                var e164 = ViewState.PhoneE164;
                if (!string.IsNullOrEmpty(e164))
                    return e164.StartsWith("+") ? e164[1..] : e164;
            }
            return ViewState.MatchValue;
        }
    }
}
