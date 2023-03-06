using Gizmo.Web.Api.Models;

namespace Gizmo.Client
{
    public partial class TestClient : IGizmoClient
    {
        private readonly List<UserProductGroupModel> _userProductGroups;
        private readonly List<UserProductModel> _userProducts;
        private readonly List<UserPaymentMethodModel> _userPaymentMethods;

        public TestClient()
        {
            Random random = new();

            #region PRODUCT GROUPS
            _userProductGroups = new List<UserProductGroupModel>
            {
                new() { Id = 1, Name = "#Coffee" },
                new() { Id = 2, Name = "#Beverages" },
                new() { Id = 3, Name = "#Sandwiches" },
                new() { Id = 4, Name = "#Snacks" },
                new() { Id = 5, Name = "#Time offers" },
                new() { Id = 6, Name = "#Toasts" },
                new() { Id = 7, Name = "#Burgers" }
            };
            #endregion

            #region PRODUCTS
            _userProducts = Enumerable.Range(1, 20).Select(x => new UserProductModel()
            {
                Id = x,
                ProductGroupId = random.Next(1, _userProductGroups.Count + 1),
                Name = $"#Coca Cola {x} 500ml",
                Description = "#Iced coffee is a coffee beverage served cold. It may be prepared either by brewing coffee in the normal way and then serving it over ice.",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                PointsAward = random.Next(1, 500),
                ProductType = (ProductType)random.Next(0, 3),
                PurchaseOptions = (PurchaseOptionType)random.Next(0, 2),

            }).ToList();

            _userProducts.Where(product => product.ProductType == ProductType.ProductTime)
                .ToList()
                .ForEach(product =>
                {
                    product.TimeProduct = new UserProductTimeModel()
                    {
                        Minutes = random.Next(30, 180)
                    };
                });

            _userProducts.Where(product => product.ProductType == ProductType.ProductBundle)
               .ToList()
               .ForEach(product =>
               {
                   product.Bundle = new UserProductBundleModel()
                   {
                       BundledProducts = Enumerable.Range(1, 20)
                        .Take(random.Next(1, 5))
                        .Select(x => new UserProductBundledModel() { ProductId = x, Quantity = random.Next(1, 3) })
                   };
               });



            #endregion

            #region PAYMENT METHODS
            _userPaymentMethods = new List<UserPaymentMethodModel>()
            {
                new UserPaymentMethodModel() { Id = -1 , Name= "Cash" , DisplayOrder =0},
                new UserPaymentMethodModel() { Id = -2 , Name= "Credit card" , DisplayOrder =0},
                new UserPaymentMethodModel() { Id = -3 , Name= "Balance" , DisplayOrder =0},
            };
            #endregion
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

        public Task<UserRecoveryMethodGetResultModel> UserPasswordRecoveryMethodGetAsync(string userNameEmailOrMobile, CancellationToken cancellationToken = default)
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

        public Task<PagedList<UserApplicationEnterpriseModel>> UserApplicationEnterprisesGetAsync(UserApplicationEnterprisesFilter filters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<UserApplicationCategoryModel>> UserApplicationCategoriesGetAsync(UserApplicationCategoriesFilter filters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<UserApplicationModel>> UserApplicationsGetAsync(UserApplicationsFilter filters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<UserApplicationLinkModel>> UserApplicationLinksGetAsync(UserApplicationLinksFilter filters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserApplicationModel?> UserApplicationGetAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<UserExecutableModel>> UserExecutablesGetAsync(UserExecutablesFilter filters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserExecutableModel> UserExecutableGetAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<UserPersonalFileModel>> UserPersonalFilesGetAsync(UserPersonalFilesFilter filters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateResult> UserPasswordUpdateAsync(string oldPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
