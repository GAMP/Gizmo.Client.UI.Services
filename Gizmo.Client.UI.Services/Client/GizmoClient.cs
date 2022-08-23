using Gizmo.Shared.Client.UI.ViewModels;

namespace Gizmo.Client
{
    public class GizmoClient : IGizmoClient
    {
        public async Task<UserLoginResult> LoginAsync(string loginName, string password, CancellationToken cancellationToken)
        {
            await Task.Delay(3000, cancellationToken);
            return new UserLoginResult() { AccessToken = string.Empty, Result = UserLoginCode.Sucess };
        }

        public async Task<UserLogoutResult> UserLogoutAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(3000, cancellationToken);
            return new UserLogoutResult();
        }

        public IEnumerable<ProductGroupViewModel> GetProductGroups()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductViewModel> GetProducts()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ApplicationViewModel> GetApplications()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ExecutableViewModel> GetExecutables()
        {
            throw new NotImplementedException();
        }
    }

    public class UserLoginResult
    {
        public UserLoginCode Result { get; init; }

        public string AccessToken
        {
            get; init;
        }
    }

    public class UserLogoutResult
    {

    }

    public enum UserLoginCode
    {
        Sucess = 0,
    }
}
