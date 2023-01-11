using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;

namespace Gizmo.Client
{
    public class TestClient : IGizmoClient
    {
        public TestClient()
        {
            Random random = new Random();

            _productGroups = new List<ProductGroup>();
            _productGroups.Add(new ProductGroup() { Id = 1, Name = "#Coffee" });
            _productGroups.Add(new ProductGroup() { Id = 2, Name = "#Beverages" });
            _productGroups.Add(new ProductGroup() { Id = 3, Name = "#Sandwiches" });
            _productGroups.Add(new ProductGroup() { Id = 4, Name = "#Snacks" });
            _productGroups.Add(new ProductGroup() { Id = 5, Name = "#Time offers" });

            _productGroups.Add(new ProductGroup() { Id = 6, Name = "#Coffee" });
            _productGroups.Add(new ProductGroup() { Id = 7, Name = "#Beverages" });
            _productGroups.Add(new ProductGroup() { Id = 8, Name = "#Sandwiches" });
            _productGroups.Add(new ProductGroup() { Id = 9, Name = "#Snacks" });
            _productGroups.Add(new ProductGroup() { Id = 10, Name = "#Time offers" });
            _productGroups.Add(new ProductGroup() { Id = 11, Name = "#Coffee" });
            _productGroups.Add(new ProductGroup() { Id = 12, Name = "#Beverages" });
            _productGroups.Add(new ProductGroup() { Id = 13, Name = "#Sandwiches" });
            _productGroups.Add(new ProductGroup() { Id = 14, Name = "#Snacks" });
            _productGroups.Add(new ProductGroup() { Id = 15, Name = "#Time offers" });
            _productGroups.Add(new ProductGroup() { Id = 16, Name = "#Coffee" });
            _productGroups.Add(new ProductGroup() { Id = 17, Name = "#Beverages" });
            _productGroups.Add(new ProductGroup() { Id = 18, Name = "#Sandwiches" });
            _productGroups.Add(new ProductGroup() { Id = 19, Name = "#Snacks" });
            _productGroups.Add(new ProductGroup() { Id = 20, Name = "#Time offers" });

            _products = Enumerable.Range(1, 18).Select(i => new Product()
            {
                Id = i,
                ProductGroupId = random.Next(1, 5),
                Name = $"#Coca Cola {i} 500ml",
                Description = "#Iced coffee is a coffee beverage served cold. It may be prepared either by brewing coffee in the normal way and then serving it over ice.",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                Points = random.Next(1, 500),
                ProductType = (ProductType)random.Next(0, 3),
                PurchaseOptions = (PurchaseOptionType)random.Next(0, 2),
            }).ToList();
        }

        private List<ProductGroup> _productGroups;
        private List<Product> _products;

        public async Task<PagedList<UserAgreement>> GetUserAgreementsAsync(UserAgreementsFilter filter)
        {
            var userAgreements = Enumerable.Range(1, 3).Select(i => new UserAgreement()
            {
                Id = i,
                Agreement = $"#{i} Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
            }).ToList();

            var pagedList = new PagedList<UserAgreement>(userAgreements, new PaginationMetadata());

            return pagedList;
        }

        public async Task<List<UserAgreementState>> GetUserAgreementStatesAsync(int userId)
        {
            var userAgreementStates = Enumerable.Range(1, 3).Select(i => new UserAgreementState()
            {
                UserAgreementId = i,
                AcceptState = UserAgreementAcceptState.None
            }).ToList();

            return userAgreementStates;
        }

        public async Task<UpdateResult> AcceptUserAgreementForUserAsync(int userAgreementId, int userId)
        {
            return new UpdateResult();
        }

        public async Task<UpdateResult> RejectUserAgreementForUserAsync(int userAgreementId, int userId)
        {
            return new UpdateResult();
        }

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

        public async Task<PagedList<ProductGroup>> GetProductGroupsAsync(ProductGroupsFilter filter)
        {
            var pagedList = new PagedList<ProductGroup>(_productGroups, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<Product>> GetProductsAsync(ProductsFilter filter)
        {
            var query = _products.AsQueryable();

            if (filter.ProductGroupId.HasValue)
            {
                query = _products.Where(a => a.ProductGroupId == filter.ProductGroupId.Value).AsQueryable();
            }

            var pagedList = new PagedList<Product>(query.ToList(), new PaginationMetadata());

            return pagedList;
        }

        public async Task<Product> GetProductByIdAsync(int id, GetOptions? options = null)
        {
            return _products.Where(a => a.Id == id).FirstOrDefault();
        }

        public async Task<PagedList<BundledProduct>> GetBundledProductsAsync(int id)
        {
            Random random = new Random();

            var bundledProducts = Enumerable.Range(1, 5).Select(i => new BundledProduct()
            {
                Id = i,
                ProductId = random.Next(1, 5),
                Quantity = random.Next(1, 5),
                UnitPrice = random.Next(1, 5)
            }).ToList();

            var pagedList = new PagedList<BundledProduct>(bundledProducts, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<ApplicationGroup>> GetApplicationGroupsAsync(ApplicationGroupsFilter filter)
        {
            List<ApplicationGroup> applicationGroups = Enumerable.Range(1, 5).Select(i => new ApplicationGroup()
            {
                Id = i,
                Name = $"#Category ({i})"
            }).ToList();

            var pagedList = new PagedList<ApplicationGroup>(applicationGroups, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<ApplicationEnterprise>> GetAppEnterprisesAsync(ApplicationEnterprisesFilter filter)
        {
            List<ApplicationEnterprise> applicationEnterprises = Enumerable.Range(1, 5).Select(i => new ApplicationEnterprise()
            {
                Id = i,
                Name = $"#Test ({i})"
            }).ToList();

            var pagedList = new PagedList<ApplicationEnterprise>(applicationEnterprises, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<Application>> GetApplicationsAsync(ApplicationsFilter filter)
        {
            Random random = new Random();

            List<Application> applications = Enumerable.Range(1, 15).Select(i => new Application()
            {
                Id = i,
                ApplicationCategoryId = random.Next(1, 5),
                Title = $"#Fortnite ({i})",
                Description = "#Fall Guys is a massively multiplayer party game with up to 60 players online in a free-for-all struggle through round after round of escalating chaos until one victor remains!",
                PublisherId = random.Next(1, 5),
                ReleaseDate = DateTime.Now
            }).ToList();

            var pagedList = new PagedList<Application>(applications, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<ApplicationExecutable>> GetApplicationExecutablesAsync(ApplicationExecutablesFilter filter)
        {
            List<ApplicationExecutable> executables = new List<ApplicationExecutable>();
            executables.Add(new ApplicationExecutable() { Id = 1, Caption = "#battlenet.exe" });
            executables.Add(new ApplicationExecutable() { Id = 2, Caption = "#DOTA" });
            executables.Add(new ApplicationExecutable() { Id = 3, Caption = "#Spotify" });
            executables.Add(new ApplicationExecutable() { Id = 4, Caption = "#valve_steamclient.exe" });

            var pagedList = new PagedList<ApplicationExecutable>(executables, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<PaymentMethod>> GetPaymentMethodsAsync(PaymentMethodsFilter filter)
        {
            List<PaymentMethod> paymentMethods = Enumerable.Range(1, 5).Select(i => new PaymentMethod()
            {
                Id = i,
                Name = $"#Payment method {i}"
            }).ToList();

            var pagedList = new PagedList<PaymentMethod>(paymentMethods, new PaginationMetadata());

            return pagedList;
        }

        public Task<ApplicationImage> GetApplicationImageAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationExecutableImage> GetApplicationExecutableImageAsync(int id)
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
