using Gizmo.Web.Api.Models;

namespace Gizmo.Client
{
    public partial class TestClient : IGizmoClient
    {
        private readonly List<ProductGroupModel> _productGroups;
        private readonly List<ProductModel> _products;
        private readonly List<UserProductGroupModel> _userProductGroups;
        private readonly List<UserProductModel> _userProducts;
        private readonly List<UserPaymentMethodModel> _userPaymentMethods;
        private readonly List<NewsModel> _newsModel;

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
            };
            _products = Enumerable.Range(1, 20).Select(x => new ProductModel()
            {
                Id = x,
                ProductGroupId = random.Next(1, _productGroups.Count + 1),
                Name = $"#Coca Cola {x} 500ml",
                Description = "#Iced coffee is a coffee beverage served cold. It may be prepared either by brewing coffee in the normal way and then serving it over ice.",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                Points = random.Next(1, 500),
                ProductType = (ProductType)random.Next(0, 3),
                PurchaseOptions = (PurchaseOptionType)random.Next(0, 2),
            }).ToList();
            _userProductGroups = _productGroups.ConvertAll(x => new UserProductGroupModel
            {
                Id = x.Id,
                Name = x.Name
            });
            _userProducts = _products.ConvertAll(x => new UserProductModel
            {
                Id = x.Id,
                ProductGroupId = x.ProductGroupId,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                PointsPrice = x.PointsPrice,
                ProductType = x.ProductType,
                PurchaseOptions = x.PurchaseOptions
            });
            _userPaymentMethods = Enumerable.Range(1, 5).Select(i => new UserPaymentMethodModel
            {
                Id = i,
                Name = $"#User Payment method {i}"
            }).ToList();
            _newsModel = new()
            {
                { new ()
                    {
                        Id = 2,
                        Title = $"#Title 1",
                        Data = "<div style=\"max-width: 40.0rem; margin: 8.6rem 3.2rem 6.5rem 3.2rem\">#1 Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</div>",
                        ThumbnailUrl = $"carousel_1.jpg",
                        Url = "gizmo://addcart/prop/1"
                    }
                },
                { new ()
                    {
                        Id = 3,
                        Title = "LAST OF US",
                        Data = "LAST OF US",
                        ThumbnailUrl = "https://i3.ytimg.com/vi/DIQzxJNR9iI/maxresdefault.jpg",
                        Url = "gizmo://addcart/2"
                    }
                },
                { new ()
                    {
                        Id = 4,
                        Title = "GTA - 5",
                        Data = "GTA - 5",
                        MediaUrl = "https://www.youtube.com/watch?v=TsAaH8yqB70&ab_channel=Punish",
                        ThumbnailUrl = "https://i3.ytimg.com/vi/Ce1eUo0K3VE/maxresdefault.jpg",
                        Url="gizmo://addcart"
                    }
                },
                { new ()
                    {
                        Id = 5,
                        Title = "CRYSIS - 4",
                        Data = "CRYSIS - 4",
                        MediaUrl = "https://www.youtube.com/watch?v=TsAaH8yqB70&ab_channel=Punish",
                        Url = "https://www.theloadout.com/crysis-4/release-date"
                    }
                }
            };
        }

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


        public Task<ProductModel?> ProductGetAsync(int id, ModelFilterOptions? options = null, CancellationToken cToken = default)
        {
            var product = _products.Find(x => x.Id == id);

            return Task.FromResult(product);
        }

        public Task<PagedList<ProductModel>> ProductsGetAsync(ProductsFilter filter, CancellationToken cToken = default)
        {
            var pagedList = new PagedList<ProductModel>(_products);

            return Task.FromResult(pagedList);
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

        public Task<UserProductGroupModel?> UserProductGroupGetAsync(int id, CancellationToken cToken = default) =>
            Task.FromResult(_userProductGroups.Find(x => x.Id == id));

        public Task<PagedList<UserProductGroupModel>> UserProductGroupsGetAsync(UserProductGroupsFilter filters, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PagedList<UserProductGroupModel>(_userProductGroups));

        public Task<UserProductModel?> UserProductGetAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = _userProducts.Find(x => x.Id == id);
            return Task.FromResult(product);
        }

        public Task<PagedList<UserProductModel>> UserProductsGetAsync(UserProductsFilter filters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedList<UserProductModel>(_userProducts));
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

        public Task<PagedList<NewsModel>> NewsGetAsync(NewsFilter filters, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PagedList<NewsModel>(_newsModel));

        public Task<NewsModel?> NewsGetAsync(int id, CancellationToken cToken = default) =>
            Task.FromResult(_newsModel.Find(x => x.Id == id));
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

        public Task<PagedList<UserPaymentMethodModel>> UserPaymentMethodsGetAsync(UserPaymentMethodsFilter filters, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PagedList<UserPaymentMethodModel>(_userPaymentMethods));
        public Task<UserPaymentMethodModel?> UserPaymentMethodGetAsync(int id, CancellationToken cToken = default) =>
            Task.FromResult(_userPaymentMethods.Find(x => x.Id == id));
        public Task<AccountCreationResultModelByMobilePhone> UserCreateByMobileStartAsync(string mobilePhone, ConfirmationCodeDeliveryMethod confirmationCodeDeliveryMethod = ConfirmationCodeDeliveryMethod.Undetermined, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PasswordRecoveryStartResultModelByMobile> UserPasswordRecoveryByMobileStartAsync(string username, ConfirmationCodeDeliveryMethod confirmationCodeDeliveryMethod = ConfirmationCodeDeliveryMethod.Undetermined, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
