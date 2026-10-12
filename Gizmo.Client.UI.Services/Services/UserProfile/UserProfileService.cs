using Gizmo.Server.Exceptions;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.User.Clients;
using Microsoft.Extensions.Logging;
using UserUsersWebApiClient = Gizmo.Web.Api.User.Clients.UsersWebApiClient;

namespace Gizmo.Client.UI.Services;

public sealed class UserProfileService : IUserProfileService
{
    private readonly ProfileWebApiClient _client;
    private readonly UserUsersWebApiClient _usersClient;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(ProfileWebApiClient client, UserUsersWebApiClient usersClient, ILogger<UserProfileService> logger)
    {
        _client = client;
        _usersClient = usersClient;
        _logger = logger;
    }

    public event EventHandler<UserProfileUpdate>? Updated;

    public async Task<UserProfileUpdateCode> UpdateAsync(UserProfileUpdate update, CancellationToken ct = default)
    {
        try
        {
            await _client.UpdateAsync(UserProfileUpdateMapper.Map(update), ct);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex) when (ex.IsExceptionCode(ExceptionCode.NonUniqueEntityValue))
        {
            _logger.LogWarning(ex, "Profile update rejected: non-unique value.");
            return UserProfileUpdateCode.NonUniqueValue;
        }
        catch (Exception ex) when (ex.IsExceptionCode(ExceptionCode.ValidationError) || ex.IsExceptionCode(ExceptionCode.InvalidProperty))
        {
            _logger.LogWarning(ex, "Profile update rejected: invalid input.");
            return UserProfileUpdateCode.InvalidInput;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Profile update failed.");
            return UserProfileUpdateCode.Failure;
        }

        Updated?.Invoke(this, update);

        return UserProfileUpdateCode.Success;
    }

    public Task<bool> EmailExistAsync(string email, CancellationToken ct = default)
    {
        return _usersClient.EmailExistAsync(email, ct);
    }

    public Task<bool> MobilePhoneExistAsync(string mobilePhone, CancellationToken ct = default)
    {
        return _usersClient.MobilePhoneExistAsync(mobilePhone, ct);
    }
}
