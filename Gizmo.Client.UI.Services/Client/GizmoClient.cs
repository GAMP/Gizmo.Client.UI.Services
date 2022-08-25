using Gizmo.Web.Api.Models;

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

        public IEnumerable<ProductGroup> GetProductGroups(ProductGroupsFilter filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetProducts(ProductsFilter filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Application> GetApplications(ApplicationsFilter filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ApplicationExecutable> GetApplicationExecutables(ApplicationExecutablesFilter filter)
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
