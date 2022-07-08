namespace Gizmo.Client
{
    public class GizmoClient : IGizmoClient
    {
        public async Task<UserLoginResult> LoginAsync(string loginName,string password, CancellationToken cancellationToken)
        {
            await Task.Delay(3000,cancellationToken);
            return new UserLoginResult() {  AccessToken = string.Empty, Result = UserLoginCode.Sucess};
        }

        public async Task<UserLogoutResult> UserLogoutAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(3000, cancellationToken);
            return new UserLogoutResult();
        }
    }

    public class UserLoginResult
    {
        public UserLoginCode Result { get; init; }

        public string AccessToken
        {
            get;init;
        }
    }

    public class UserLogoutResult
    {

    }

    public enum UserLoginCode
    {
        Sucess=0,
    }
}
