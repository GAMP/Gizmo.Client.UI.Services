using Gizmo.Web.Api.Models;

namespace Gizmo.Client
{
    public class TestClient : IGizmoClient
    {
        public TestClient()
        {
            Random random = new();

            _productGroups = new List<ProductGroupModel>
            {
                new() { Id = 1, Name = "#Coffee" },
                new() { Id = 2, Name = "#Beverages" },
                new() { Id = 3, Name = "#Sandwiches" },
                new() { Id = 4, Name = "#Snacks" },
                new() { Id = 5, Name = "#Time offers" },
                new() { Id = 6, Name = "#Coffee" },
                new() { Id = 7, Name = "#Beverages" },
                new() { Id = 8, Name = "#Sandwiches" },
                new() { Id = 9, Name = "#Snacks" },
                new() { Id = 10, Name = "#Time offers" },
                new() { Id = 11, Name = "#Coffee" },
                new() { Id = 12, Name = "#Beverages" },
                new() { Id = 13, Name = "#Sandwiches" },
                new() { Id = 14, Name = "#Snacks" },
                new() { Id = 15, Name = "#Time offers" },
                new() { Id = 16, Name = "#Coffee" },
                new() { Id = 17, Name = "#Beverages" },
                new() { Id = 18, Name = "#Sandwiches" },
                new() { Id = 19, Name = "#Snacks" },
                new() { Id = 20, Name = "#Time offers" }
            };

            _products = Enumerable.Range(1, 18).Select(i => new ProductModel()
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

        private List<ProductGroupModel> _productGroups;
        private List<ProductModel> _products;

        public async Task<PagedList<UserAgreementModel>> GetUserAgreementsAsync(UserAgreementsFilter filter)
        {
            var userAgreements = Enumerable.Range(1, 3).Select(i => new UserAgreementModel()
            {
                Id = i,
                Agreement = $"#{i} Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
            }).ToList();

            var pagedList = new PagedList<UserAgreementModel>(userAgreements, new PaginationMetadata());

            return pagedList;
        }

        public async Task<List<UserAgreementModelState>> GetUserAgreementStatesAsync(int userId)
        {
            var userAgreementStates = Enumerable.Range(1, 3).Select(i => new UserAgreementModelState()
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

        public async Task UserPasswordChange(int userId, string oldPassword, string newPassword)
        {

        }

        public async Task<PagedList<ProductGroup>> GetProductGroupsAsync(ProductGroupsFilter filter)
        public async Task<PagedList<ProductGroupModel>> GetProductGroupsAsync(ProductGroupsFilter filter)
        {
            var pagedList = new PagedList<ProductGroupModel>(_productGroups, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<ProductModel>> GetProductsAsync(ProductsFilter filter)
        {
            var query = _products.AsQueryable();

            if (filter.ProductGroupId.HasValue)
            {
                query = _products.Where(a => a.ProductGroupId == filter.ProductGroupId.Value).AsQueryable();
            }

            var pagedList = new PagedList<ProductModel>(query.ToList(), new PaginationMetadata());

            return pagedList;
        }

        public async Task<ProductModel> GetProductByIdAsync(int id, ModelFilterOptions? options = null)
        {
            return _products.Where(a => a.Id == id).FirstOrDefault();
        }

        public async Task<PagedList<ProductBundledModel>> GetBundledProductsAsync(int id)
        {
            Random random = new();

            var bundledProducts = Enumerable.Range(1, 5).Select(i => new ProductBundledModel()
            {
                Id = i,
                ProductId = random.Next(1, 5),
                Quantity = random.Next(1, 5),
                UnitPrice = random.Next(1, 5)
            }).ToList();

            var pagedList = new PagedList<ProductBundledModel>(bundledProducts, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<ApplicationGroupModel>> GetApplicationGroupsAsync(ApplicationGroupsFilter filter)
        {
            List<ApplicationGroupModel> applicationGroups = Enumerable.Range(1, 5).Select(i => new ApplicationGroupModel()
            {
                Id = i,
                Name = $"#Category ({i})"
            }).ToList();

            var pagedList = new PagedList<ApplicationGroupModel>(applicationGroups, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<ApplicationEnterpriseModel>> GetAppEnterprisesAsync(ApplicationEnterprisesFilter filter)
        {
            List<ApplicationEnterpriseModel> applicationEnterprises = Enumerable.Range(1, 5).Select(i => new ApplicationEnterpriseModel()
            {
                Id = i,
                Name = $"#Test ({i})"
            }).ToList();

            var pagedList = new PagedList<ApplicationEnterpriseModel>(applicationEnterprises, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<ApplicationModel>> GetApplicationsAsync(ApplicationsFilter filter)
        {
            Random random = new();

            List<ApplicationModel> applications = Enumerable.Range(1, 15).Select(i => new ApplicationModel()
            {
                Id = i,
                ApplicationCategoryId = random.Next(1, 5),
                Title = $"#Fortnite ({i})",
                Description = "#Fall Guys is a massively multiplayer party game with up to 60 players online in a free-for-all struggle through round after round of escalating chaos until one victor remains!",
                PublisherId = random.Next(1, 5),
                ReleaseDate = DateTime.Now
            }).ToList();

            var pagedList = new PagedList<ApplicationModel>(applications, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<ApplicationExecutableModel>> GetApplicationExecutablesAsync(ApplicationExecutablesFilter filter)
        {
            List<ApplicationExecutableModel> executables = new()
            {
                new() { Id = 1, Caption = "#battlenet.exe" },
                new() { Id = 2, Caption = "#DOTA" },
                new() { Id = 3, Caption = "#Spotify" },
                new() { Id = 4, Caption = "#valve_steamclient.exe" },
                new() { Id = 5, Caption = "#Chrome.exe" },
                new() { Id = 6, Caption = "#Chrome.exe" },
                new() { Id = 7, Caption = "#Chrome.exe" },
                new() { Id = 8, Caption = "#Chrome.exe" },
                new() { Id = 9, Caption = "#Chrome.exe" },
                new() { Id = 10, Caption = "#Chrome.exe" },
                new() { Id = 11, Caption = "#Chrome.exe" },
                new() { Id = 12, Caption = "#Chrome.exe" }
            };

            var pagedList = new PagedList<ApplicationExecutableModel>(executables, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<PaymentMethodModel>> GetPaymentMethodsAsync(PaymentMethodsFilter filter)
        {
            List<PaymentMethodModel> paymentMethods = Enumerable.Range(1, 5).Select(i => new PaymentMethodModel()
            {
                Id = i,
                Name = $"#Payment method {i}"
            }).ToList();

            var pagedList = new PagedList<PaymentMethodModel>(paymentMethods, new PaginationMetadata());

            return pagedList;
        }

        public Task<ApplicationModelImage> GetApplicationImageAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationExecutableModelImage> GetApplicationExecutableImageAsync(int id)
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
