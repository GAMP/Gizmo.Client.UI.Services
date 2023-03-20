using Gizmo.Client.Interfaces;
using Gizmo.Client.UI.View;
using Gizmo.Web.Api.Models;

namespace Gizmo.Client
{
    public partial class TestClient : IGizmoClient
    {
        private readonly List<UserPersonalFileModel> _personalFiles;
        private readonly List<UserApplicationEnterpriseModel> _applicationEnterprises;
        private readonly List<UserApplicationCategoryModel> _userApplicationCategories;
        private readonly List<UserApplicationLinkModel> _userApplicationLinks;

        private readonly List<UserApplicationModel> _userApplications;
        private readonly List<UserExecutableModel> _userExecutables;

        private readonly List<UserProductGroupModel> _userProductGroups;
        private readonly List<UserProductModel> _userProducts;
        private readonly List<UserPaymentMethodModel> _userPaymentMethods;
        private readonly List<NewsModel> _newsModel;

        public event EventHandler<ClientExecutionContextStateArgs>? ExecutionContextStateChage;

        public TestClient()
        {
            Random random = new();

            _personalFiles = Enumerable.Range(1, 5).Select(i => new UserPersonalFileModel()
            {
                Id = i,
                Caption = $"#Personal File ({i})"
            }).ToList();

            _applicationEnterprises = Enumerable.Range(1, 5).Select(i => new UserApplicationEnterpriseModel()
            {
                Id = i,
                Name = $"#Test ({i})"
            }).ToList();

            _userApplicationCategories = Enumerable.Range(1, 5).Select(i => new UserApplicationCategoryModel()
            {
                Id = i,
                Name = $"#Category ({i})"
            }).ToList();

            _userApplications = Enumerable.Range(1, 100).Select(i => new UserApplicationModel()
            {
                Id = i,
                ApplicationCategoryId = random.Next(1, 5),
                Title = $"#Fortnite ({i})",
                Description = "#Fall Guys is a massively multiplayer party game with up to 60 players online in a free-for-all struggle through round after round of escalating chaos until one victor remains!",
                PublisherId = random.Next(1, 5),
                ReleaseDate = DateTime.Now,
                ImageId = i
            }).ToList();

            _userApplicationLinks = Enumerable.Range(1, 500).Select(i => new UserApplicationLinkModel()
            {
                Id = i,
                ApplicationId = random.Next(1, 100),
            }).ToList();

            List<string> executableNames = new List<string>()
            {
                "#battlenet.exe",
                "#DOTA",
                "#Spotify",
                "#valve_steamclient.exe"
            };

            _userExecutables = Enumerable.Range(1, 500).Select(i => new UserExecutableModel()
            {
                Id = i,
                ApplicationId = random.Next(1, 100),
                Caption = $"{executableNames[random.Next(1, 4)]} {i}",
                Description = "",
                PersonalFiles = Enumerable.Range(1, 4).Select(x => new UserExecutablePersonalFileModel()
                {
                    PersonalFileId = x,
                    UseOrder = x
                }),
                ImageId = i,
                Options = ExecutableOptionType.QuickLaunch
            }).ToList();

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
            _userProducts = Enumerable.Range(1, 500).Select(x => new UserProductModel()
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
                DefaultImageId = x,

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
            #region ADVERTISMENT
            _newsModel = new()
            {
                { new ()
                    {
                        Id = 1,
                        Title = "DEFAULT VIDEO",
                        Data = "DEFAULT VIDEO",
                        MediaUrl = "https://media.geeksforgeeks.org/wp-content/uploads/20210314115545/sample-video.mp4",
                        Url="gizmo://products/cart/add?productId=1&size=2"
                    }
                },
                { new ()
                    {
                        Id = 2,
                        Title = "GTA - 5",
                        Data = "GTA - 5",
                        ThumbnailUrl = "https://i3.ytimg.com/vi/Ce1eUo0K3VE/maxresdefault.jpg",
                        MediaUrl = "https://vk.com/video_ext.php?oid=-2000182257&id=118182257&hash=0f8faf02a738549a&hd=2",
                        Url="gizmo://products/details/navigate?productId=1"
                    }
                },
                { new ()
                    {
                        Id = 3,
                        Title = "CRYSIS - 4",
                        Data = "<div style=\"max-width: 40.0rem; margin: 8.6rem 3.2rem 6.5rem 3.2rem\">#1 Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.#1 Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</div>",
                        MediaUrl = "https://www.youtube.com/watch?v=TsAaH8yqB70&ab_channel=Punish",
                        Url = "https://www.theloadout.com/crysis-4/release-date"
                    }
                }
            };
            #endregion
        }

        public async Task<LoginResult> UserLoginAsync(string loginName, string? password, CancellationToken cancellationToken)
        {
            await Task.Delay(new Random().Next(100,1000), cancellationToken);
            return LoginResult.Sucess;
        }

        public async Task UserLogoutAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(3000, cancellationToken);
        }

