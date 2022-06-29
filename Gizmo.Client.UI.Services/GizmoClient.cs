namespace Gizmo.Client
{
    public class GizmoClient : IGizmoClient
    {
        public async Task<UserLoginResult> LoginAsync(string username,string password)
        {
            await Task.Delay(3000);
            return new UserLoginResult() {  AccessToken = string.Empty, LoginCode = UserLoginCode.Sucess};
        }
    }

    public class UserLoginResult
    {
        public UserLoginCode LoginCode { get; init; }

        public string AccessToken
        {
            get;init;
        }
    }

    public enum UserLoginCode
    {
        Sucess=0,

    }
}
