using Gizmo.Web.Api.Models;

namespace Gizmo.Client
{
    public partial class TestClient : IGizmoClient
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

        private readonly List<ProductGroupModel> _productGroups;
        private List<ProductModel> _products;

        public async Task<LoginResult> UserLoginAsync(string loginName, string? password, CancellationToken cancellationToken)
        {
            await Task.Delay(3000, cancellationToken);
            return LoginResult.Sucess;
        }

        public async Task UserLogoutAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(3000, cancellationToken);
        }

        #region Password Recovery

        public Task<UserRecoveryMethodGetResultModel> UserPasswordRecoveryMethodGetAsync(string userNameEmailOrMobile, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PasswordRecoveryStartResultModelByMobile> UserPasswordRecoveryByMobileStartAsync(string username, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PasswordRecoveryStartResultModelByEmail> UserPasswordRecoveryByEmailStartAsync(string email, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PasswordRecoveryCompleteResultCode> UserPasswordRecoveryCompleteAsync(string token, string confirmationCode, string newPassword, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Registration

        public Task<UserModelRequiredInfo> GetDefaultUserGroupRequiredInfoAsync()
        {
            return Task.FromResult(new UserModelRequiredInfo());
        }

        public Task<AccountCreationResultModelByEmail> AccountCreationByEmailStartAsync(string email)
        {
            return Task.FromResult(new AccountCreationResultModelByEmail());
        }

        public Task<AccountCreationResultModelByMobilePhone> AccountCreationByMobilePhoneStartAsync(string mobilePhone)
        {
            return Task.FromResult(new AccountCreationResultModelByMobilePhone());
        }

        public Task<AccountCreationCompleteResultModel> AccountCreationCompleteAsync(UserModelUpdate user, string password)
        {
            return Task.FromResult(new AccountCreationCompleteResultModel());
        }

        public Task<AccountCreationCompleteResultModelByToken> AccountCreationByTokenCompleteAsync(string token, UserModelUpdate user, string password)
        {
            return Task.FromResult(new AccountCreationCompleteResultModelByToken());
        }

        #endregion

        #region Products

        public Task<PagedList<ProductGroupModel>> ProductGroupsGetAsync(ProductGroupsFilter filter, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedList<ProductGroupModel>(_productGroups));
        }

        public Task<PagedList<ProductModel>> ProductsGetAsync(ProductsFilter filter, CancellationToken cancellationToken = default)
        {
            var query = _products.AsQueryable();

            if (filter.ProductGroupId.HasValue)
            {
                query = _products.Where(a => a.ProductGroupId == filter.ProductGroupId.Value).AsQueryable();
            }

            var pagedList = new PagedList<ProductModel>(query.ToList());

            return Task.FromResult(pagedList);
        }

        public Task<ProductModel> ProductGetAsync(int id, ModelFilterOptions? options = null)
        {
            return Task.FromResult(_products.Where(a => a.Id == id).First());
        }

        public Task<PagedList<ProductBundledModel>> ProductsBundleGetAsync(int id, CancellationToken cancellationToken = default)
        {
            Random random = new();

            var bundledProducts = Enumerable.Range(1, 5).Select(i => new ProductBundledModel()
            {
                Id = i,
                ProductId = random.Next(1, 5),
                Quantity = random.Next(1, 5),
                UnitPrice = random.Next(1, 5)
            }).ToList();

            var pagedList = new PagedList<ProductBundledModel>(bundledProducts);

            return Task.FromResult(pagedList);
        }

        #endregion

        #region Applications

        public Task<ApplicationModelImage> GetApplicationImageAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationExecutableModelImage> GetApplicationExecutableImageAsync(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        public Task<PagedList<UserAgreementModel>> UserAgreementsGetAsync(UserAgreementsFilter filter, CancellationToken cancellationToken = default)
        {
            var userAgreements = Enumerable.Range(1, 3).Select(i => new UserAgreementModel()
            {
                Id = i,
                Agreement = $"#{i} Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
            }).ToList();

            var pagedList = new PagedList<UserAgreementModel>(userAgreements);

            return Task.FromResult(pagedList);
        }

        public Task<PagedList<UserAgreementModel>> UserAgreementsPendingGetAsync(UserAgreementsFilter filter, CancellationToken cancellationToken = default)
        {
            var userAgreements = Enumerable.Range(1, 3).Select(i => new UserAgreementModel()
            {
                Id = i,
                Agreement = $"#{i} Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
            }).ToList();

            var pagedList = new PagedList<UserAgreementModel>(userAgreements);

            return Task.FromResult(pagedList);
        }

        public Task<UpdateResult> UserAgreementAcceptAsync(int userAgreementId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateResult> UserAgreementRejectAsync(int userAgreementId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserEmailExistAsync(string email, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserMobileExistAsync(string mobilePhone, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TokenIsValidAsync(TokenType tokenType, string token, string confirmationCode, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserModelRequiredInfo?> UserGroupDefaultRequiredInfoGetAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AccountCreationResultModelByEmail> UserCreateByEmailStartAsync(string email, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AccountCreationResultModelByMobilePhone> UserCreateByMobileStartAsync(string mobilePhone, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AccountCreationCompleteResultModel> UserCreateCompleteAsync(UserProfileModelCreate user, string password, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AccountCreationCompleteResultModelByToken> UserCreateByTokenCompleteAsync(string token, UserProfileModelCreate user, string password, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserBalanceModel> UserBalanceGetAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserProfileModel> UserProfileGetAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new UserProfileModel()
            {
                Username = "#Test Username",
                FirstName = "#Test First Name",
                LastName = "#Test Last Name",
                BirthDate = new DateTime(1950, 1, 2),
                Sex = Sex.Male,
                Country = "#Greece",
                Address = "#Test Address 123",
                Email = "#test@test.test",
                Phone = "#0123456789",
                MobilePhone = "#1234567890",
                //TODO: A DOES NOT EXISTS IN API RegistrationDate = new DateTime(2020, 3, 4)
            });
        }

        public Task<UpdateResult> UserProfileUpdateAsync(UserProfileModelUpdate user, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserAgreementModelState>> UserAgreementsStatesGetAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateResult> UserAgreementStateSetAsync(int userAgreementId, UserAgreementAcceptState state, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExistAsync(string userNameEmailOrMobile, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        Task<UpdateResult> IGizmoClient.UserPasswordUpdateAsync(string oldPassword, string newPassword, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentIntentCreateResultModel> PaymentIntentCreateAsync(PaymentIntentCreateParametersDepositModel parameters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentOnlineConfigurationModel> OnlinePaymentsConfigurationGetAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PaymentOnlineConfigurationModel()
            {
                Presets = new List<decimal>() { 5, 10, 15, 20, 25, 30, 35 }
            });
        }

        public Task<UserUsageSessionModel> UserUsageSessionGetAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ClientReservationModel> ClientReservationGetAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<NewsModel>> NewsGetAsync(NewsFilter filters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<FeedModel>> FeedsGetAsync(FeedsFilter filters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<PaymentMethodModel>> PaymentMethodsGetAsync(PaymentMethodsFilter filter, CancellationToken cancellationToken = default)
        {
            List<PaymentMethodModel> paymentMethods = Enumerable.Range(1, 5).Select(i => new PaymentMethodModel()
            {
                Id = i,
                Name = $"#Payment method {i}"
            }).ToList();

            var pagedList = new PagedList<PaymentMethodModel>(paymentMethods);

            return Task.FromResult(pagedList);
        }

        public Task<CreateResult> UserOrderCreateAsync(int userId, OrderCalculateModelOptions calculateOrderOptions, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<ApplicationGroupModel>> ApplicationGroupsGetAsync(ApplicationGroupsFilter filter, CancellationToken cancellationToken = default)
        {
            List<ApplicationGroupModel> applicationGroups = Enumerable.Range(1, 5).Select(i => new ApplicationGroupModel()
            {
                Id = i,
                Name = $"#Category ({i})"
            }).ToList();

            var pagedList = new PagedList<ApplicationGroupModel>(applicationGroups);

            return Task.FromResult(pagedList);
        }

        public Task<PagedList<ApplicationEnterpriseModel>> EnterprisesGetAsync(ApplicationEnterprisesFilter filter, CancellationToken cancellationToken = default)
        {
            List<ApplicationEnterpriseModel> applicationEnterprises = Enumerable.Range(1, 5).Select(i => new ApplicationEnterpriseModel()
            {
                Id = i,
                Name = $"#Test ({i})"
            }).ToList();

            var pagedList = new PagedList<ApplicationEnterpriseModel>(applicationEnterprises);

            return Task.FromResult(pagedList);
        }

        public Task<PagedList<ApplicationModel>> ApplicationsGetAsync(ApplicationsFilter filter, CancellationToken cancellationToken = default)
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

            var pagedList = new PagedList<ApplicationModel>(applications);

            return Task.FromResult(pagedList);
        }

        public Task<PagedList<ApplicationExecutableModel>> ExecutablesGetAsync(ApplicationExecutablesFilter filter, CancellationToken cancellationToken = default)
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

            var pagedList = new PagedList<ApplicationExecutableModel>(executables);

            return Task.FromResult(pagedList);
        }

        public Task<PagedList<UserProductGroupModel>> UserProductGroupsGetAsync(UserProductGroupsFilter filters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<UserProductModel>> UserProductsGetAsync(UserProductsFilter filters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserProductModel> UserProductGetAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<UserPaymentMethodModel>> UserPaymentMethodsGetAsync(UserPaymentMethodsFilter filters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AccountCreationResultModelByMobilePhone> UserCreateByMobileStartAsync(string mobilePhone, ConfirmationCodeDeliveryMethod confirmationCodeDeliveryMethod = ConfirmationCodeDeliveryMethod.Undetermined, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PasswordRecoveryStartResultModelByMobile> UserPasswordRecoveryByMobileStartAsync(string username, ConfirmationCodeDeliveryMethod confirmationCodeDeliveryMethod = ConfirmationCodeDeliveryMethod.Undetermined, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ProductGroupModel?> ProductGroupGetAsync(int id, CancellationToken cToken = default) =>
            Task.FromResult(_productGroups.Find(x => x.Id == id));
    }

}