        public Task<UserRecoveryMethodGetResultModel> UserPasswordRecoveryMethodGetAsync(string userNameEmailOrMobile, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new UserRecoveryMethodGetResultModel());
        }

        public Task<PasswordRecoveryStartResultModelByEmail> UserPasswordRecoveryByEmailStartAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PasswordRecoveryStartResultModelByEmail());
        }

        public Task<PasswordRecoveryCompleteResultCode> UserPasswordRecoveryCompleteAsync(string token, string confirmationCode, string newPassword, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PasswordRecoveryCompleteResultCode());
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
            return Task.FromResult(new UpdateResult());
        }

        public Task<UpdateResult> UserAgreementRejectAsync(int userAgreementId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new UpdateResult());
        }

        public Task<bool> UserEmailExistAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<bool> UserMobileExistAsync(string mobilePhone, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<bool> TokenIsValidAsync(TokenType tokenType, string token, string confirmationCode, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        public Task<UserModelRequiredInfo?> UserGroupDefaultRequiredInfoGetAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult((UserModelRequiredInfo?)new UserModelRequiredInfo());
        }

        public Task<AccountCreationResultModelByEmail> UserCreateByEmailStartAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AccountCreationResultModelByEmail());
        }

        public Task<AccountCreationCompleteResultModel> UserCreateCompleteAsync(UserProfileModelCreate user, string password, List<UserAgreementModelState> agreementStates, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AccountCreationCompleteResultModel());
        }

        public Task<AccountCreationCompleteResultModelByToken> UserCreateByTokenCompleteAsync(string token, UserProfileModelCreate user, string password, List<UserAgreementModelState> agreementStates, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AccountCreationCompleteResultModelByToken());
        }

        public Task<UserBalanceModel> UserBalanceGetAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new UserBalanceModel()
            {

            });
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
                //TODO: A Service RegistrationDate
            });
        }

        public Task<UpdateResult> UserProfileUpdateAsync(UserProfileModelUpdate user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new UpdateResult());
        }

        public Task<List<UserAgreementModelState>> UserAgreementsStatesGetAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<UserAgreementModelState>());
        }

        public Task<UpdateResult> UserAgreementStateSetAsync(int userAgreementId, UserAgreementAcceptState state, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new UpdateResult());
        }

        public Task<bool> UserExistAsync(string userNameEmailOrMobile, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
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
            return Task.FromResult(new PaymentIntentCreateResultModel());
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
            return Task.FromResult(new UserUsageSessionModel());
        }

        public Task<ClientReservationModel> ClientReservationGetAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new ClientReservationModel());
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

        public Task<CreateResult> UserOrderCreateAsync(OrderCalculateModelOptions calculateOrderOptions, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new CreateResult());
        }

        public Task<PagedList<UserPaymentMethodModel>> UserPaymentMethodsGetAsync(UserPaymentMethodsFilter filters, CancellationToken cancellationToken = default) =>
            Task.FromResult(new PagedList<UserPaymentMethodModel>(_userPaymentMethods));

        public Task<UserPaymentMethodModel?> UserPaymentMethodGetAsync(int id, CancellationToken cToken = default) =>
            Task.FromResult(_userPaymentMethods.Find(x => x.Id == id));

        public Task<AccountCreationResultModelByMobilePhone> UserCreateByMobileStartAsync(string mobilePhone, ConfirmationCodeDeliveryMethod confirmationCodeDeliveryMethod = ConfirmationCodeDeliveryMethod.Undetermined, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AccountCreationResultModelByMobilePhone());
        }

        public Task<PasswordRecoveryStartResultModelByMobile> UserPasswordRecoveryByMobileStartAsync(string username, Gizmo.ConfirmationCodeDeliveryMethod confirmationCodeDeliveryMethod = Gizmo.ConfirmationCodeDeliveryMethod.Undetermined, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PasswordRecoveryStartResultModelByMobile());
        }

        public Task<PagedList<UserApplicationEnterpriseModel>> UserApplicationEnterprisesGetAsync(UserApplicationEnterprisesFilter filters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedList<UserApplicationEnterpriseModel>(_applicationEnterprises));
        }

        public Task<UserApplicationEnterpriseModel?> UserApplicationEnterpriseGetAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = _applicationEnterprises.Find(x => x.Id == id);
            return Task.FromResult(item);
        }

        public Task<PagedList<UserApplicationCategoryModel>> UserApplicationCategoriesGetAsync(UserApplicationCategoriesFilter filters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedList<UserApplicationCategoryModel>(_userApplicationCategories));
        }

        public Task<UserApplicationCategoryModel?> UserApplicationCategoryGetAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = _userApplicationCategories.Find(x => x.Id == id);
            return Task.FromResult(item);
        }

        public Task<PagedList<UserApplicationModel>> UserApplicationsGetAsync(UserApplicationsFilter filters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedList<UserApplicationModel>(_userApplications));
        }

        public Task<PagedList<UserApplicationLinkModel>> UserApplicationLinksGetAsync(UserApplicationLinksFilter filters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedList<UserApplicationLinkModel>(_userApplicationLinks));
        }

        public Task<UserApplicationLinkModel?> UserApplicationLinkGetAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = _userApplicationLinks.Find(x => x.Id == id);
            return Task.FromResult(item);
        }

        public Task<UserApplicationModel?> UserApplicationGetAsync(int id, CancellationToken cancellationToken = default)
        {
            var app = _userApplications.Find(x => x.Id == id);
            return Task.FromResult(app);
        }

        public Task<PagedList<UserExecutableModel>> UserExecutablesGetAsync(UserExecutablesFilter filters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedList<UserExecutableModel>(_userExecutables));
        }

        public Task<UserExecutableModel?> UserExecutableGetAsync(int id, CancellationToken cancellationToken = default)
        {
            var exe = _userExecutables.Find(x => x.Id == id);
            return Task.FromResult(exe);
        }

        public Task<PagedList<UserPersonalFileModel>> UserPersonalFilesGetAsync(UserPersonalFilesFilter filters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PagedList<UserPersonalFileModel>(_personalFiles));
        }

        public Task<UserPersonalFileModel?> UserPersonalFileGetAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = _personalFiles.Find(x => x.Id == id);
            return Task.FromResult(item);
        }

        public Task<UpdateResult> UserPasswordUpdateAsync(string oldPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new UpdateResult());
        }

        public Task<IAppExecutionContextResult> AppExeExecutionContextGetAsync(int appExeId, CancellationToken cancellationToken)
        {
            
            throw new NotImplementedException();
        }

        public Task<string> AppExePathGetAsync(int appExeId, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> AppExeFileExistsAsync(int appExeId, bool ignoreDeployments = false, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<bool> PersonalFileExistAsync(int appExeId, int personalFileId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<string> PersonalFilePathGetAsync(int appExeId, int personalFileId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> AppExePassAgeRatingAsync(int appExeId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<bool> AppExeExecutionLimitPassAsync(int appExeId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<RegistrationVerificationMethod> GetRegistrationVerificationMethodAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(RegistrationVerificationMethod.None);
        }
    }
}
