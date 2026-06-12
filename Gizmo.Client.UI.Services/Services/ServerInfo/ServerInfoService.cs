using System;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.Web.Api.Clients;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services;

public sealed class ServerInfoService : IServerInfoService
{
    private readonly Gizmo.Web.Api.User.Clients.OptionsWebApiClient _optionsClient;
    private readonly Gizmo.Web.Api.User.Clients.SystemWebApiClient _systemClient;
    private readonly PublicOptionsWebApiClient _publicOptionsClient;
    private readonly ILogger<ServerInfoService> _logger;

    private string? _cachedRegionCode;
    private bool _regionLoaded;

    private string? _cachedVersion;
    private bool _versionLoaded;

    private string? _cachedDefaultCulture;
    private bool _cultureLoaded;

    private bool _cachedRegistrationEnabled;
    private bool _registrationEnabledLoaded;

    private PasswordPolicy? _cachedPasswordPolicy;

    private static readonly PasswordPolicy _defaultPasswordPolicy = new()
    {
        MinimumLength    = 4,
        MaximumLength    = 24,
        RequireLowerCase = false,
        RequireUpperCase = false,
        RequireNumbers   = false,
    };

    public ServerInfoService(
        Gizmo.Web.Api.User.Clients.OptionsWebApiClient optionsClient,
        Gizmo.Web.Api.User.Clients.SystemWebApiClient systemClient,
        PublicOptionsWebApiClient publicOptionsClient,
        ILogger<ServerInfoService> logger)
    {
        _optionsClient = optionsClient;
        _systemClient = systemClient;
        _publicOptionsClient = publicOptionsClient;
        _logger = logger;
    }

    public async Task<string?> GetRegionCodeAsync(CancellationToken ct = default)
    {
        if (_regionLoaded)
            return _cachedRegionCode;

        try
        {
            var options = await _optionsClient.RegionalAsync(ct);
            var code = options.CountryCode;
            if (!string.IsNullOrWhiteSpace(code))
                _cachedRegionCode = code;
            _regionLoaded = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch server regional options.");
        }

        return _cachedRegionCode;
    }

    public async Task<string?> GetVersionAsync(CancellationToken ct = default)
    {
        if (_versionLoaded)
            return _cachedVersion;

        try
        {
            var version = await _systemClient.Version(ct);
            if (!string.IsNullOrWhiteSpace(version))
                _cachedVersion = version;
            _versionLoaded = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch server version.");
        }

        return _cachedVersion;
    }

    public async Task<string?> GetDefaultCultureAsync(CancellationToken ct = default)
    {
        if (_cultureLoaded)
            return _cachedDefaultCulture;

        try
        {
            var options = await _publicOptionsClient.GeneralAsync(ct);
            var culture = options.DefaultCulture;
            if (!string.IsNullOrWhiteSpace(culture))
                _cachedDefaultCulture = culture;
            _cultureLoaded = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch server general options.");
        }

        return _cachedDefaultCulture;
    }

    public async Task<bool> GetRegistrationEnabledAsync(CancellationToken ct = default)
    {
        if (_registrationEnabledLoaded)
            return _cachedRegistrationEnabled;

        try
        {
            _cachedRegistrationEnabled = await _optionsClient.RegistrationEnabledAsync(ct);
            _registrationEnabledLoaded = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch registration enabled flag.");
        }

        return _cachedRegistrationEnabled;
    }

    public async Task<PasswordPolicy> GetPasswordPolicyAsync(CancellationToken ct = default)
    {
        if (_cachedPasswordPolicy is not null)
            return _cachedPasswordPolicy;

        try
        {
            var options = await _optionsClient.UserPasswordPolicyAsync(ct);
            _cachedPasswordPolicy = PasswordPolicyMapper.Map(options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch server password policy.");
            return _defaultPasswordPolicy;
        }

        return _cachedPasswordPolicy;
    }
}
