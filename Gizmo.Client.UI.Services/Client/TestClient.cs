using Gizmo.Client.UI.View.States;
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

        public bool IsConnected => true;

        public bool IsConnecting => false;

        public int Number => 100;

        public bool IsOutOfOrder => false;

        public bool IsInputLocked => false;

        public event EventHandler<ClientExecutionContextStateArgs>? ExecutionContextStateChange;
        public event EventHandler<UserLoginStateChangeEventArgs>? LoginStateChange;
        public event EventHandler<UserBalanceEventArgs>? UserBalanceChange;
        public event EventHandler<UserIdleEventArgs>? UserIdleChange;
        public event EventHandler<AppEnterpriseChangeEventArgs>? AppEnterpriseChange;
        public event EventHandler<AppCategoryChangeEventArgs>? AppCategoryChange;
        public event EventHandler<AppChangeEventArgs>? AppChange;
        public event EventHandler<AppExeChangeEventArgs>? AppExeChange;
        public event EventHandler<FeedChangeEventArgs>? FeedChange;
        public event EventHandler<NewsChangeEventArgs>? NewsChange;
        public event EventHandler<PersonalFileChangeEventArgs> PersonalFileChange;
        public event EventHandler<AppLinkChangeEventArgs> AppLinkChange;
        public event EventHandler<ConnectionStateEventArgs>? ConnectionStateChange;
        public event EventHandler<LockStateEventArgs> LockStateChange;
        public event EventHandler<OutOfOrderStateEventArgs> OutOfOrderStateChange;

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
                ImageId = random.Next(0, 2) == 1 ? i : null,
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
                new UserPaymentMethodModel() { Id = -1, Name= "Cash", DisplayOrder = 0 },
                new UserPaymentMethodModel() { Id = -2, Name= "Credit card", DisplayOrder = 0 },
                new UserPaymentMethodModel() { Id = -3, Name= "Balance", DisplayOrder = 0 },
                new UserPaymentMethodModel() { Id = 1, Name= "Online", DisplayOrder = 0, IsOnline = true },
            };
            #endregion

            #region ADVERTISMENT
            _newsModel = new()
            {
                { new ()
                    {
                        Id = 1,
                        Title = "DEFAULT VIDEO",
                        Data = "Action with custom media",
                        MediaUrl = "https://media.geeksforgeeks.org/wp-content/uploads/20210314115545/sample-video.mp4",
                        Url = "gizmo://products/cart/add?productId=1&size=2"
                    }
                },
                { new ()
                    {
                        Id = 2,
                        Title = "DEFAULT VIDEO",
                        Data = "Action with thumb error",
                        //MediaUrl = "https://media.geeksforgeeks.org/wp-content/uploads/20210314115545/sample-video.mp4",
                        ThumbnailUrl = "http://localhost/test.png",
                        Url = "gizmo://products/cart/add?productId=1&size=2"
                    }
                },
                { new ()
                    {
                        Id = 3,
                        Title = "DEFAULT VIDEO",
                        Data = "Action with thumb",
                        ThumbnailUrl = "https://i3.ytimg.com/vi/Ce1eUo0K3VE/maxresdefault.jpg",
                        Url = "gizmo://products/cart/add?productId=1&size=2"
                    }
                },
                { new ()
                    {
                        Id = 4,
                        Title = "GTA - 5",
                        Data = "GTA - 5 VK",
                        ThumbnailUrl = "https://i3.ytimg.com/vi/Ce1eUo0K3VE/maxresdefault.jpg",
                        MediaUrl = "https://vk.com/video_ext.php?oid=-2000182257&id=118182257&hash=0f8faf02a738549a&hd=2",
                        Url = "gizmo://products/details/navigate?productId=1"
                    }
                },
                { new ()
                    {
                        Id = 5,
                        Title = "CRYSIS - 4",
                        Data = "<div style=\"max-width: 40.0rem; margin: 8.6rem 3.2rem 6.5rem 3.2rem\">Youtube with url #1 Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.#1 Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</div>",
                        MediaUrl = "https://www.youtube.com/watch?v=TsAaH8yqB70&ab_channel=Punish",
                        Url = "https://www.theloadout.com/crysis-4/release-date"
                    }
                }
            };
            #endregion
        }

        public async Task<LoginResult> UserLoginAsync(string loginName, string? password, CancellationToken cancellationToken)
        {
            LoginStateChange?.Invoke(this, new UserLoginStateChangeEventArgs(null, LoginState.LoggingIn));
            await Task.Delay(new Random().Next(100, 1000), cancellationToken);

            LoginStateChange?.Invoke(this, new UserLoginStateChangeEventArgs(null, LoginState.LoggedIn));

            LoginStateChange?.Invoke(this, new UserLoginStateChangeEventArgs(null, LoginState.LoginCompleted));
            return LoginResult.Sucess;
        }

        public async Task UserLogoutAsync(CancellationToken cancellationToken)
        {
            LoginStateChange?.Invoke(this, new UserLoginStateChangeEventArgs(null, LoginState.LoggingOut));
            await Task.Delay(3000, cancellationToken);
            LoginStateChange?.Invoke(this, new UserLoginStateChangeEventArgs(null, LoginState.LoggedOut));
        }

        public Task<PagedList<UserAgreementModel>> UserAgreementsGetAsync(UserAgreementsFilter filter, CancellationToken cancellationToken = default)
        {
            var userAgreements = Enumerable.Range(1, 3).Select(i => new UserAgreementModel()
            {
                Id = i,
                Name = $"User agreement {i}",
                Agreement = $"#{i} Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                IsRejectable = i > 1
            }).ToList();

            var pagedList = new PagedList<UserAgreementModel>(userAgreements);

            return Task.FromResult(pagedList);
        }

        public Task<PagedList<UserAgreementModel>> UserAgreementsPendingGetAsync(UserAgreementsFilter filter, CancellationToken cancellationToken = default)
        {
            var userAgreements = Enumerable.Range(1, 3).Select(i => new UserAgreementModel()
            {
                Id = i,
                Name = $"User agreement {i}",
                Agreement = $"#{i} Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                IsRejectable = i > 1
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

        public async Task<bool> UserEmailExistAsync(string email, CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            throw new Exception("Test");

            return email == "1" ? true : false;
        }

        public async Task<bool> UserMobileExistAsync(string mobilePhone, CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            //throw new Exception("Test");

            return mobilePhone == "1" ? true : false;
        }

        public Task<UserModelRequiredInfo?> UserGroupDefaultRequiredInfoGetAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult((UserModelRequiredInfo?)new UserModelRequiredInfo());
        }

        public async Task<AccountCreationCompleteResultModel> UserCreateCompleteAsync(UserProfileModelCreate user, string password, List<UserAgreementModelState> agreementStates, CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            throw new Exception("Test");

            return new AccountCreationCompleteResultModel();
        }

        public async Task<AccountCreationCompleteResultModelByToken> UserCreateByTokenCompleteAsync(string token, UserProfileModelCreate user, string password, List<UserAgreementModelState> agreementStates, CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            throw new Exception("Test");

            return new AccountCreationCompleteResultModelByToken();
        }

        public Task<UserBalanceModel> UserBalanceGetAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new UserBalanceModel()
            {

            });
        }

        public Task<UserProfileModel> UserProfileGetAsync(CancellationToken cancellationToken = default)
        {
            throw new Exception();
            return Task.FromResult(new UserProfileModel()
            {
                Username = "#Test Username",
                FirstName = "#Test First Name",
                LastName = "#Test Last Name",
                BirthDate = new DateTime(1950, 1, 2),
                Sex = Sex.Male,
                Country = "Samoa",
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

        public async Task<bool> UserExistAsync(string userNameEmailOrMobile, CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            //throw new Exception("Test");

            return userNameEmailOrMobile == "1" ? true : false;
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

        public async Task<PaymentIntentCreateResultModel> PaymentIntentCreateAsync(PaymentIntentCreateParametersDepositModel parameters, CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            return new PaymentIntentCreateResultModel()
            {
                PaymentUrl = "https://blazor.net",
                //NativeQrImage = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4KPCFET0NUWVBFIHN2ZyBQVUJMSUMgIi0vL1czQy8vRFREIFNWRyAxLjEvL0VOIiAiaHR0cDovL3d3dy53My5vcmcvR3JhcGhpY3MvU1ZHLzEuMS9EVEQvc3ZnMTEuZHRkIj4KPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZlcnNpb249IjEuMSIgdmlld0JveD0iMCAwIDg1IDg1IiBzdHJva2U9Im5vbmUiPgoJPHJlY3Qgd2lkdGg9IjEwMCUiIGhlaWdodD0iMTAwJSIgZmlsbD0iI2ZmZmZmZiIvPgoJPHBhdGggZD0iTTAsMGg3djFoLTd6IE05LDBoMnYxaC0yeiBNMTIsMGgxdjJoLTF6IE0xOSwwaDF2MWgtMXogTTI1LDBoNHYxaC00eiBNMzIsMGg5djFoLTl6IE00MiwwaDJ2MWgtMnogTTQ2LDBoMXYxaC0xeiBNNDksMGgydjFoLTJ6IE01MywwaDF2MWgtMXogTTU1LDBoM3YxaC0zeiBNNjIsMGgxdjFoLTF6IE02NCwwaDF2MWgtMXogTTY2LDBoMXYxaC0xeiBNNjgsMGgydjFoLTJ6IE03MiwwaDF2MWgtMXogTTc0LDBoMXYzaC0xeiBNNzYsMGgxdjFoLTF6IE03OCwwaDd2MWgtN3ogTTAsMWgxdjZoLTF6IE02LDFoMXY2aC0xeiBNMTAsMWgxdjFoLTF6IE0xNSwxaDR2MWgtNHogTTIwLDFoMnYyaC0yeiBNMjMsMWgxdjFoLTF6IE0yNSwxaDF2MmgtMXogTTI5LDFoM3YxaC0zeiBNMzUsMWgxdjRoLTF6IE0zOSwxaDJ2MmgtMnogTTQxLDFoMXYxaC0xeiBNNTQsMWgxdjJoLTF6IE01NiwxaDF2MWgtMXogTTU4LDFoM3YyaC0zeiBNNjEsMWgxdjFoLTF6IE02MywxaDF2M2gtMXogTTY1LDFoMXYzaC0xeiBNNjcsMWgxdjFoLTF6IE02OSwxaDJ2MWgtMnogTTc1LDFoMXYxaC0xeiBNNzgsMWgxdjZoLTF6IE04NCwxaDF2NmgtMXogTTIsMmgzdjNoLTN6IE05LDJoMXYzaC0xeiBNMTMsMmgydjFoLTJ6IE0xNiwyaDN2MWgtM3ogTTIyLDJoMXYyaC0xeiBNMjQsMmgxdjFoLTF6IE0yNywyaDF2MmgtMXogTTMwLDJoMXYzaC0xeiBNMzQsMmgxdjJoLTF6IE0zNiwyaDF2NWgtMXogTTQ3LDJoM3YzaC0zeiBNNTAsMmgydjFoLTJ6IE02MiwyaDF2MWgtMXogTTY0LDJoMXY2aC0xeiBNNzAsMmgxdjFoLTF6IE03MiwyaDF2MWgtMXogTTgwLDJoM3YzaC0zeiBNMTAsM2gydjJoLTJ6IE0xMiwzaDF2MWgtMXogTTE3LDNoM3YxaC0zeiBNMjEsM2gxdjJoLTF6IE0yNiwzaDF2NWgtMXogTTI4LDNoMXY3aC0xeiBNMjksM2gxdjJoLTF6IE0zMiwzaDF2MTFoLTF6IE0zMywzaDF2MWgtMXogTTM5LDNoMXYxaC0xeiBNNDIsM2gxdjJoLTF6IE00NCwzaDF2MmgtMXogTTUyLDNoMXY3aC0xeiBNNTUsM2gxdjJoLTF6IE01NywzaDF2MWgtMXogTTYwLDNoMnYxaC0yeiBNNzUsM2gxdjFoLTF6IE04LDRoMXYxaC0xeiBNMTMsNGgxdjFoLTF6IE0xNyw0aDF2MWgtMXogTTIwLDRoMXYxaC0xeiBNMjQsNGgxdjFoLTF6IE0zMSw0aDF2MWgtMXogTTQxLDRoMXYxaC0xeiBNNDUsNGgydjFoLTJ6IE01MCw0aDF2MWgtMXogTTUzLDRoMnYxaC0yeiBNNTYsNGgxdjVoLTF6IE01OSw0aDF2MWgtMXogTTYyLDRoMXYzaC0xeiBNNjcsNGgydjJoLTJ6IE03Myw0aDJ2MWgtMnogTTExLDVoMXYxaC0xeiBNMTQsNWgzdjFoLTN6IE0xOCw1aDF2M2gtMXogTTE5LDVoMXYxaC0xeiBNMjMsNWgxdjFoLTF6IE0zMyw1aDF2MWgtMXogTTM3LDVoMXYxaC0xeiBNMzksNWgydjFoLTJ6IE00Niw1aDJ2MWgtMnogTTU3LDVoMXYxaC0xeiBNNjAsNWgxdjNoLTF6IE02Niw1aDF2MmgtMXogTTY5LDVoMXYxaC0xeiBNNzUsNWgxdjFoLTF6IE0xLDZoNXYxaC01eiBNOCw2aDF2NWgtMXogTTEwLDZoMXYxaC0xeiBNMTIsNmgxdjJoLTF6IE0xNCw2aDF2MWgtMXogTTE2LDZoMXYxaC0xeiBNMjAsNmgxdjJoLTF6IE0yMiw2aDF2MWgtMXogTTI0LDZoMXY0aC0xeiBNMzAsNmgxdjFoLTF6IE0zNCw2aDF2M2gtMXogTTM4LDZoMXYzaC0xeiBNNDAsNmgxdjJoLTF6IE00Miw2aDF2MWgtMXogTTQ0LDZoMXY1aC0xeiBNNDYsNmgxdjFoLTF6IE00OCw2aDF2MmgtMXogTTUwLDZoMXYxaC0xeiBNNTQsNmgxdjFoLTF6IE01OCw2aDF2M2gtMXogTTY4LDZoMXYxaC0xeiBNNzAsNmgxdjFoLTF6IE03Miw2aDF2M2gtMXogTTc0LDZoMXYyaC0xeiBNNzYsNmgxdjJoLTF6IE03OSw2aDV2MWgtNXogTTksN2gxdjFoLTF6IE0xMSw3aDF2MWgtMXogTTE1LDdoMXYxaC0xeiBNMjEsN2gxdjJoLTF6IE0yMyw3aDF2MWgtMXogTTM3LDdoMXYyaC0xeiBNMzksN2gxdjFoLTF6IE00MSw3aDF2NGgtMXogTTQ1LDdoMXYxaC0xeiBNNDcsN2gxdjFoLTF6IE00OSw3aDF2MWgtMXogTTU3LDdoMXYxaC0xeiBNNjMsN2gxdjFoLTF6IE03NSw3aDF2NGgtMXogTTIsOGgydjFoLTJ6IE02LDhoMnYxaC0yeiBNMTcsOGgxdjJoLTF6IE0yNyw4aDF2NWgtMXogTTI5LDhoM3YxaC0zeiBNMzYsOGgxdjJoLTF6IE00Miw4aDJ2MWgtMnogTTQ2LDhoMXYyaC0xeiBNNTAsOGgydjFoLTJ6IE01Myw4aDN2MWgtM3ogTTYyLDhoMXYyaC0xeiBNNjUsOGgzdjFoLTN6IE02OSw4aDF2NWgtMXogTTc3LDhoMnYxaC0yeiBNODAsOGgxdjFoLTF6IE0xLDloMXYxaC0xeiBNNCw5aDF2MWgtMXogTTEwLDloMnYyaC0yeiBNMTMsOWg0djJoLTR6IE0xOSw5aDJ2MWgtMnogTTIzLDloMXYxaC0xeiBNMzEsOWgxdjFoLTF6IE0zOSw5aDF2MWgtMXogTTQzLDloMXYxaC0xeiBNNDUsOWgxdjFoLTF6IE00Nyw5aDR2MWgtNHogTTU0LDloMXYzaC0xeiBNNTcsOWgxdjFoLTF6IE03MCw5aDF2M2gtMXogTTc0LDloMXYxaC0xeiBNODEsOWgxdjJoLTF6IE04NCw5aDF2M2gtMXogTTIsMTBoMnYxaC0yeiBNNSwxMGgydjFoLTJ6IE05LDEwaDF2MWgtMXogTTEyLDEwaDF2MmgtMXogTTE5LDEwaDF2MWgtMXogTTIxLDEwaDF2MWgtMXogTTI2LDEwaDF2MWgtMXogTTMzLDEwaDF2MWgtMXogTTM1LDEwaDF2MmgtMXogTTM3LDEwaDJ2MmgtMnogTTQwLDEwaDF2MWgtMXogTTQyLDEwaDF2NGgtMXogTTQ3LDEwaDF2M2gtMXogTTQ5LDEwaDF2MWgtMXogTTUxLDEwaDF2MmgtMXogTTU1LDEwaDJ2MWgtMnogTTU4LDEwaDR2MWgtNHogTTY4LDEwaDF2MmgtMXogTTczLDEwaDF2M2gtMXogTTc2LDEwaDF2MWgtMXogTTMsMTFoMnYxaC0yeiBNNywxMWgxdjJoLTF6IE0xMCwxMWgxdjJoLTF6IE0xNCwxMWgydjFoLTJ6IE0yMCwxMWgxdjJoLTF6IE0yMywxMWgxdjJoLTF6IE0yOCwxMWg0djJoLTR6IE0zNCwxMWgxdjJoLTF6IE0zNiwxMWgxdjJoLTF6IE0zOSwxMWgxdjRoLTF6IE00NiwxMWgxdjZoLTF6IE00OCwxMWgxdjJoLTF6IE01MCwxMWgxdjFoLTF6IE01MiwxMWgxdjNoLTF6IE01NiwxMWgxdjJoLTF6IE01OSwxMWgxdjFoLTF6IE02MSwxMWgydjFoLTJ6IE02NSwxMWgxdjFoLTF6IE02NywxMWgxdjFoLTF6IE03MiwxMWgxdjFoLTF6IE04MiwxMWgxdjFoLTF6IE0wLDEyaDF2MmgtMXogTTIsMTJoMXYyaC0xeiBNNCwxMmgxdjNoLTF6IE02LDEyaDF2MWgtMXogTTksMTJoMXYyaC0xeiBNMTUsMTJoMXYyaC0xeiBNMTcsMTJoMnYxaC0yeiBNMjIsMTJoMXYyaC0xeiBNMjQsMTJoMXYxaC0xeiBNMjYsMTJoMXYyaC0xeiBNMzMsMTJoMXYzaC0xeiBNMzcsMTJoMXYyaC0xeiBNNDAsMTJoMnYyaC0yeiBNNDksMTJoMXYzaC0xeiBNNTMsMTJoMXYyaC0xeiBNNTgsMTJoMXYxaC0xeiBNNjEsMTJoMXYxaC0xeiBNNjYsMTJoMXYzaC0xeiBNNzQsMTJoMXYyaC0xeiBNNzYsMTJoMXYxaC0xeiBNNzgsMTJoMXYyaC0xeiBNODEsMTJoMXYxaC0xeiBNODMsMTJoMXYzaC0xeiBNMywxM2gxdjJoLTF6IE04LDEzaDF2M2gtMXogTTEzLDEzaDJ2MWgtMnogTTE2LDEzaDF2MWgtMXogTTI1LDEzaDF2MmgtMXogTTMwLDEzaDF2MmgtMXogTTM1LDEzaDF2MmgtMXogTTQzLDEzaDF2MmgtMXogTTUxLDEzaDF2MmgtMXogTTYwLDEzaDF2MmgtMXogTTYyLDEzaDR2MWgtNHogTTY3LDEzaDJ2MWgtMnogTTcwLDEzaDN2MWgtM3ogTTc3LDEzaDF2NGgtMXogTTc5LDEzaDJ2MmgtMnogTTgyLDEzaDF2MWgtMXogTTg0LDEzaDF2MWgtMXogTTEsMTRoMXYxaC0xeiBNNSwxNGgydjFoLTJ6IE0xMCwxNGgzdjFoLTN6IE0xOCwxNGgxdjJoLTF6IE0yMSwxNGgxdjFoLTF6IE0yNCwxNGgxdjNoLTF6IE0yNywxNGgxdjFoLTF6IE0zNCwxNGgxdjJoLTF6IE0zNiwxNGgxdjNoLTF6IE0zOCwxNGgxdjJoLTF6IE00MCwxNGgxdjFoLTF6IE00OCwxNGgxdjJoLTF6IE01MCwxNGgxdjFoLTF6IE02MSwxNGgzdjFoLTN6IE02NSwxNGgxdjFoLTF6IE02NywxNGgxdjFoLTF6IE03MiwxNGgxdjFoLTF6IE03NiwxNGgxdjFoLTF6IE04MSwxNGgxdjFoLTF6IE0wLDE1aDF2MWgtMXogTTIsMTVoMXYxaC0xeiBNNywxNWgxdjNoLTF6IE0xMiwxNWgxdjFoLTF6IE0xNCwxNWgxdjFoLTF6IE0xNiwxNWgxdjFoLTF6IE0xOSwxNWgxdjFoLTF6IE0yMywxNWgxdjFoLTF6IE0yNiwxNWgxdjFoLTF6IE0zMSwxNWgydjFoLTJ6IE0zNywxNWgxdjFoLTF6IE00MiwxNWgxdjFoLTF6IE00NSwxNWgxdjFoLTF6IE01NCwxNWgydjFoLTJ6IE01NywxNWgzdjFoLTN6IE02MSwxNWgxdjFoLTF6IE02MywxNWgxdjFoLTF6IE02OCwxNWgxdjFoLTF6IE03MCwxNWgxdjFoLTF6IE03NCwxNWgydjFoLTJ6IE03OCwxNWgydjJoLTJ6IE0xLDE2aDF2M2gtMXogTTMsMTZoMXY1aC0xeiBNNCwxNmgzdjFoLTN6IE05LDE2aDF2M2gtMXogTTEwLDE2aDF2MWgtMXogTTEzLDE2aDF2MmgtMXogTTE1LDE2aDF2MWgtMXogTTIyLDE2aDF2MmgtMXogTTI3LDE2aDJ2MWgtMnogTTM1LDE2aDF2MmgtMXogTTQwLDE2aDF2NmgtMXogTTQxLDE2aDF2MWgtMXogTTQzLDE2aDF2MWgtMXogTTQ3LDE2aDF2MWgtMXogTTUxLDE2aDF2NGgtMXogTTU1LDE2aDN2MWgtM3ogTTU5LDE2aDJ2MWgtMnogTTYyLDE2aDF2MmgtMXogTTY0LDE2aDF2MmgtMXogTTcxLDE2aDN2MmgtM3ogTTc1LDE2aDJ2MWgtMnogTTgyLDE2aDF2MWgtMXogTTAsMTdoMXYxaC0xeiBNMiwxN2gxdjJoLTF6IE01LDE3aDF2NWgtMXogTTgsMTdoMXYzaC0xeiBNMTIsMTdoMXYyaC0xeiBNMTYsMTdoMXYxaC0xeiBNMTgsMTdoMXYyaC0xeiBNMjYsMTdoMXYxaC0xeiBNMjgsMTdoMnYxaC0yeiBNMzEsMTdoNHYxaC00eiBNMzcsMTdoMXYxaC0xeiBNNDIsMTdoMXYxaC0xeiBNNTMsMTdoMnYyaC0yeiBNNTYsMTdoMXYxaC0xeiBNNjAsMTdoMXY1aC0xeiBNNjEsMTdoMXYyaC0xeiBNNjYsMTdoMXYxaC0xeiBNNjgsMTdoMXYyaC0xeiBNNzAsMTdoMXYyaC0xeiBNODEsMTdoMXYxaC0xeiBNODQsMTdoMXY1aC0xeiBNNiwxOGgxdjFoLTF6IE0xMCwxOGgydjFoLTJ6IE0xNSwxOGgxdjFoLTF6IE0xNywxOGgxdjFoLTF6IE0yMCwxOGgxdjJoLTF6IE0yMywxOGgxdjNoLTF6IE0yNSwxOGgxdjFoLTF6IE0yOSwxOGgydjFoLTJ6IE0zMiwxOGgxdjNoLTF6IE0zNiwxOGgxdjJoLTF6IE0zOCwxOGgydjFoLTJ6IE00NCwxOGgxdjJoLTF6IE00NywxOGgxdjNoLTF6IE00OSwxOGgxdjFoLTF6IE01NSwxOGgxdjJoLTF6IE01OCwxOGgxdjFoLTF6IE02MywxOGgxdjJoLTF6IE02NSwxOGgxdjFoLTF6IE03MiwxOGgxdjZoLTF6IE03NCwxOGgxdjJoLTF6IE03NywxOGgydjNoLTJ6IE03OSwxOGgydjFoLTJ6IE04MywxOGgxdjJoLTF6IE0wLDE5aDF2MWgtMXogTTQsMTloMXYzaC0xeiBNMTYsMTloMXY0aC0xeiBNMTksMTloMXYxaC0xeiBNMjIsMTloMXYxaC0xeiBNMzEsMTloMXYxaC0xeiBNMzQsMTloMXY0aC0xeiBNMzUsMTloMXYxaC0xeiBNMzcsMTloMXY0aC0xeiBNMzksMTloMXYxaC0xeiBNNDEsMTloMXYxaC0xeiBNNDUsMTloMXYxaC0xeiBNNTIsMTloMXYxaC0xeiBNNTcsMTloMXYxaC0xeiBNNjQsMTloMXY0aC0xeiBNNjYsMTloMnYxaC0yeiBNNjksMTloMXYyaC0xeiBNNzEsMTloMXYxaC0xeiBNNzUsMTloMXYxaC0xeiBNODAsMTloMXYxaC0xeiBNMiwyMGgxdjFoLTF6IE02LDIwaDF2MWgtMXogTTksMjBoMXYxaC0xeiBNMTIsMjBoMnYxaC0yeiBNMTUsMjBoMXYxaC0xeiBNMTcsMjBoMXYxaC0xeiBNMjEsMjBoMXYzaC0xeiBNMjQsMjBoMnYxaC0yeiBNMjcsMjBoMXYzaC0xeiBNMjgsMjBoMXYxaC0xeiBNMzMsMjBoMXYzaC0xeiBNMzgsMjBoMXY0aC0xeiBNNDMsMjBoMXYxaC0xeiBNNDgsMjBoMXYxaC0xeiBNNTMsMjBoMnY0aC0yeiBNNTYsMjBoMXYxaC0xeiBNNTgsMjBoMXY0aC0xeiBNNjIsMjBoMXYyaC0xeiBNNjcsMjBoMnYxaC0yeiBNNzMsMjBoMXYxaC0xeiBNNzksMjBoMXYxaC0xeiBNMSwyMWgxdjFoLTF6IE04LDIxaDF2MmgtMXogTTEzLDIxaDF2MmgtMXogTTI0LDIxaDF2MWgtMXogTTMxLDIxaDF2MWgtMXogTTM1LDIxaDJ2MWgtMnogTTM5LDIxaDF2MmgtMXogTTQxLDIxaDJ2MWgtMnogTTQ1LDIxaDF2MWgtMXogTTQ5LDIxaDF2NmgtMXogTTUxLDIxaDF2MTJoLTF6IE01NSwyMWgxdjJoLTF6IE01NywyMWgxdjFoLTF6IE01OSwyMWgxdjNoLTF6IE02MywyMWgxdjFoLTF6IE03NCwyMWgydjFoLTJ6IE04MiwyMWgxdjVoLTF6IE0wLDIyaDF2MmgtMXogTTIsMjJoMXYzaC0xeiBNNiwyMmgydjFoLTJ6IE0xMiwyMmgxdjJoLTF6IE0xNSwyMmgxdjFoLTF6IE0xNywyMmgxdjNoLTF6IE0yMCwyMmgxdjFoLTF6IE0yMywyMmgxdjNoLTF6IE0yNSwyMmgxdjFoLTF6IE0zNiwyMmgxdjFoLTF6IE00NCwyMmgxdjFoLTF6IE00NywyMmgydjFoLTJ6IE01NiwyMmgxdjFoLTF6IE02MSwyMmgxdjFoLTF6IE02NiwyMmgxdjRoLTF6IE03MCwyMmgxdjJoLTF6IE03NiwyMmgxdjFoLTF6IE03OCwyMmgxdjFoLTF6IE04MCwyMmgydjFoLTJ6IE0xLDIzaDF2NGgtMXogTTMsMjNoMnYxaC0yeiBNMTAsMjNoMXYyaC0xeiBNMTksMjNoMXYyaC0xeiBNMjQsMjNoMXYxaC0xeiBNMjgsMjNoM3YxaC0zeiBNNDAsMjNoMnYxaC0yeiBNNDMsMjNoMXY0aC0xeiBNNDYsMjNoMXYyaC0xeiBNNTcsMjNoMXYxaC0xeiBNNjAsMjNoMXYxaC0xeiBNNjUsMjNoMXYxaC0xeiBNNjcsMjNoMnYxaC0yeiBNNzEsMjNoMXYxaC0xeiBNNzMsMjNoMnYxaC0yeiBNODEsMjNoMXYyaC0xeiBNMywyNGgxdjFoLTF6IE02LDI0aDN2MWgtM3ogTTEzLDI0aDN2MmgtM3ogTTE2LDI0aDF2MWgtMXogTTE4LDI0aDF2MWgtMXogTTIxLDI0aDJ2MWgtMnogTTI1LDI0aDF2MWgtMXogTTMxLDI0aDJ2MWgtMnogTTM0LDI0aDF2MWgtMXogTTM2LDI0aDJ2MWgtMnogTTM5LDI0aDF2MWgtMXogTTQyLDI0aDF2M2gtMXogTTQ0LDI0aDF2NGgtMXogTTQ1LDI0aDF2MWgtMXogTTUyLDI0aDJ2MmgtMnogTTU2LDI0aDF2MWgtMXogTTYxLDI0aDF2MWgtMXogTTYzLDI0aDF2MmgtMXogTTY4LDI0aDF2MWgtMXogTTczLDI0aDF2MWgtMXogTTc1LDI0aDJ2MWgtMnogTTgwLDI0aDF2M2gtMXogTTg0LDI0aDF2MWgtMXogTTQsMjVoMnY0aC0yeiBNMjIsMjVoMXYyaC0xeiBNMjQsMjVoMXYxaC0xeiBNMjksMjVoM3YxaC0zeiBNMzMsMjVoMXYxaC0xeiBNMzUsMjVoMnYxaC0yeiBNMzgsMjVoMXYxaC0xeiBNNDAsMjVoMXYxaC0xeiBNNTAsMjVoMXYyaC0xeiBNNTQsMjVoMnYxaC0yeiBNNTgsMjVoM3YxaC0zeiBNNjIsMjVoMXYyaC0xeiBNNjQsMjVoMnYzaC0yeiBNNjcsMjVoMXYyaC0xeiBNNzAsMjVoMXY5aC0xeiBNNzIsMjVoMXY1aC0xeiBNNzQsMjVoMXY0aC0xeiBNNzYsMjVoMnYyaC0yeiBNODMsMjVoMXYyaC0xeiBNMiwyNmgydjFoLTJ6IE02LDI2aDJ2MWgtMnogTTksMjZoNHYyaC00eiBNMTQsMjZoMXYxaC0xeiBNMTYsMjZoMnYyaC0yeiBNMTksMjZoM3YyaC0zeiBNMjgsMjZoMXY3aC0xeiBNMjksMjZoMXYxaC0xeiBNMzIsMjZoMXYxaC0xeiBNMzQsMjZoMXYyaC0xeiBNNDUsMjZoMXY1aC0xeiBNNTIsMjZoMXYxaC0xeiBNNTYsMjZoMXY3aC0xeiBNNTcsMjZoMXYyaC0xeiBNNjEsMjZoMXYyaC0xeiBNNjgsMjZoMXY2aC0xeiBNNjksMjZoMXYyaC0xeiBNNzMsMjZoMXYxaC0xeiBNODEsMjZoMXYxaC0xeiBNNywyN2gydjJoLTJ6IE0xMywyN2gxdjFoLTF6IE0yNCwyN2gxdjJoLTF6IE0zMSwyN2gxdjJoLTF6IE0zNiwyN2gxdjFoLTF6IE00MCwyN2gydjFoLTJ6IE00OCwyN2gxdjNoLTF6IE01MywyN2gzdjJoLTN6IE03MSwyN2gxdjNoLTF6IE03NywyN2gxdjJoLTF6IE03OSwyN2gxdjJoLTF6IE0xLDI4aDF2MWgtMXogTTYsMjhoMXYxaC0xeiBNOSwyOGgxdjFoLTF6IE0xMiwyOGgxdjFoLTF6IE0xNiwyOGgxdjFoLTF6IE0xOCwyOGgxdjJoLTF6IE0yMSwyOGgzdjFoLTN6IE0yNywyOGgxdjJoLTF6IE0yOSwyOGgydjFoLTJ6IE0zMiwyOGgxdjVoLTF6IE0zNSwyOGgxdjFoLTF6IE0zOSwyOGgxdjFoLTF6IE00NywyOGgxdjFoLTF6IE00OSwyOGgydjFoLTJ6IE01MiwyOGgxdjhoLTF6IE01OCwyOGgxdjJoLTF6IE02MiwyOGgxdjFoLTF6IE03NSwyOGgydjNoLTJ6IE03OCwyOGgxdjFoLTF6IE04MCwyOGgxdjVoLTF6IE04MywyOGgxdjNoLTF6IE0wLDI5aDF2MmgtMXogTTIsMjloMXYxaC0xeiBNNCwyOWgxdjRoLTF6IE04LDI5aDF2NGgtMXogTTEzLDI5aDJ2MWgtMnogTTE3LDI5aDF2MTFoLTF6IE0yMSwyOWgxdjFoLTF6IE0yNiwyOWgxdjNoLTF6IE00MCwyOWgxdjVoLTF6IE00MSwyOWgxdjFoLTF6IE00MywyOWgxdjFoLTF6IE01OSwyOWgzdjFoLTN6IE02MywyOWgxdjJoLTF6IE02NiwyOWgydjFoLTJ6IE03MywyOWgxdjJoLTF6IE04MSwyOWgydjFoLTJ6IE04NCwyOWgxdjNoLTF6IE02LDMwaDF2MWgtMXogTTEwLDMwaDF2MmgtMXogTTE0LDMwaDF2MmgtMXogTTE5LDMwaDJ2MWgtMnogTTIyLDMwaDF2MmgtMXogTTI0LDMwaDF2MWgtMXogTTMwLDMwaDF2MWgtMXogTTM0LDMwaDF2MWgtMXogTTM2LDMwaDJ2NGgtMnogTTM4LDMwaDJ2MWgtMnogTTQyLDMwaDF2MWgtMXogTTUwLDMwaDF2MWgtMXogTTU0LDMwaDF2MWgtMXogTTU5LDMwaDF2MmgtMXogTTYxLDMwaDF2MWgtMXogTTY0LDMwaDF2NmgtMXogTTY1LDMwaDF2MmgtMXogTTc0LDMwaDF2MWgtMXogTTc4LDMwaDF2MWgtMXogTTgyLDMwaDF2MWgtMXogTTIsMzFoMXYxaC0xeiBNOSwzMWgxdjFoLTF6IE0xMiwzMWgxdjFoLTF6IE0xNiwzMWgxdjFoLTF6IE0xOSwzMWgxdjFoLTF6IE0yMSwzMWgxdjJoLTF6IE0yMywzMWgxdjFoLTF6IE0yNSwzMWgxdjFoLTF6IE0yNywzMWgxdjRoLTF6IE0zMywzMWgxdjFoLTF6IE0zNSwzMWgxdjFoLTF6IE0zOSwzMWgxdjFoLTF6IE00MywzMWgxdjRoLTF6IE00NywzMWgydjFoLTJ6IE01OCwzMWgxdjVoLTF6IE02NywzMWgxdjNoLTF6IE02OSwzMWgxdjFoLTF6IE03MSwzMWgydjFoLTJ6IE03NiwzMWgxdjJoLTF6IE04MSwzMWgxdjFoLTF6IE01LDMyaDN2MWgtM3ogTTEzLDMyaDF2MWgtMXogTTIwLDMyaDF2MmgtMXogTTI5LDMyaDN2MWgtM3ogTTM4LDMyaDF2NGgtMXogTTQxLDMyaDJ2MmgtMnogTTUwLDMyaDF2M2gtMXogTTUzLDMyaDN2MWgtM3ogTTU3LDMyaDF2NGgtMXogTTYwLDMyaDN2MWgtM3ogTTY2LDMyaDF2M2gtMXogTTcyLDMyaDF2MWgtMXogTTc0LDMyaDF2MmgtMXogTTc3LDMyaDJ2MmgtMnogTTc5LDMyaDF2MWgtMXogTTgyLDMyaDJ2MWgtMnogTTAsMzNoMnYxaC0yeiBNMywzM2gxdjJoLTF6IE01LDMzaDF2MWgtMXogTTksMzNoMXY0aC0xeiBNMTEsMzNoMnYxaC0yeiBNMTYsMzNoMXY0aC0xeiBNMTksMzNoMXYyaC0xeiBNMjMsMzNoNHYxaC00eiBNMjksMzNoMXYyaC0xeiBNMzEsMzNoMXYxaC0xeiBNMzUsMzNoMXYzaC0xeiBNMzksMzNoMXYxaC0xeiBNNDYsMzNoM3YxaC0zeiBNNTUsMzNoMXYxaC0xeiBNNjAsMzNoMXYxaC0xeiBNNjksMzNoMXYzaC0xeiBNODEsMzNoMXY1aC0xeiBNODIsMzNoMXYxaC0xeiBNMSwzNGgxdjFoLTF6IE00LDM0aDF2MWgtMXogTTYsMzRoMXYxaC0xeiBNMTAsMzRoMXYzaC0xeiBNMTIsMzRoM3YxaC0zeiBNMTgsMzRoMXYyaC0xeiBNMjIsMzRoMXYxaC0xeiBNMjUsMzRoMnYxaC0yeiBNMzAsMzRoMXY0aC0xeiBNMzcsMzRoMXYxNGgtMXogTTQ0LDM0aDF2MmgtMXogTTQ2LDM0aDF2MWgtMXogTTQ4LDM0aDJ2NmgtMnogTTU0LDM0aDF2MmgtMXogTTU5LDM0aDF2MmgtMXogTTYzLDM0aDF2NWgtMXogTTc2LDM0aDF2MmgtMXogTTgsMzVoMXYxaC0xeiBNMTEsMzVoMXYyaC0xeiBNMTQsMzVoMXYyaC0xeiBNMjAsMzVoMXY2aC0xeiBNMjEsMzVoMXYxaC0xeiBNMjQsMzVoMXY2aC0xeiBNMzMsMzVoMnYxaC0yeiBNNDAsMzVoMXY0aC0xeiBNNDUsMzVoMXYxaC0xeiBNNTEsMzVoMXYyaC0xeiBNNTMsMzVoMXYyaC0xeiBNNTUsMzVoMXYxaC0xeiBNNjEsMzVoMnYxaC0yeiBNNjUsMzVoMXYxaC0xeiBNNjgsMzVoMXYxaC0xeiBNNzAsMzVoMXYyaC0xeiBNNzIsMzVoMXYxaC0xeiBNNzQsMzVoMXY0aC0xeiBNNzUsMzVoMXYxaC0xeiBNNzcsMzVoMXY0aC0xeiBNODIsMzVoM3YxaC0zeiBNMCwzNmgydjFoLTJ6IE00LDM2aDR2MWgtNHogTTE1LDM2aDF2MWgtMXogTTE5LDM2aDF2MWgtMXogTTIzLDM2aDF2MmgtMXogTTI1LDM2aDF2MWgtMXogTTI3LDM2aDF2MWgtMXogTTMxLDM2aDF2MWgtMXogTTMzLDM2aDF2MWgtMXogTTM2LDM2aDF2M2gtMXogTTQxLDM2aDF2MWgtMXogTTQzLDM2aDF2MWgtMXogTTQ3LDM2aDF2MWgtMXogTTUwLDM2aDF2M2gtMXogTTU2LDM2aDF2M2gtMXogTTYyLDM2aDF2NWgtMXogTTY2LDM2aDF2NGgtMXogTTczLDM2aDF2M2gtMXogTTc5LDM2aDF2MWgtMXogTTgyLDM2aDF2MWgtMXogTTg0LDM2aDF2MWgtMXogTTIsMzdoMXYzaC0xeiBNMTgsMzdoMXYzaC0xeiBNMjIsMzdoMXYyaC0xeiBNMjgsMzdoMXY4aC0xeiBNNDQsMzdoMXYxaC0xeiBNNTQsMzdoMXYzaC0xeiBNNTUsMzdoMXYxaC0xeiBNNjUsMzdoMXYxaC0xeiBNNjcsMzdoMnYxaC0yeiBNNzEsMzdoMXYxaC0xeiBNODMsMzdoMXYyaC0xeiBNMCwzOGgxdjJoLTF6IE0zLDM4aDJ2MWgtMnogTTYsMzhoNXYxaC01eiBNMTIsMzhoMXYzaC0xeiBNMTUsMzhoMnYxaC0yeiBNMTksMzhoMXYyaC0xeiBNMjEsMzhoMXYxaC0xeiBNMjYsMzhoMXYyaC0xeiBNMjksMzhoMXYyaC0xeiBNMzEsMzhoMnYxaC0yeiBNMzQsMzhoMXYxaC0xeiBNMzgsMzhoMXYxaC0xeiBNNDEsMzhoMnYxaC0yeiBNNDUsMzhoMXY0aC0xeiBNNjAsMzhoMXYxaC0xeiBNNjQsMzhoMXYyaC0xeiBNNjgsMzhoMnYxaC0yeiBNNzUsMzhoMXYxaC0xeiBNNzgsMzhoMXYzaC0xeiBNMSwzOWgxdjFoLTF6IE00LDM5aDJ2MWgtMnogTTcsMzloMXYxaC0xeiBNOSwzOWgxdjJoLTF6IE0xMSwzOWgxdjFoLTF6IE0xNCwzOWgxdjFoLTF6IE0xNiwzOWgxdjNoLTF6IE0yNSwzOWgxdjNoLTF6IE0yNywzOWgxdjFoLTF6IE0zMiwzOWgydjFoLTJ6IE0zOSwzOWgxdjJoLTF6IE00MiwzOWgxdjFoLTF6IE00NCwzOWgxdjFoLTF6IE00NiwzOWgxdjJoLTF6IE01MiwzOWgydjFoLTJ6IE01NSwzOWgxdjVoLTF6IE01NywzOWgxdjFoLTF6IE02NSwzOWgxdjJoLTF6IE02OCwzOWgxdjFoLTF6IE03MSwzOWgxdjRoLTF6IE04MCwzOWgxdjNoLTF6IE04NCwzOWgxdjFoLTF6IE01LDQwaDF2M2gtMXogTTYsNDBoMXYxaC0xeiBNOCw0MGgxdjJoLTF6IE0xMyw0MGgxdjJoLTF6IE0yMSw0MGgxdjJoLTF6IE0yMyw0MGgxdjFoLTF6IE0zMCw0MGgxdjNoLTF6IE0zNCw0MGgzdjFoLTN6IE0zOCw0MGgxdjFoLTF6IE00MCw0MGgxdjJoLTF6IE00Myw0MGgxdjFoLTF6IE00OCw0MGgxdjFoLTF6IE01Niw0MGgxdjJoLTF6IE01OCw0MGgydjFoLTJ6IE02MSw0MGgxdjNoLTF6IE02OSw0MGgydjFoLTJ6IE03NSw0MGgzdjFoLTN6IE03OSw0MGgxdjJoLTF6IE04Miw0MGgydjFoLTJ6IE0zLDQxaDJ2MWgtMnogTTEwLDQxaDF2MWgtMXogTTE1LDQxaDF2M2gtMXogTTE4LDQxaDJ2MWgtMnogTTIyLDQxaDF2MmgtMXogTTI2LDQxaDJ2MWgtMnogTTMxLDQxaDJ2MWgtMnogTTM1LDQxaDF2MWgtMXogTTQyLDQxaDF2MmgtMXogTTQ0LDQxaDF2M2gtMXogTTQ3LDQxaDF2M2gtMXogTTQ5LDQxaDF2MmgtMXogTTUzLDQxaDF2MWgtMXogTTY0LDQxaDF2MWgtMXogTTY4LDQxaDF2MWgtMXogTTcwLDQxaDF2NGgtMXogTTczLDQxaDF2M2gtMXogTTc1LDQxaDF2MmgtMXogTTgyLDQxaDF2MWgtMXogTTAsNDJoMXY0aC0xeiBNMiw0MmgxdjFoLTF6IE00LDQyaDF2MmgtMXogTTYsNDJoMXYxaC0xeiBNOSw0MmgxdjFoLTF6IE0xNCw0MmgxdjRoLTF6IE0yMCw0MmgxdjFoLTF6IE0yMyw0MmgxdjFoLTF6IE0yNyw0MmgxdjNoLTF6IE0yOSw0MmgxdjFoLTF6IE0zMSw0MmgxdjFoLTF6IE0zMyw0MmgxdjNoLTF6IE0zOCw0MmgxdjFoLTF6IE00MSw0MmgxdjFoLTF6IE00Myw0MmgxdjJoLTF6IE00Niw0MmgxdjFoLTF6IE00OCw0MmgxdjVoLTF6IE01Miw0MmgxdjNoLTF6IE02MCw0MmgxdjRoLTF6IE02Miw0MmgxdjJoLTF6IE02Nyw0MmgxdjFoLTF6IE03NCw0MmgxdjFoLTF6IE03Niw0MmgzdjFoLTN6IE04Myw0MmgydjFoLTJ6IE0zLDQzaDF2M2gtMXogTTEwLDQzaDF2MWgtMXogTTE4LDQzaDJ2MmgtMnogTTI0LDQzaDF2MWgtMXogTTMyLDQzaDF2MmgtMXogTTM0LDQzaDJ2M2gtMnogTTQwLDQzaDF2MWgtMXogTTU0LDQzaDF2NmgtMXogTTU3LDQzaDN2MWgtM3ogTTY0LDQzaDF2MmgtMXogTTY2LDQzaDF2MWgtMXogTTcyLDQzaDF2NWgtMXogTTgxLDQzaDF2MWgtMXogTTIsNDRoMXYxaC0xeiBNNSw0NGg1djFoLTV6IE0xMiw0NGgydjFoLTJ6IE0xNiw0NGgydjFoLTJ6IE0yMCw0NGg0djFoLTR6IE0zMCw0NGgxdjFoLTF6IE0zNiw0NGgxdjFoLTF6IE0zOCw0NGgydjFoLTJ6IE00MSw0NGgxdjFoLTF6IE00NSw0NGgxdjNoLTF6IE00Niw0NGgxdjFoLTF6IE01MSw0NGgxdjJoLTF6IE01Myw0NGgxdjJoLTF6IE01Niw0NGgydjFoLTJ6IE02MSw0NGgxdjFoLTF6IE02Nyw0NGgxdjFoLTF6IE02OSw0NGgxdjJoLTF6IE03NCw0NGgxdjFoLTF6IE03Niw0NGgydjFoLTJ6IE04Myw0NGgxdjFoLTF6IE01LDQ1aDF2MmgtMXogTTcsNDVoMXYxaC0xeiBNOSw0NWgzdjFoLTN6IE0xMyw0NWgxdjNoLTF6IE0xNyw0NWgxdjFoLTF6IE0yMyw0NWgxdjFoLTF6IE0yOSw0NWgxdjJoLTF6IE00MCw0NWgxdjZoLTF6IE00NCw0NWgxdjJoLTF6IE01MCw0NWgxdjFoLTF6IE02OCw0NWgxdjNoLTF6IE03Nyw0NWgydjJoLTJ6IE03OSw0NWgxdjFoLTF6IE04MSw0NWgydjFoLTJ6IE04NCw0NWgxdjFoLTF6IE0xLDQ2aDJ2MWgtMnogTTQsNDZoMXYxaC0xeiBNNiw0NmgxdjFoLTF6IE04LDQ2aDF2MWgtMXogTTEwLDQ2aDN2MWgtM3ogTTE1LDQ2aDF2M2gtMXogTTE2LDQ2aDF2MWgtMXogTTE4LDQ2aDF2M2gtMXogTTI0LDQ2aDF2MmgtMXogTTMwLDQ2aDN2MmgtM3ogTTMzLDQ2aDJ2MWgtMnogTTM4LDQ2aDF2MWgtMXogTTQxLDQ2aDF2MWgtMXogTTQzLDQ2aDF2MmgtMXogTTUyLDQ2aDF2MTdoLTF6IE01OCw0NmgydjJoLTJ6IE02MSw0NmgxdjNoLTF6IE02Myw0NmgydjFoLTJ6IE02Niw0NmgxdjJoLTF6IE03MCw0NmgydjFoLTJ6IE03Myw0NmgxdjJoLTF6IE03NSw0NmgxdjdoLTF6IE03Niw0NmgxdjJoLTF6IE04Miw0NmgxdjNoLTF6IE0yLDQ3aDF2MWgtMXogTTcsNDdoMXYxaC0xeiBNMTEsNDdoMnYxaC0yeiBNMTQsNDdoMXYxaC0xeiBNMjIsNDdoMXY2aC0xeiBNMjYsNDdoMXYyaC0xeiBNMzQsNDdoMnYxaC0yeiBNNDcsNDdoMXYxaC0xeiBNNDksNDdoMXYxaC0xeiBNNTEsNDdoMXYxaC0xeiBNNTUsNDdoMXYxaC0xeiBNNjAsNDdoMXYxaC0xeiBNNjIsNDdoMXYyaC0xeiBNNjcsNDdoMXYxaC0xeiBNNzEsNDdoMXYxaC0xeiBNNzQsNDdoMXYxaC0xeiBNNzcsNDdoMXYxaC0xeiBNNzksNDdoM3YxaC0zeiBNODMsNDdoMXYxaC0xeiBNMCw0OGgydjJoLTJ6IE02LDQ4aDF2MWgtMXogTTgsNDhoMXYyaC0xeiBNMTAsNDhoMXYxaC0xeiBNMTIsNDhoMXYxaC0xeiBNMTYsNDhoMnYxaC0yeiBNMTksNDhoMXY3aC0xeiBNMjAsNDhoMXYyaC0xeiBNMjUsNDhoMXYxaC0xeiBNMjcsNDhoNHYxaC00eiBNMzIsNDhoMXYxaC0xeiBNMzYsNDhoMXYzaC0xeiBNNDEsNDhoMXY5aC0xeiBNNDQsNDhoM3YxaC0zeiBNNTMsNDhoMXYxaC0xeiBNNTcsNDhoMXYxaC0xeiBNNjMsNDhoMnYxaC0yeiBNNjksNDhoMXYxaC0xeiBNNzgsNDhoMnYxaC0yeiBNODQsNDhoMXYyaC0xeiBNMiw0OWgxdjJoLTF6IE01LDQ5aDF2NGgtMXogTTksNDloMXYzaC0xeiBNMTMsNDloMXY1aC0xeiBNMTcsNDloMXYxaC0xeiBNMjMsNDloMnYxaC0yeiBNMjcsNDloMXYyaC0xeiBNMjksNDloMXY0aC0xeiBNMzEsNDloMXYxaC0xeiBNMzMsNDloM3YxaC0zeiBNMzgsNDloMXY0aC0xeiBNNDMsNDloMXYyaC0xeiBNNDUsNDloMnYxaC0yeiBNNDksNDloMnYyaC0yeiBNNTgsNDloMnYzaC0yeiBNNjMsNDloMXYxaC0xeiBNNjUsNDloMXYxaC0xeiBNNzAsNDloMXYxaC0xeiBNNzQsNDloMXYyaC0xeiBNNzcsNDloMXY0aC0xeiBNNzgsNDloMXYxaC0xeiBNODEsNDloMXY0aC0xeiBNMyw1MGgxdjFoLTF6IE02LDUwaDF2MWgtMXogTTEwLDUwaDF2MWgtMXogTTE1LDUwaDJ2MWgtMnogTTIxLDUwaDF2MmgtMXogTTIzLDUwaDF2MWgtMXogTTI1LDUwaDF2MWgtMXogTTI4LDUwaDF2MWgtMXogTTMwLDUwaDF2MWgtMXogTTMyLDUwaDF2OGgtMXogTTMzLDUwaDF2MWgtMXogTTM1LDUwaDF2MWgtMXogTTM3LDUwaDF2MWgtMXogTTQyLDUwaDF2M2gtMXogTTQ3LDUwaDF2NGgtMXogTTU1LDUwaDF2M2gtMXogTTYyLDUwaDF2MWgtMXogTTY0LDUwaDF2M2gtMXogTTY2LDUwaDJ2MmgtMnogTTY5LDUwaDF2MWgtMXogTTcyLDUwaDF2MWgtMXogTTc2LDUwaDF2MWgtMXogTTc5LDUwaDJ2MWgtMnogTTgyLDUwaDF2MWgtMXogTTAsNTFoMnYxaC0yeiBNNCw1MWgxdjhoLTF6IE04LDUxaDF2NmgtMXogTTExLDUxaDF2M2gtMXogTTEyLDUxaDF2MWgtMXogTTE2LDUxaDN2MmgtM3ogTTIwLDUxaDF2MmgtMXogTTI0LDUxaDF2MWgtMXogTTI2LDUxaDF2MmgtMXogTTM0LDUxaDF2MmgtMXogTTM5LDUxaDF2MWgtMXogTTQ0LDUxaDF2MWgtMXogTTQ4LDUxaDF2MmgtMXogTTUxLDUxaDF2MWgtMXogTTU0LDUxaDF2MmgtMXogTTU3LDUxaDF2MmgtMXogTTY1LDUxaDF2MmgtMXogTTY4LDUxaDF2MWgtMXogTTcxLDUxaDF2MmgtMXogTTczLDUxaDF2M2gtMXogTTc4LDUxaDF2MmgtMXogTTg0LDUxaDF2MWgtMXogTTMsNTJoMXYxaC0xeiBNNiw1MmgydjFoLTJ6IE0yMyw1MmgxdjFoLTF6IE0yNyw1MmgydjNoLTJ6IE0zMCw1MmgydjFoLTJ6IE0zMyw1MmgxdjFoLTF6IE0zNSw1MmgxdjNoLTF6IE0zNyw1MmgxdjFoLTF6IE00MCw1MmgxdjRoLTF6IE01MCw1MmgxdjJoLTF6IE01Myw1MmgxdjFoLTF6IE01Niw1MmgxdjZoLTF6IE01OSw1MmgxdjFoLTF6IE02MSw1MmgxdjVoLTF6IE03NCw1MmgxdjFoLTF6IE03Niw1MmgxdjZoLTF6IE03OSw1MmgydjFoLTJ6IE04Miw1MmgxdjFoLTF6IE0wLDUzaDF2MmgtMXogTTIsNTNoMXY1aC0xeiBNMTQsNTNoMXYzaC0xeiBNMTcsNTNoMXYxaC0xeiBNMjEsNTNoMXYzaC0xeiBNMjQsNTNoMXYxaC0xeiBNNDQsNTNoMXYxaC0xeiBNNTEsNTNoMXYxaC0xeiBNNjAsNTNoMXYxaC0xeiBNNjMsNTNoMXYzaC0xeiBNNjYsNTNoMXYxaC0xeiBNNjgsNTNoMXYyaC0xeiBNNzAsNTNoMXYyaC0xeiBNODAsNTNoMXY0aC0xeiBNODQsNTNoMXYzaC0xeiBNNiw1NGgxdjFoLTF6IE0xMCw1NGgxdjVoLTF6IE0xMiw1NGgxdjFoLTF6IE0yMCw1NGgxdjVoLTF6IE0yMyw1NGgxdjFoLTF6IE0yNiw1NGgxdjFoLTF6IE0zMCw1NGgxdjFoLTF6IE0zMyw1NGgxdjFoLTF6IE0zOCw1NGgydjFoLTJ6IE00Myw1NGgxdjJoLTF6IE00Niw1NGgxdjJoLTF6IE00OSw1NGgxdjFoLTF6IE01NCw1NGgxdjFoLTF6IE01OCw1NGgydjJoLTJ6IE02Miw1NGgxdjFoLTF6IE02NCw1NGgxdjNoLTF6IE02OSw1NGgxdjJoLTF6IE03NCw1NGgxdjNoLTF6IE03NSw1NGgxdjFoLTF6IE03OCw1NGgxdjFoLTF6IE04MSw1NGgzdjFoLTN6IE0xMSw1NWgxdjFoLTF6IE0xMyw1NWgxdjFoLTF6IE0yOCw1NWgxdjZoLTF6IE0zNCw1NWgxdjFoLTF6IE0zNiw1NWgxdjFoLTF6IE00Miw1NWgxdjFoLTF6IE00NSw1NWgxdjRoLTF6IE00Nyw1NWgxdjJoLTF6IE01MCw1NWgxdjFoLTF6IE02MCw1NWgxdjFoLTF6IE02NSw1NWgxdjFoLTF6IE03MSw1NWgxdjFoLTF6IE03Myw1NWgxdjJoLTF6IE0wLDU2aDF2MWgtMXogTTUsNTZoM3YxaC0zeiBNOSw1NmgxdjFoLTF6IE0xMiw1NmgxdjJoLTF6IE0xNiw1NmgxdjJoLTF6IE0xOSw1NmgxdjFoLTF6IE0yNiw1NmgxdjNoLTF6IE0yOSw1NmgxdjRoLTF6IE0zMCw1NmgydjFoLTJ6IE0zNSw1NmgxdjFoLTF6IE0zOCw1NmgxdjJoLTF6IE00OSw1NmgxdjFoLTF6IE01Myw1NmgxdjZoLTF6IE01NCw1NmgydjFoLTJ6IE01OCw1NmgxdjFoLTF6IE02Nyw1NmgxdjFoLTF6IE03MCw1NmgxdjFoLTF6IE03Miw1NmgxdjJoLTF6IE03NSw1NmgxdjFoLTF6IE03Nyw1NmgzdjNoLTN6IE04MSw1NmgxdjFoLTF6IE03LDU3aDF2MWgtMXogTTEzLDU3aDF2MmgtMXogTTE3LDU3aDF2NGgtMXogTTE4LDU3aDF2MWgtMXogTTIxLDU3aDJ2MWgtMnogTTI0LDU3aDF2NGgtMXogTTMxLDU3aDF2MmgtMXogTTM0LDU3aDF2MmgtMXogTTQwLDU3aDF2MmgtMXogTTQzLDU3aDJ2MWgtMnogTTQ2LDU3aDF2MmgtMXogTTQ4LDU3aDF2MWgtMXogTTU0LDU3aDF2MWgtMXogTTU3LDU3aDF2MWgtMXogTTU5LDU3aDJ2MWgtMnogTTYyLDU3aDJ2MWgtMnogTTY2LDU3aDF2MmgtMXogTTY5LDU3aDF2MWgtMXogTTgyLDU3aDJ2M2gtMnogTTAsNThoMXYyaC0xeiBNNSw1OGgydjFoLTJ6IE04LDU4aDF2MWgtMXogTTE5LDU4aDF2M2gtMXogTTI3LDU4aDF2MWgtMXogTTM1LDU4aDJ2M2gtMnogTTM3LDU4aDF2MWgtMXogTTM5LDU4aDF2MWgtMXogTTQyLDU4aDF2MmgtMXogTTQ3LDU4aDF2MmgtMXogTTQ5LDU4aDJ2MWgtMnogTTU4LDU4aDF2NGgtMXogTTYwLDU4aDF2NGgtMXogTTYxLDU4aDF2MWgtMXogTTYzLDU4aDN2MWgtM3ogTTY3LDU4aDJ2MWgtMnogTTcwLDU4aDJ2MWgtMnogTTgwLDU4aDF2MWgtMXogTTEsNTloMXYyaC0xeiBNNSw1OWgxdjFoLTF6IE03LDU5aDF2MmgtMXogTTksNTloMXYxaC0xeiBNMTQsNTloMXYxaC0xeiBNMTgsNTloMXYzaC0xeiBNMjIsNTloMXYxaC0xeiBNMzAsNTloMXYyaC0xeiBNMzIsNTloMXYxaC0xeiBNNDEsNTloMXYyaC0xeiBNNDgsNTloMXY2aC0xeiBNNTAsNTloMnYxaC0yeiBNNTQsNTloM3YxaC0zeiBNNjMsNTloMXYyaC0xeiBNNjgsNTloMnYyaC0yeiBNNzIsNTloM3YxaC0zeiBNNzgsNTloMXYzaC0xeiBNODEsNTloMXYxaC0xeiBNODQsNTloMXYxaC0xeiBNMyw2MGgydjFoLTJ6IE02LDYwaDF2MWgtMXogTTExLDYwaDF2MWgtMXogTTE1LDYwaDJ2MWgtMnogTTIwLDYwaDJ2MmgtMnogTTIzLDYwaDF2M2gtMXogTTMxLDYwaDF2MWgtMXogTTM0LDYwaDF2MWgtMXogTTM5LDYwaDF2NGgtMXogTTQzLDYwaDR2MWgtNHogTTUwLDYwaDF2MWgtMXogTTU1LDYwaDN2MWgtM3ogTTYxLDYwaDJ2M2gtMnogTTY2LDYwaDF2NGgtMXogTTc0LDYwaDR2MWgtNHogTTc5LDYwaDF2MmgtMXogTTksNjFoMnYxaC0yeiBNMTMsNjFoMnYxaC0yeiBNMjIsNjFoMXYxaC0xeiBNMjcsNjFoMXY0aC0xeiBNMzIsNjFoMnYyaC0yeiBNMzUsNjFoMXYyaC0xeiBNMzcsNjFoMXYyaC0xeiBNNDIsNjFoMXYyaC0xeiBNNDYsNjFoMnYxaC0yeiBNNDksNjFoMXYxaC0xeiBNNTEsNjFoMXYyaC0xeiBNNjQsNjFoMnYzaC0yeiBNNjcsNjFoMXYxaC0xeiBNNzAsNjFoMnYyaC0yeiBNNzUsNjFoMnYxaC0yeiBNODAsNjFoMXYyaC0xeiBNODQsNjFoMXYxaC0xeiBNMSw2MmgxdjdoLTF6IE0yLDYyaDN2MWgtM3ogTTYsNjJoMnYxaC0yeiBNMTEsNjJoMXYxaC0xeiBNMTQsNjJoMnYxaC0yeiBNMTcsNjJoMXYxaC0xeiBNMjEsNjJoMXYxaC0xeiBNMjUsNjJoMXY2aC0xeiBNMjYsNjJoMXYxaC0xeiBNMjksNjJoMXY1aC0xeiBNNDMsNjJoM3YxaC0zeiBNNDcsNjJoMXYyaC0xeiBNNTAsNjJoMXY0aC0xeiBNNTYsNjJoMXYzaC0xeiBNNjksNjJoMXYxaC0xeiBNNzIsNjJoM3YxaC0zeiBNNzcsNjJoMXYxaC0xeiBNODIsNjJoMXYxaC0xeiBNMCw2M2gxdjFoLTF6IE00LDYzaDF2MWgtMXogTTEyLDYzaDF2M2gtMXogTTE0LDYzaDF2MWgtMXogTTE2LDYzaDF2M2gtMXogTTE4LDYzaDF2MWgtMXogTTIwLDYzaDF2MWgtMXogTTMwLDYzaDF2MmgtMXogTTM0LDYzaDF2MmgtMXogTTQxLDYzaDF2MmgtMXogTTQ1LDYzaDF2MmgtMXogTTUzLDYzaDJ2MWgtMnogTTU3LDYzaDF2MWgtMXogTTYzLDYzaDF2MWgtMXogTTY3LDYzaDJ2MWgtMnogTTcwLDYzaDF2MWgtMXogTTc2LDYzaDF2MmgtMXogTTc4LDYzaDJ2MWgtMnogTTg0LDYzaDF2NWgtMXogTTMsNjRoMXYzaC0xeiBNNSw2NGgxdjNoLTF6IE02LDY0aDF2MWgtMXogTTgsNjRoMXYzaC0xeiBNMTEsNjRoMXYzaC0xeiBNMTMsNjRoMXY0aC0xeiBNMTcsNjRoMXYyaC0xeiBNMjMsNjRoMnYxaC0yeiBNMjYsNjRoMXY0aC0xeiBNMzMsNjRoMXYxaC0xeiBNNDAsNjRoMXYyaC0xeiBNNDYsNjRoMXY0aC0xeiBNNDksNjRoMXYzaC0xeiBNNTEsNjRoMXYzaC0xeiBNNTIsNjRoMXYxaC0xeiBNNTUsNjRoMXYzaC0xeiBNNTgsNjRoM3YxaC0zeiBNNjIsNjRoMXYyaC0xeiBNNjUsNjRoMXYxaC0xeiBNNjgsNjRoMXYxaC0xeiBNNzEsNjRoMXYxaC0xeiBNNzMsNjRoMXYzaC0xeiBNNzksNjRoMXYzaC0xeiBNODEsNjRoMnYyaC0yeiBNMCw2NWgxdjRoLTF6IE03LDY1aDF2MWgtMXogTTksNjVoMnYyaC0yeiBNMTQsNjVoMXYxaC0xeiBNMTgsNjVoMXYyaC0xeiBNMjAsNjVoMXYxaC0xeiBNMjIsNjVoMnYxaC0yeiBNMjgsNjVoMXYxaC0xeiBNMzIsNjVoMXYxaC0xeiBNMzcsNjVoMXYxaC0xeiBNMzksNjVoMXY1aC0xeiBNNDIsNjVoMXYxaC0xeiBNNDQsNjVoMXYyaC0xeiBNNDcsNjVoMXYxaC0xeiBNNTMsNjVoMnYxaC0yeiBNNTksNjVoMXYxaC0xeiBNNjEsNjVoMXYxaC0xeiBNNjMsNjVoMXYxaC0xeiBNNzAsNjVoMXY0aC0xeiBNNzQsNjVoMnYyaC0yeiBNNzgsNjVoMXYxaC0xeiBNNCw2NmgxdjJoLTF6IE02LDY2aDF2MWgtMXogTTE1LDY2aDF2MWgtMXogTTE5LDY2aDF2MmgtMXogTTI0LDY2aDF2MmgtMXogTTMwLDY2aDF2MWgtMXogTTM0LDY2aDN2MWgtM3ogTTQzLDY2aDF2NWgtMXogTTQ1LDY2aDF2MmgtMXogTTU0LDY2aDF2MmgtMXogTTU2LDY2aDN2MWgtM3ogTTY0LDY2aDF2M2gtMXogTTY5LDY2aDF2MmgtMXogTTcyLDY2aDF2MmgtMXogTTc3LDY2aDF2OGgtMXogTTgyLDY2aDJ2MWgtMnogTTIsNjdoMXYzaC0xeiBNNyw2N2gxdjJoLTF6IE0xNiw2N2gxdjFoLTF6IE0yMCw2N2gydjFoLTJ6IE0yMyw2N2gxdjhoLTF6IE0yNyw2N2gxdjNoLTF6IE0zMiw2N2gydjFoLTJ6IE0zNiw2N2gxdjFoLTF6IE00MCw2N2gxdjNoLTF6IE00OCw2N2gxdjFoLTF6IE01Niw2N2gxdjFoLTF6IE02MSw2N2gxdjJoLTF6IE02Myw2N2gxdjFoLTF6IE02Nyw2N2gxdjVoLTF6IE02OCw2N2gxdjFoLTF6IE03MSw2N2gxdjFoLTF6IE03NSw2N2gxdjFoLTF6IE04MCw2N2gydjJoLTJ6IE01LDY4aDJ2MWgtMnogTTgsNjhoMnYxaC0yeiBNMTgsNjhoMXYxaC0xeiBNMjgsNjhoMXYzaC0xeiBNMjksNjhoMXYxaC0xeiBNMzUsNjhoMXYxaC0xeiBNNDEsNjhoMXY0aC0xeiBNNDcsNjhoMXYzaC0xeiBNNDksNjhoMXY0aC0xeiBNNTAsNjhoMnYxaC0yeiBNNTcsNjhoMXY1aC0xeiBNNTgsNjhoMXYxaC0xeiBNNzMsNjhoMnYxaC0yeiBNNzYsNjhoMXY2aC0xeiBNNzksNjhoMXYzaC0xeiBNODMsNjhoMXYyaC0xeiBNMyw2OWgxdjVoLTF6IE00LDY5aDF2MmgtMXogTTksNjloMnY1aC0yeiBNMTQsNjloM3YxaC0zeiBNMTksNjloNHYxaC00eiBNMjQsNjloMXYxaC0xeiBNMzAsNjloMXY0aC0xeiBNMzEsNjloMXYxaC0xeiBNMzQsNjloMXYyaC0xeiBNMzYsNjloM3YxaC0zeiBNNDQsNjloMXYyaC0xeiBNNDgsNjloMXYxaC0xeiBNNTIsNjloMXYyaC0xeiBNNTksNjloMnYyaC0yeiBNNjMsNjloMXYxaC0xeiBNNjksNjloMXYxaC0xeiBNNzEsNjloMnYxaC0yeiBNODEsNjloMnYxaC0yeiBNODQsNjloMXYxaC0xeiBNMCw3MGgxdjFoLTF6IE02LDcwaDF2MWgtMXogTTExLDcwaDJ2MWgtMnogTTE5LDcwaDF2MWgtMXogTTIxLDcwaDJ2MWgtMnogTTI2LDcwaDF2M2gtMXogTTI5LDcwaDF2M2gtMXogTTM1LDcwaDF2MWgtMXogTTM3LDcwaDJ2MWgtMnogTTQyLDcwaDF2M2gtMXogTTQ1LDcwaDF2MmgtMXogTTUwLDcwaDJ2MWgtMnogTTUzLDcwaDJ2MWgtMnogTTYxLDcwaDF2MWgtMXogTTY0LDcwaDN2MWgtM3ogTTcwLDcwaDF2MWgtMXogTTgyLDcwaDF2M2gtMXogTTEsNzFoMXYxaC0xeiBNNyw3MWgxdjRoLTF6IE04LDcxaDF2MWgtMXogTTEyLDcxaDN2MmgtM3ogTTE1LDcxaDJ2MWgtMnogTTIwLDcxaDF2MmgtMXogTTIyLDcxaDF2MWgtMXogTTI3LDcxaDF2MWgtMXogTTMxLDcxaDF2M2gtMXogTTMzLDcxaDF2MWgtMXogTTUwLDcxaDF2MWgtMXogTTU0LDcxaDJ2MWgtMnogTTU4LDcxaDF2MWgtMXogTTYwLDcxaDF2N2gtMXogTTcyLDcxaDF2MWgtMXogTTc0LDcxaDF2MWgtMXogTTgwLDcxaDF2MTNoLTF6IE0wLDcyaDF2MWgtMXogTTYsNzJoMXYxaC0xeiBNMTEsNzJoMXYzaC0xeiBNMTcsNzJoMXYxaC0xeiBNMjQsNzJoMXYxaC0xeiBNMzQsNzJoMXYyaC0xeiBNMzcsNzJoMXYxaC0xeiBNMzksNzJoMnYxaC0yeiBNNDYsNzJoMnYxaC0yeiBNNTMsNzJoMXYxaC0xeiBNNTUsNzJoMnYxaC0yeiBNNjEsNzJoM3YxaC0zeiBNNjUsNzJoMnY0aC0yeiBNNjgsNzJoMXYyaC0xeiBNNzAsNzJoMXYyaC0xeiBNNzgsNzJoMnYxaC0yeiBNODMsNzJoMnYyaC0yeiBNMiw3M2gxdjJoLTF6IE00LDczaDJ2MWgtMnogTTEyLDczaDF2NGgtMXogTTE5LDczaDF2MmgtMXogTTIyLDczaDF2M2gtMXogTTI3LDczaDF2MmgtMXogTTMyLDczaDF2OGgtMXogTTM1LDczaDF2MWgtMXogTTM4LDczaDJ2MWgtMnogTTQ1LDczaDF2NGgtMXogTTQ2LDczaDF2MWgtMXogTTQ5LDczaDF2M2gtMXogTTUwLDczaDF2MWgtMXogTTU2LDczaDF2MmgtMXogTTU4LDczaDF2MWgtMXogTTYyLDczaDJ2MmgtMnogTTcyLDczaDF2M2gtMXogTTc0LDczaDF2NGgtMXogTTc5LDczaDF2MWgtMXogTTgxLDczaDF2MmgtMXogTTAsNzRoMnYxaC0yeiBNNCw3NGgxdjFoLTF6IE02LDc0aDF2MWgtMXogTTgsNzRoMnYxaC0yeiBNMTQsNzRoNHYxaC00eiBNMjEsNzRoMXYxaC0xeiBNMjUsNzRoMXY2aC0xeiBNMjYsNzRoMXYxaC0xeiBNMjgsNzRoMXY3aC0xeiBNMjksNzRoMXYxaC0xeiBNMzMsNzRoMXYxaC0xeiBNMzYsNzRoMnYxaC0yeiBNNDEsNzRoNHYxaC00eiBNNDcsNzRoMXYxaC0xeiBNNTEsNzRoMXY0aC0xeiBNNTcsNzRoMXYyaC0xeiBNNTksNzRoMXY0aC0xeiBNNjcsNzRoMXYyaC0xeiBNNzMsNzRoMXYyaC0xeiBNNzgsNzRoMXYxaC0xeiBNODMsNzRoMXYyaC0xeiBNMSw3NWgxdjFoLTF6IE0zLDc1aDF2MWgtMXogTTUsNzVoMXYxaC0xeiBNOCw3NWgxdjFoLTF6IE0xNCw3NWgxdjNoLTF6IE0xNyw3NWgydjFoLTJ6IE0yMCw3NWgxdjJoLTF6IE0yNCw3NWgxdjFoLTF6IE0zMCw3NWgydjJoLTJ6IE0zNSw3NWgxdjFoLTF6IE0zOCw3NWgydjNoLTJ6IE00Myw3NWgxdjFoLTF6IE02Myw3NWgydjFoLTJ6IE02OCw3NWgydjFoLTJ6IE03MSw3NWgxdjNoLTF6IE03OSw3NWgxdjJoLTF6IE0wLDc2aDF2MWgtMXogTTYsNzZoMXYxaC0xeiBNOSw3NmgydjFoLTJ6IE0xMyw3NmgxdjRoLTF6IE0xNSw3NmgydjFoLTJ6IE0xOCw3NmgxdjFoLTF6IE0yNyw3NmgxdjFoLTF6IE0yOSw3NmgxdjFoLTF6IE0zMyw3NmgydjFoLTJ6IE0zNyw3NmgxdjFoLTF6IE00MCw3NmgzdjFoLTN6IE00NCw3NmgxdjJoLTF6IE00Niw3NmgxdjFoLTF6IE00OCw3NmgxdjFoLTF6IE01MCw3NmgxdjJoLTF6IE01Miw3Nmg1djFoLTV6IE01OCw3NmgxdjFoLTF6IE02NCw3NmgxdjJoLTF6IE02Niw3NmgxdjFoLTF6IE03MCw3NmgxdjloLTF6IE03Niw3NmgxdjZoLTF6IE03Nyw3NmgydjFoLTJ6IE04Miw3NmgxdjNoLTF6IE04NCw3NmgxdjRoLTF6IE04LDc3aDF2MmgtMXogTTEwLDc3aDF2NGgtMXogTTM0LDc3aDJ2MmgtMnogTTQxLDc3aDF2MmgtMXogTTQ3LDc3aDF2NWgtMXogTTUyLDc3aDF2NGgtMXogTTU2LDc3aDF2NGgtMXogTTYxLDc3aDF2MmgtMXogTTYzLDc3aDF2M2gtMXogTTY1LDc3aDF2MWgtMXogTTY4LDc3aDF2MWgtMXogTTcyLDc3aDF2NWgtMXogTTczLDc3aDF2MWgtMXogTTAsNzhoN3YxaC03eiBNMTEsNzhoMXYxaC0xeiBNMTUsNzhoMXYxaC0xeiBNMTcsNzhoMXYyaC0xeiBNMTksNzhoMnYxaC0yeiBNMjIsNzhoMXYyaC0xeiBNMjQsNzhoMXYyaC0xeiBNMjYsNzhoMXYxaC0xeiBNMzAsNzhoMXYxaC0xeiBNMzMsNzhoMXY2aC0xeiBNMzcsNzhoMXYxaC0xeiBNMzksNzhoMnYxaC0yeiBNNDIsNzhoMXYzaC0xeiBNNDMsNzhoMXYxaC0xeiBNNDgsNzhoMnYxaC0yeiBNNTQsNzhoMXYxaC0xeiBNNjIsNzhoMXYyaC0xeiBNNjksNzhoMXYxaC0xeiBNNzQsNzhoMnYxaC0yeiBNNzgsNzhoMXYxaC0xeiBNODMsNzhoMXYxaC0xeiBNMCw3OWgxdjZoLTF6IE02LDc5aDF2NmgtMXogTTksNzloMXYyaC0xeiBNMTgsNzloMXYxaC0xeiBNMjAsNzloMnYxaC0yeiBNMjMsNzloMXY0aC0xeiBNMjcsNzloMXYyaC0xeiBNMzQsNzloMXYxaC0xeiBNMzgsNzloMnYxaC0yeiBNNDQsNzloMXYzaC0xeiBNNDYsNzloMXY1aC0xeiBNNDksNzloMnYyaC0yeiBNNTgsNzloMXYyaC0xeiBNNjQsNzloMnYxaC0yeiBNNzMsNzloMXYyaC0xeiBNNzUsNzloMXYxaC0xeiBNODEsNzloMXY0aC0xeiBNMiw4MGgzdjNoLTN6IE0xMSw4MGgxdjVoLTF6IE0xNSw4MGgxdjFoLTF6IE0xOSw4MGgxdjRoLTF6IE0yMSw4MGgxdjJoLTF6IE0yOSw4MGgzdjFoLTN6IE0zNSw4MGgxdjFoLTF6IE0zNyw4MGgydjFoLTJ6IE00MCw4MGgxdjJoLTF6IE00OCw4MGgxdjFoLTF6IE01MSw4MGgxdjJoLTF6IE01Myw4MGgzdjFoLTN6IE02MCw4MGgydjFoLTJ6IE02NCw4MGgxdjFoLTF6IE02Nyw4MGgydjJoLTJ6IE03NCw4MGgxdjRoLTF6IE03Nyw4MGgydjJoLTJ6IE03OSw4MGgxdjFoLTF6IE04Miw4MGgxdjFoLTF6IE04LDgxaDF2MmgtMXogTTEzLDgxaDJ2MWgtMnogTTE4LDgxaDF2NGgtMXogTTIyLDgxaDF2MmgtMXogTTMwLDgxaDJ2MWgtMnogTTM0LDgxaDF2MmgtMXogTTM2LDgxaDF2MWgtMXogTTM5LDgxaDF2NGgtMXogTTQxLDgxaDF2MWgtMXogTTQzLDgxaDF2NGgtMXogTTQ5LDgxaDF2MWgtMXogTTUzLDgxaDF2MWgtMXogTTU3LDgxaDF2MWgtMXogTTYyLDgxaDJ2MWgtMnogTTY5LDgxaDF2MmgtMXogTTcxLDgxaDF2M2gtMXogTTg0LDgxaDF2MWgtMXogTTEwLDgyaDF2MWgtMXogTTE0LDgyaDR2MWgtNHogTTIwLDgyaDF2MWgtMXogTTI0LDgyaDF2MWgtMXogTTI2LDgyaDF2M2gtMXogTTI4LDgyaDF2MWgtMXogTTM1LDgyaDF2MmgtMXogTTQ4LDgyaDF2MmgtMXogTTUwLDgyaDF2MmgtMXogTTU1LDgyaDF2MWgtMXogTTU4LDgyaDR2MWgtNHogTTY1LDgyaDF2M2gtMXogTTczLDgyaDF2MmgtMXogTTc5LDgyaDF2MWgtMXogTTgzLDgyaDF2MWgtMXogTTksODNoMXYyaC0xeiBNMTMsODNoMXYxaC0xeiBNMTYsODNoMnYxaC0yeiBNMjEsODNoMXYyaC0xeiBNMjUsODNoMXYyaC0xeiBNMzAsODNoM3YxaC0zeiBNMzcsODNoMXYxaC0xeiBNNDIsODNoMXYyaC0xeiBNNDUsODNoMXYyaC0xeiBNNDcsODNoMXYxaC0xeiBNNDksODNoMXYyaC0xeiBNNTEsODNoMXYyaC0xeiBNNTQsODNoMXYyaC0xeiBNNTYsODNoMXYyaC0xeiBNNTksODNoMXYxaC0xeiBNNjEsODNoM3YxaC0zeiBNNjYsODNoMXYxaC0xeiBNNjgsODNoMXYxaC0xeiBNNzIsODNoMXYxaC0xeiBNNzUsODNoMXYyaC0xeiBNNzcsODNoMnYxaC0yeiBNODIsODNoMXYxaC0xeiBNMSw4NGg1djFoLTV6IE0xMiw4NGgxdjFoLTF6IE0xNSw4NGgydjFoLTJ6IE0yMCw4NGgxdjFoLTF6IE0yMyw4NGgxdjFoLTF6IE0yOSw4NGgxdjFoLTF6IE0zMSw4NGgydjFoLTJ6IE0zNCw4NGgxdjFoLTF6IE0zOCw4NGgxdjFoLTF6IE00MCw4NGgydjFoLTJ6IE00NCw4NGgxdjFoLTF6IE02MCw4NGgxdjFoLTF6IE02Myw4NGgydjFoLTJ6IE02OSw4NGgxdjFoLTF6IE04Myw4NGgydjFoLTJ6IiBmaWxsPSIjMDAwMDAwIi8+Cjwvc3ZnPgo=",
                //QrImage = "data:image/svg+xml;charset=utf-8;base64, PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4KPCFET0NUWVBFIHN2ZyBQVUJMSUMgIi0vL1czQy8vRFREIFNWRyAxLjEvL0VOIiAiaHR0cDovL3d3dy53My5vcmcvR3JhcGhpY3MvU1ZHLzEuMS9EVEQvc3ZnMTEuZHRkIj4KPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZlcnNpb249IjEuMSIgdmlld0JveD0iMCAwIDg1IDg1IiBzdHJva2U9Im5vbmUiPgoJPHJlY3Qgd2lkdGg9IjEwMCUiIGhlaWdodD0iMTAwJSIgZmlsbD0iI2ZmZmZmZiIvPgoJPHBhdGggZD0iTTAsMGg3djFoLTd6IE05LDBoMnYxaC0yeiBNMTIsMGgxdjJoLTF6IE0xOSwwaDF2MWgtMXogTTI1LDBoNHYxaC00eiBNMzIsMGg5djFoLTl6IE00MiwwaDJ2MWgtMnogTTQ2LDBoMXYxaC0xeiBNNDksMGgydjFoLTJ6IE01MywwaDF2MWgtMXogTTU1LDBoM3YxaC0zeiBNNjIsMGgxdjFoLTF6IE02NCwwaDF2MWgtMXogTTY2LDBoMXYxaC0xeiBNNjgsMGgydjFoLTJ6IE03MiwwaDF2MWgtMXogTTc0LDBoMXYzaC0xeiBNNzYsMGgxdjFoLTF6IE03OCwwaDd2MWgtN3ogTTAsMWgxdjZoLTF6IE02LDFoMXY2aC0xeiBNMTAsMWgxdjFoLTF6IE0xNSwxaDR2MWgtNHogTTIwLDFoMnYyaC0yeiBNMjMsMWgxdjFoLTF6IE0yNSwxaDF2MmgtMXogTTI5LDFoM3YxaC0zeiBNMzUsMWgxdjRoLTF6IE0zOSwxaDJ2MmgtMnogTTQxLDFoMXYxaC0xeiBNNTQsMWgxdjJoLTF6IE01NiwxaDF2MWgtMXogTTU4LDFoM3YyaC0zeiBNNjEsMWgxdjFoLTF6IE02MywxaDF2M2gtMXogTTY1LDFoMXYzaC0xeiBNNjcsMWgxdjFoLTF6IE02OSwxaDJ2MWgtMnogTTc1LDFoMXYxaC0xeiBNNzgsMWgxdjZoLTF6IE04NCwxaDF2NmgtMXogTTIsMmgzdjNoLTN6IE05LDJoMXYzaC0xeiBNMTMsMmgydjFoLTJ6IE0xNiwyaDN2MWgtM3ogTTIyLDJoMXYyaC0xeiBNMjQsMmgxdjFoLTF6IE0yNywyaDF2MmgtMXogTTMwLDJoMXYzaC0xeiBNMzQsMmgxdjJoLTF6IE0zNiwyaDF2NWgtMXogTTQ3LDJoM3YzaC0zeiBNNTAsMmgydjFoLTJ6IE02MiwyaDF2MWgtMXogTTY0LDJoMXY2aC0xeiBNNzAsMmgxdjFoLTF6IE03MiwyaDF2MWgtMXogTTgwLDJoM3YzaC0zeiBNMTAsM2gydjJoLTJ6IE0xMiwzaDF2MWgtMXogTTE3LDNoM3YxaC0zeiBNMjEsM2gxdjJoLTF6IE0yNiwzaDF2NWgtMXogTTI4LDNoMXY3aC0xeiBNMjksM2gxdjJoLTF6IE0zMiwzaDF2MTFoLTF6IE0zMywzaDF2MWgtMXogTTM5LDNoMXYxaC0xeiBNNDIsM2gxdjJoLTF6IE00NCwzaDF2MmgtMXogTTUyLDNoMXY3aC0xeiBNNTUsM2gxdjJoLTF6IE01NywzaDF2MWgtMXogTTYwLDNoMnYxaC0yeiBNNzUsM2gxdjFoLTF6IE04LDRoMXYxaC0xeiBNMTMsNGgxdjFoLTF6IE0xNyw0aDF2MWgtMXogTTIwLDRoMXYxaC0xeiBNMjQsNGgxdjFoLTF6IE0zMSw0aDF2MWgtMXogTTQxLDRoMXYxaC0xeiBNNDUsNGgydjFoLTJ6IE01MCw0aDF2MWgtMXogTTUzLDRoMnYxaC0yeiBNNTYsNGgxdjVoLTF6IE01OSw0aDF2MWgtMXogTTYyLDRoMXYzaC0xeiBNNjcsNGgydjJoLTJ6IE03Myw0aDJ2MWgtMnogTTExLDVoMXYxaC0xeiBNMTQsNWgzdjFoLTN6IE0xOCw1aDF2M2gtMXogTTE5LDVoMXYxaC0xeiBNMjMsNWgxdjFoLTF6IE0zMyw1aDF2MWgtMXogTTM3LDVoMXYxaC0xeiBNMzksNWgydjFoLTJ6IE00Niw1aDJ2MWgtMnogTTU3LDVoMXYxaC0xeiBNNjAsNWgxdjNoLTF6IE02Niw1aDF2MmgtMXogTTY5LDVoMXYxaC0xeiBNNzUsNWgxdjFoLTF6IE0xLDZoNXYxaC01eiBNOCw2aDF2NWgtMXogTTEwLDZoMXYxaC0xeiBNMTIsNmgxdjJoLTF6IE0xNCw2aDF2MWgtMXogTTE2LDZoMXYxaC0xeiBNMjAsNmgxdjJoLTF6IE0yMiw2aDF2MWgtMXogTTI0LDZoMXY0aC0xeiBNMzAsNmgxdjFoLTF6IE0zNCw2aDF2M2gtMXogTTM4LDZoMXYzaC0xeiBNNDAsNmgxdjJoLTF6IE00Miw2aDF2MWgtMXogTTQ0LDZoMXY1aC0xeiBNNDYsNmgxdjFoLTF6IE00OCw2aDF2MmgtMXogTTUwLDZoMXYxaC0xeiBNNTQsNmgxdjFoLTF6IE01OCw2aDF2M2gtMXogTTY4LDZoMXYxaC0xeiBNNzAsNmgxdjFoLTF6IE03Miw2aDF2M2gtMXogTTc0LDZoMXYyaC0xeiBNNzYsNmgxdjJoLTF6IE03OSw2aDV2MWgtNXogTTksN2gxdjFoLTF6IE0xMSw3aDF2MWgtMXogTTE1LDdoMXYxaC0xeiBNMjEsN2gxdjJoLTF6IE0yMyw3aDF2MWgtMXogTTM3LDdoMXYyaC0xeiBNMzksN2gxdjFoLTF6IE00MSw3aDF2NGgtMXogTTQ1LDdoMXYxaC0xeiBNNDcsN2gxdjFoLTF6IE00OSw3aDF2MWgtMXogTTU3LDdoMXYxaC0xeiBNNjMsN2gxdjFoLTF6IE03NSw3aDF2NGgtMXogTTIsOGgydjFoLTJ6IE02LDhoMnYxaC0yeiBNMTcsOGgxdjJoLTF6IE0yNyw4aDF2NWgtMXogTTI5LDhoM3YxaC0zeiBNMzYsOGgxdjJoLTF6IE00Miw4aDJ2MWgtMnogTTQ2LDhoMXYyaC0xeiBNNTAsOGgydjFoLTJ6IE01Myw4aDN2MWgtM3ogTTYyLDhoMXYyaC0xeiBNNjUsOGgzdjFoLTN6IE02OSw4aDF2NWgtMXogTTc3LDhoMnYxaC0yeiBNODAsOGgxdjFoLTF6IE0xLDloMXYxaC0xeiBNNCw5aDF2MWgtMXogTTEwLDloMnYyaC0yeiBNMTMsOWg0djJoLTR6IE0xOSw5aDJ2MWgtMnogTTIzLDloMXYxaC0xeiBNMzEsOWgxdjFoLTF6IE0zOSw5aDF2MWgtMXogTTQzLDloMXYxaC0xeiBNNDUsOWgxdjFoLTF6IE00Nyw5aDR2MWgtNHogTTU0LDloMXYzaC0xeiBNNTcsOWgxdjFoLTF6IE03MCw5aDF2M2gtMXogTTc0LDloMXYxaC0xeiBNODEsOWgxdjJoLTF6IE04NCw5aDF2M2gtMXogTTIsMTBoMnYxaC0yeiBNNSwxMGgydjFoLTJ6IE05LDEwaDF2MWgtMXogTTEyLDEwaDF2MmgtMXogTTE5LDEwaDF2MWgtMXogTTIxLDEwaDF2MWgtMXogTTI2LDEwaDF2MWgtMXogTTMzLDEwaDF2MWgtMXogTTM1LDEwaDF2MmgtMXogTTM3LDEwaDJ2MmgtMnogTTQwLDEwaDF2MWgtMXogTTQyLDEwaDF2NGgtMXogTTQ3LDEwaDF2M2gtMXogTTQ5LDEwaDF2MWgtMXogTTUxLDEwaDF2MmgtMXogTTU1LDEwaDJ2MWgtMnogTTU4LDEwaDR2MWgtNHogTTY4LDEwaDF2MmgtMXogTTczLDEwaDF2M2gtMXogTTc2LDEwaDF2MWgtMXogTTMsMTFoMnYxaC0yeiBNNywxMWgxdjJoLTF6IE0xMCwxMWgxdjJoLTF6IE0xNCwxMWgydjFoLTJ6IE0yMCwxMWgxdjJoLTF6IE0yMywxMWgxdjJoLTF6IE0yOCwxMWg0djJoLTR6IE0zNCwxMWgxdjJoLTF6IE0zNiwxMWgxdjJoLTF6IE0zOSwxMWgxdjRoLTF6IE00NiwxMWgxdjZoLTF6IE00OCwxMWgxdjJoLTF6IE01MCwxMWgxdjFoLTF6IE01MiwxMWgxdjNoLTF6IE01NiwxMWgxdjJoLTF6IE01OSwxMWgxdjFoLTF6IE02MSwxMWgydjFoLTJ6IE02NSwxMWgxdjFoLTF6IE02NywxMWgxdjFoLTF6IE03MiwxMWgxdjFoLTF6IE04MiwxMWgxdjFoLTF6IE0wLDEyaDF2MmgtMXogTTIsMTJoMXYyaC0xeiBNNCwxMmgxdjNoLTF6IE02LDEyaDF2MWgtMXogTTksMTJoMXYyaC0xeiBNMTUsMTJoMXYyaC0xeiBNMTcsMTJoMnYxaC0yeiBNMjIsMTJoMXYyaC0xeiBNMjQsMTJoMXYxaC0xeiBNMjYsMTJoMXYyaC0xeiBNMzMsMTJoMXYzaC0xeiBNMzcsMTJoMXYyaC0xeiBNNDAsMTJoMnYyaC0yeiBNNDksMTJoMXYzaC0xeiBNNTMsMTJoMXYyaC0xeiBNNTgsMTJoMXYxaC0xeiBNNjEsMTJoMXYxaC0xeiBNNjYsMTJoMXYzaC0xeiBNNzQsMTJoMXYyaC0xeiBNNzYsMTJoMXYxaC0xeiBNNzgsMTJoMXYyaC0xeiBNODEsMTJoMXYxaC0xeiBNODMsMTJoMXYzaC0xeiBNMywxM2gxdjJoLTF6IE04LDEzaDF2M2gtMXogTTEzLDEzaDJ2MWgtMnogTTE2LDEzaDF2MWgtMXogTTI1LDEzaDF2MmgtMXogTTMwLDEzaDF2MmgtMXogTTM1LDEzaDF2MmgtMXogTTQzLDEzaDF2MmgtMXogTTUxLDEzaDF2MmgtMXogTTYwLDEzaDF2MmgtMXogTTYyLDEzaDR2MWgtNHogTTY3LDEzaDJ2MWgtMnogTTcwLDEzaDN2MWgtM3ogTTc3LDEzaDF2NGgtMXogTTc5LDEzaDJ2MmgtMnogTTgyLDEzaDF2MWgtMXogTTg0LDEzaDF2MWgtMXogTTEsMTRoMXYxaC0xeiBNNSwxNGgydjFoLTJ6IE0xMCwxNGgzdjFoLTN6IE0xOCwxNGgxdjJoLTF6IE0yMSwxNGgxdjFoLTF6IE0yNCwxNGgxdjNoLTF6IE0yNywxNGgxdjFoLTF6IE0zNCwxNGgxdjJoLTF6IE0zNiwxNGgxdjNoLTF6IE0zOCwxNGgxdjJoLTF6IE00MCwxNGgxdjFoLTF6IE00OCwxNGgxdjJoLTF6IE01MCwxNGgxdjFoLTF6IE02MSwxNGgzdjFoLTN6IE02NSwxNGgxdjFoLTF6IE02NywxNGgxdjFoLTF6IE03MiwxNGgxdjFoLTF6IE03NiwxNGgxdjFoLTF6IE04MSwxNGgxdjFoLTF6IE0wLDE1aDF2MWgtMXogTTIsMTVoMXYxaC0xeiBNNywxNWgxdjNoLTF6IE0xMiwxNWgxdjFoLTF6IE0xNCwxNWgxdjFoLTF6IE0xNiwxNWgxdjFoLTF6IE0xOSwxNWgxdjFoLTF6IE0yMywxNWgxdjFoLTF6IE0yNiwxNWgxdjFoLTF6IE0zMSwxNWgydjFoLTJ6IE0zNywxNWgxdjFoLTF6IE00MiwxNWgxdjFoLTF6IE00NSwxNWgxdjFoLTF6IE01NCwxNWgydjFoLTJ6IE01NywxNWgzdjFoLTN6IE02MSwxNWgxdjFoLTF6IE02MywxNWgxdjFoLTF6IE02OCwxNWgxdjFoLTF6IE03MCwxNWgxdjFoLTF6IE03NCwxNWgydjFoLTJ6IE03OCwxNWgydjJoLTJ6IE0xLDE2aDF2M2gtMXogTTMsMTZoMXY1aC0xeiBNNCwxNmgzdjFoLTN6IE05LDE2aDF2M2gtMXogTTEwLDE2aDF2MWgtMXogTTEzLDE2aDF2MmgtMXogTTE1LDE2aDF2MWgtMXogTTIyLDE2aDF2MmgtMXogTTI3LDE2aDJ2MWgtMnogTTM1LDE2aDF2MmgtMXogTTQwLDE2aDF2NmgtMXogTTQxLDE2aDF2MWgtMXogTTQzLDE2aDF2MWgtMXogTTQ3LDE2aDF2MWgtMXogTTUxLDE2aDF2NGgtMXogTTU1LDE2aDN2MWgtM3ogTTU5LDE2aDJ2MWgtMnogTTYyLDE2aDF2MmgtMXogTTY0LDE2aDF2MmgtMXogTTcxLDE2aDN2MmgtM3ogTTc1LDE2aDJ2MWgtMnogTTgyLDE2aDF2MWgtMXogTTAsMTdoMXYxaC0xeiBNMiwxN2gxdjJoLTF6IE01LDE3aDF2NWgtMXogTTgsMTdoMXYzaC0xeiBNMTIsMTdoMXYyaC0xeiBNMTYsMTdoMXYxaC0xeiBNMTgsMTdoMXYyaC0xeiBNMjYsMTdoMXYxaC0xeiBNMjgsMTdoMnYxaC0yeiBNMzEsMTdoNHYxaC00eiBNMzcsMTdoMXYxaC0xeiBNNDIsMTdoMXYxaC0xeiBNNTMsMTdoMnYyaC0yeiBNNTYsMTdoMXYxaC0xeiBNNjAsMTdoMXY1aC0xeiBNNjEsMTdoMXYyaC0xeiBNNjYsMTdoMXYxaC0xeiBNNjgsMTdoMXYyaC0xeiBNNzAsMTdoMXYyaC0xeiBNODEsMTdoMXYxaC0xeiBNODQsMTdoMXY1aC0xeiBNNiwxOGgxdjFoLTF6IE0xMCwxOGgydjFoLTJ6IE0xNSwxOGgxdjFoLTF6IE0xNywxOGgxdjFoLTF6IE0yMCwxOGgxdjJoLTF6IE0yMywxOGgxdjNoLTF6IE0yNSwxOGgxdjFoLTF6IE0yOSwxOGgydjFoLTJ6IE0zMiwxOGgxdjNoLTF6IE0zNiwxOGgxdjJoLTF6IE0zOCwxOGgydjFoLTJ6IE00NCwxOGgxdjJoLTF6IE00NywxOGgxdjNoLTF6IE00OSwxOGgxdjFoLTF6IE01NSwxOGgxdjJoLTF6IE01OCwxOGgxdjFoLTF6IE02MywxOGgxdjJoLTF6IE02NSwxOGgxdjFoLTF6IE03MiwxOGgxdjZoLTF6IE03NCwxOGgxdjJoLTF6IE03NywxOGgydjNoLTJ6IE03OSwxOGgydjFoLTJ6IE04MywxOGgxdjJoLTF6IE0wLDE5aDF2MWgtMXogTTQsMTloMXYzaC0xeiBNMTYsMTloMXY0aC0xeiBNMTksMTloMXYxaC0xeiBNMjIsMTloMXYxaC0xeiBNMzEsMTloMXYxaC0xeiBNMzQsMTloMXY0aC0xeiBNMzUsMTloMXYxaC0xeiBNMzcsMTloMXY0aC0xeiBNMzksMTloMXYxaC0xeiBNNDEsMTloMXYxaC0xeiBNNDUsMTloMXYxaC0xeiBNNTIsMTloMXYxaC0xeiBNNTcsMTloMXYxaC0xeiBNNjQsMTloMXY0aC0xeiBNNjYsMTloMnYxaC0yeiBNNjksMTloMXYyaC0xeiBNNzEsMTloMXYxaC0xeiBNNzUsMTloMXYxaC0xeiBNODAsMTloMXYxaC0xeiBNMiwyMGgxdjFoLTF6IE02LDIwaDF2MWgtMXogTTksMjBoMXYxaC0xeiBNMTIsMjBoMnYxaC0yeiBNMTUsMjBoMXYxaC0xeiBNMTcsMjBoMXYxaC0xeiBNMjEsMjBoMXYzaC0xeiBNMjQsMjBoMnYxaC0yeiBNMjcsMjBoMXYzaC0xeiBNMjgsMjBoMXYxaC0xeiBNMzMsMjBoMXYzaC0xeiBNMzgsMjBoMXY0aC0xeiBNNDMsMjBoMXYxaC0xeiBNNDgsMjBoMXYxaC0xeiBNNTMsMjBoMnY0aC0yeiBNNTYsMjBoMXYxaC0xeiBNNTgsMjBoMXY0aC0xeiBNNjIsMjBoMXYyaC0xeiBNNjcsMjBoMnYxaC0yeiBNNzMsMjBoMXYxaC0xeiBNNzksMjBoMXYxaC0xeiBNMSwyMWgxdjFoLTF6IE04LDIxaDF2MmgtMXogTTEzLDIxaDF2MmgtMXogTTI0LDIxaDF2MWgtMXogTTMxLDIxaDF2MWgtMXogTTM1LDIxaDJ2MWgtMnogTTM5LDIxaDF2MmgtMXogTTQxLDIxaDJ2MWgtMnogTTQ1LDIxaDF2MWgtMXogTTQ5LDIxaDF2NmgtMXogTTUxLDIxaDF2MTJoLTF6IE01NSwyMWgxdjJoLTF6IE01NywyMWgxdjFoLTF6IE01OSwyMWgxdjNoLTF6IE02MywyMWgxdjFoLTF6IE03NCwyMWgydjFoLTJ6IE04MiwyMWgxdjVoLTF6IE0wLDIyaDF2MmgtMXogTTIsMjJoMXYzaC0xeiBNNiwyMmgydjFoLTJ6IE0xMiwyMmgxdjJoLTF6IE0xNSwyMmgxdjFoLTF6IE0xNywyMmgxdjNoLTF6IE0yMCwyMmgxdjFoLTF6IE0yMywyMmgxdjNoLTF6IE0yNSwyMmgxdjFoLTF6IE0zNiwyMmgxdjFoLTF6IE00NCwyMmgxdjFoLTF6IE00NywyMmgydjFoLTJ6IE01NiwyMmgxdjFoLTF6IE02MSwyMmgxdjFoLTF6IE02NiwyMmgxdjRoLTF6IE03MCwyMmgxdjJoLTF6IE03NiwyMmgxdjFoLTF6IE03OCwyMmgxdjFoLTF6IE04MCwyMmgydjFoLTJ6IE0xLDIzaDF2NGgtMXogTTMsMjNoMnYxaC0yeiBNMTAsMjNoMXYyaC0xeiBNMTksMjNoMXYyaC0xeiBNMjQsMjNoMXYxaC0xeiBNMjgsMjNoM3YxaC0zeiBNNDAsMjNoMnYxaC0yeiBNNDMsMjNoMXY0aC0xeiBNNDYsMjNoMXYyaC0xeiBNNTcsMjNoMXYxaC0xeiBNNjAsMjNoMXYxaC0xeiBNNjUsMjNoMXYxaC0xeiBNNjcsMjNoMnYxaC0yeiBNNzEsMjNoMXYxaC0xeiBNNzMsMjNoMnYxaC0yeiBNODEsMjNoMXYyaC0xeiBNMywyNGgxdjFoLTF6IE02LDI0aDN2MWgtM3ogTTEzLDI0aDN2MmgtM3ogTTE2LDI0aDF2MWgtMXogTTE4LDI0aDF2MWgtMXogTTIxLDI0aDJ2MWgtMnogTTI1LDI0aDF2MWgtMXogTTMxLDI0aDJ2MWgtMnogTTM0LDI0aDF2MWgtMXogTTM2LDI0aDJ2MWgtMnogTTM5LDI0aDF2MWgtMXogTTQyLDI0aDF2M2gtMXogTTQ0LDI0aDF2NGgtMXogTTQ1LDI0aDF2MWgtMXogTTUyLDI0aDJ2MmgtMnogTTU2LDI0aDF2MWgtMXogTTYxLDI0aDF2MWgtMXogTTYzLDI0aDF2MmgtMXogTTY4LDI0aDF2MWgtMXogTTczLDI0aDF2MWgtMXogTTc1LDI0aDJ2MWgtMnogTTgwLDI0aDF2M2gtMXogTTg0LDI0aDF2MWgtMXogTTQsMjVoMnY0aC0yeiBNMjIsMjVoMXYyaC0xeiBNMjQsMjVoMXYxaC0xeiBNMjksMjVoM3YxaC0zeiBNMzMsMjVoMXYxaC0xeiBNMzUsMjVoMnYxaC0yeiBNMzgsMjVoMXYxaC0xeiBNNDAsMjVoMXYxaC0xeiBNNTAsMjVoMXYyaC0xeiBNNTQsMjVoMnYxaC0yeiBNNTgsMjVoM3YxaC0zeiBNNjIsMjVoMXYyaC0xeiBNNjQsMjVoMnYzaC0yeiBNNjcsMjVoMXYyaC0xeiBNNzAsMjVoMXY5aC0xeiBNNzIsMjVoMXY1aC0xeiBNNzQsMjVoMXY0aC0xeiBNNzYsMjVoMnYyaC0yeiBNODMsMjVoMXYyaC0xeiBNMiwyNmgydjFoLTJ6IE02LDI2aDJ2MWgtMnogTTksMjZoNHYyaC00eiBNMTQsMjZoMXYxaC0xeiBNMTYsMjZoMnYyaC0yeiBNMTksMjZoM3YyaC0zeiBNMjgsMjZoMXY3aC0xeiBNMjksMjZoMXYxaC0xeiBNMzIsMjZoMXYxaC0xeiBNMzQsMjZoMXYyaC0xeiBNNDUsMjZoMXY1aC0xeiBNNTIsMjZoMXYxaC0xeiBNNTYsMjZoMXY3aC0xeiBNNTcsMjZoMXYyaC0xeiBNNjEsMjZoMXYyaC0xeiBNNjgsMjZoMXY2aC0xeiBNNjksMjZoMXYyaC0xeiBNNzMsMjZoMXYxaC0xeiBNODEsMjZoMXYxaC0xeiBNNywyN2gydjJoLTJ6IE0xMywyN2gxdjFoLTF6IE0yNCwyN2gxdjJoLTF6IE0zMSwyN2gxdjJoLTF6IE0zNiwyN2gxdjFoLTF6IE00MCwyN2gydjFoLTJ6IE00OCwyN2gxdjNoLTF6IE01MywyN2gzdjJoLTN6IE03MSwyN2gxdjNoLTF6IE03NywyN2gxdjJoLTF6IE03OSwyN2gxdjJoLTF6IE0xLDI4aDF2MWgtMXogTTYsMjhoMXYxaC0xeiBNOSwyOGgxdjFoLTF6IE0xMiwyOGgxdjFoLTF6IE0xNiwyOGgxdjFoLTF6IE0xOCwyOGgxdjJoLTF6IE0yMSwyOGgzdjFoLTN6IE0yNywyOGgxdjJoLTF6IE0yOSwyOGgydjFoLTJ6IE0zMiwyOGgxdjVoLTF6IE0zNSwyOGgxdjFoLTF6IE0zOSwyOGgxdjFoLTF6IE00NywyOGgxdjFoLTF6IE00OSwyOGgydjFoLTJ6IE01MiwyOGgxdjhoLTF6IE01OCwyOGgxdjJoLTF6IE02MiwyOGgxdjFoLTF6IE03NSwyOGgydjNoLTJ6IE03OCwyOGgxdjFoLTF6IE04MCwyOGgxdjVoLTF6IE04MywyOGgxdjNoLTF6IE0wLDI5aDF2MmgtMXogTTIsMjloMXYxaC0xeiBNNCwyOWgxdjRoLTF6IE04LDI5aDF2NGgtMXogTTEzLDI5aDJ2MWgtMnogTTE3LDI5aDF2MTFoLTF6IE0yMSwyOWgxdjFoLTF6IE0yNiwyOWgxdjNoLTF6IE00MCwyOWgxdjVoLTF6IE00MSwyOWgxdjFoLTF6IE00MywyOWgxdjFoLTF6IE01OSwyOWgzdjFoLTN6IE02MywyOWgxdjJoLTF6IE02NiwyOWgydjFoLTJ6IE03MywyOWgxdjJoLTF6IE04MSwyOWgydjFoLTJ6IE04NCwyOWgxdjNoLTF6IE02LDMwaDF2MWgtMXogTTEwLDMwaDF2MmgtMXogTTE0LDMwaDF2MmgtMXogTTE5LDMwaDJ2MWgtMnogTTIyLDMwaDF2MmgtMXogTTI0LDMwaDF2MWgtMXogTTMwLDMwaDF2MWgtMXogTTM0LDMwaDF2MWgtMXogTTM2LDMwaDJ2NGgtMnogTTM4LDMwaDJ2MWgtMnogTTQyLDMwaDF2MWgtMXogTTUwLDMwaDF2MWgtMXogTTU0LDMwaDF2MWgtMXogTTU5LDMwaDF2MmgtMXogTTYxLDMwaDF2MWgtMXogTTY0LDMwaDF2NmgtMXogTTY1LDMwaDF2MmgtMXogTTc0LDMwaDF2MWgtMXogTTc4LDMwaDF2MWgtMXogTTgyLDMwaDF2MWgtMXogTTIsMzFoMXYxaC0xeiBNOSwzMWgxdjFoLTF6IE0xMiwzMWgxdjFoLTF6IE0xNiwzMWgxdjFoLTF6IE0xOSwzMWgxdjFoLTF6IE0yMSwzMWgxdjJoLTF6IE0yMywzMWgxdjFoLTF6IE0yNSwzMWgxdjFoLTF6IE0yNywzMWgxdjRoLTF6IE0zMywzMWgxdjFoLTF6IE0zNSwzMWgxdjFoLTF6IE0zOSwzMWgxdjFoLTF6IE00MywzMWgxdjRoLTF6IE00NywzMWgydjFoLTJ6IE01OCwzMWgxdjVoLTF6IE02NywzMWgxdjNoLTF6IE02OSwzMWgxdjFoLTF6IE03MSwzMWgydjFoLTJ6IE03NiwzMWgxdjJoLTF6IE04MSwzMWgxdjFoLTF6IE01LDMyaDN2MWgtM3ogTTEzLDMyaDF2MWgtMXogTTIwLDMyaDF2MmgtMXogTTI5LDMyaDN2MWgtM3ogTTM4LDMyaDF2NGgtMXogTTQxLDMyaDJ2MmgtMnogTTUwLDMyaDF2M2gtMXogTTUzLDMyaDN2MWgtM3ogTTU3LDMyaDF2NGgtMXogTTYwLDMyaDN2MWgtM3ogTTY2LDMyaDF2M2gtMXogTTcyLDMyaDF2MWgtMXogTTc0LDMyaDF2MmgtMXogTTc3LDMyaDJ2MmgtMnogTTc5LDMyaDF2MWgtMXogTTgyLDMyaDJ2MWgtMnogTTAsMzNoMnYxaC0yeiBNMywzM2gxdjJoLTF6IE01LDMzaDF2MWgtMXogTTksMzNoMXY0aC0xeiBNMTEsMzNoMnYxaC0yeiBNMTYsMzNoMXY0aC0xeiBNMTksMzNoMXYyaC0xeiBNMjMsMzNoNHYxaC00eiBNMjksMzNoMXYyaC0xeiBNMzEsMzNoMXYxaC0xeiBNMzUsMzNoMXYzaC0xeiBNMzksMzNoMXYxaC0xeiBNNDYsMzNoM3YxaC0zeiBNNTUsMzNoMXYxaC0xeiBNNjAsMzNoMXYxaC0xeiBNNjksMzNoMXYzaC0xeiBNODEsMzNoMXY1aC0xeiBNODIsMzNoMXYxaC0xeiBNMSwzNGgxdjFoLTF6IE00LDM0aDF2MWgtMXogTTYsMzRoMXYxaC0xeiBNMTAsMzRoMXYzaC0xeiBNMTIsMzRoM3YxaC0zeiBNMTgsMzRoMXYyaC0xeiBNMjIsMzRoMXYxaC0xeiBNMjUsMzRoMnYxaC0yeiBNMzAsMzRoMXY0aC0xeiBNMzcsMzRoMXYxNGgtMXogTTQ0LDM0aDF2MmgtMXogTTQ2LDM0aDF2MWgtMXogTTQ4LDM0aDJ2NmgtMnogTTU0LDM0aDF2MmgtMXogTTU5LDM0aDF2MmgtMXogTTYzLDM0aDF2NWgtMXogTTc2LDM0aDF2MmgtMXogTTgsMzVoMXYxaC0xeiBNMTEsMzVoMXYyaC0xeiBNMTQsMzVoMXYyaC0xeiBNMjAsMzVoMXY2aC0xeiBNMjEsMzVoMXYxaC0xeiBNMjQsMzVoMXY2aC0xeiBNMzMsMzVoMnYxaC0yeiBNNDAsMzVoMXY0aC0xeiBNNDUsMzVoMXYxaC0xeiBNNTEsMzVoMXYyaC0xeiBNNTMsMzVoMXYyaC0xeiBNNTUsMzVoMXYxaC0xeiBNNjEsMzVoMnYxaC0yeiBNNjUsMzVoMXYxaC0xeiBNNjgsMzVoMXYxaC0xeiBNNzAsMzVoMXYyaC0xeiBNNzIsMzVoMXYxaC0xeiBNNzQsMzVoMXY0aC0xeiBNNzUsMzVoMXYxaC0xeiBNNzcsMzVoMXY0aC0xeiBNODIsMzVoM3YxaC0zeiBNMCwzNmgydjFoLTJ6IE00LDM2aDR2MWgtNHogTTE1LDM2aDF2MWgtMXogTTE5LDM2aDF2MWgtMXogTTIzLDM2aDF2MmgtMXogTTI1LDM2aDF2MWgtMXogTTI3LDM2aDF2MWgtMXogTTMxLDM2aDF2MWgtMXogTTMzLDM2aDF2MWgtMXogTTM2LDM2aDF2M2gtMXogTTQxLDM2aDF2MWgtMXogTTQzLDM2aDF2MWgtMXogTTQ3LDM2aDF2MWgtMXogTTUwLDM2aDF2M2gtMXogTTU2LDM2aDF2M2gtMXogTTYyLDM2aDF2NWgtMXogTTY2LDM2aDF2NGgtMXogTTczLDM2aDF2M2gtMXogTTc5LDM2aDF2MWgtMXogTTgyLDM2aDF2MWgtMXogTTg0LDM2aDF2MWgtMXogTTIsMzdoMXYzaC0xeiBNMTgsMzdoMXYzaC0xeiBNMjIsMzdoMXYyaC0xeiBNMjgsMzdoMXY4aC0xeiBNNDQsMzdoMXYxaC0xeiBNNTQsMzdoMXYzaC0xeiBNNTUsMzdoMXYxaC0xeiBNNjUsMzdoMXYxaC0xeiBNNjcsMzdoMnYxaC0yeiBNNzEsMzdoMXYxaC0xeiBNODMsMzdoMXYyaC0xeiBNMCwzOGgxdjJoLTF6IE0zLDM4aDJ2MWgtMnogTTYsMzhoNXYxaC01eiBNMTIsMzhoMXYzaC0xeiBNMTUsMzhoMnYxaC0yeiBNMTksMzhoMXYyaC0xeiBNMjEsMzhoMXYxaC0xeiBNMjYsMzhoMXYyaC0xeiBNMjksMzhoMXYyaC0xeiBNMzEsMzhoMnYxaC0yeiBNMzQsMzhoMXYxaC0xeiBNMzgsMzhoMXYxaC0xeiBNNDEsMzhoMnYxaC0yeiBNNDUsMzhoMXY0aC0xeiBNNjAsMzhoMXYxaC0xeiBNNjQsMzhoMXYyaC0xeiBNNjgsMzhoMnYxaC0yeiBNNzUsMzhoMXYxaC0xeiBNNzgsMzhoMXYzaC0xeiBNMSwzOWgxdjFoLTF6IE00LDM5aDJ2MWgtMnogTTcsMzloMXYxaC0xeiBNOSwzOWgxdjJoLTF6IE0xMSwzOWgxdjFoLTF6IE0xNCwzOWgxdjFoLTF6IE0xNiwzOWgxdjNoLTF6IE0yNSwzOWgxdjNoLTF6IE0yNywzOWgxdjFoLTF6IE0zMiwzOWgydjFoLTJ6IE0zOSwzOWgxdjJoLTF6IE00MiwzOWgxdjFoLTF6IE00NCwzOWgxdjFoLTF6IE00NiwzOWgxdjJoLTF6IE01MiwzOWgydjFoLTJ6IE01NSwzOWgxdjVoLTF6IE01NywzOWgxdjFoLTF6IE02NSwzOWgxdjJoLTF6IE02OCwzOWgxdjFoLTF6IE03MSwzOWgxdjRoLTF6IE04MCwzOWgxdjNoLTF6IE04NCwzOWgxdjFoLTF6IE01LDQwaDF2M2gtMXogTTYsNDBoMXYxaC0xeiBNOCw0MGgxdjJoLTF6IE0xMyw0MGgxdjJoLTF6IE0yMSw0MGgxdjJoLTF6IE0yMyw0MGgxdjFoLTF6IE0zMCw0MGgxdjNoLTF6IE0zNCw0MGgzdjFoLTN6IE0zOCw0MGgxdjFoLTF6IE00MCw0MGgxdjJoLTF6IE00Myw0MGgxdjFoLTF6IE00OCw0MGgxdjFoLTF6IE01Niw0MGgxdjJoLTF6IE01OCw0MGgydjFoLTJ6IE02MSw0MGgxdjNoLTF6IE02OSw0MGgydjFoLTJ6IE03NSw0MGgzdjFoLTN6IE03OSw0MGgxdjJoLTF6IE04Miw0MGgydjFoLTJ6IE0zLDQxaDJ2MWgtMnogTTEwLDQxaDF2MWgtMXogTTE1LDQxaDF2M2gtMXogTTE4LDQxaDJ2MWgtMnogTTIyLDQxaDF2MmgtMXogTTI2LDQxaDJ2MWgtMnogTTMxLDQxaDJ2MWgtMnogTTM1LDQxaDF2MWgtMXogTTQyLDQxaDF2MmgtMXogTTQ0LDQxaDF2M2gtMXogTTQ3LDQxaDF2M2gtMXogTTQ5LDQxaDF2MmgtMXogTTUzLDQxaDF2MWgtMXogTTY0LDQxaDF2MWgtMXogTTY4LDQxaDF2MWgtMXogTTcwLDQxaDF2NGgtMXogTTczLDQxaDF2M2gtMXogTTc1LDQxaDF2MmgtMXogTTgyLDQxaDF2MWgtMXogTTAsNDJoMXY0aC0xeiBNMiw0MmgxdjFoLTF6IE00LDQyaDF2MmgtMXogTTYsNDJoMXYxaC0xeiBNOSw0MmgxdjFoLTF6IE0xNCw0MmgxdjRoLTF6IE0yMCw0MmgxdjFoLTF6IE0yMyw0MmgxdjFoLTF6IE0yNyw0MmgxdjNoLTF6IE0yOSw0MmgxdjFoLTF6IE0zMSw0MmgxdjFoLTF6IE0zMyw0MmgxdjNoLTF6IE0zOCw0MmgxdjFoLTF6IE00MSw0MmgxdjFoLTF6IE00Myw0MmgxdjJoLTF6IE00Niw0MmgxdjFoLTF6IE00OCw0MmgxdjVoLTF6IE01Miw0MmgxdjNoLTF6IE02MCw0MmgxdjRoLTF6IE02Miw0MmgxdjJoLTF6IE02Nyw0MmgxdjFoLTF6IE03NCw0MmgxdjFoLTF6IE03Niw0MmgzdjFoLTN6IE04Myw0MmgydjFoLTJ6IE0zLDQzaDF2M2gtMXogTTEwLDQzaDF2MWgtMXogTTE4LDQzaDJ2MmgtMnogTTI0LDQzaDF2MWgtMXogTTMyLDQzaDF2MmgtMXogTTM0LDQzaDJ2M2gtMnogTTQwLDQzaDF2MWgtMXogTTU0LDQzaDF2NmgtMXogTTU3LDQzaDN2MWgtM3ogTTY0LDQzaDF2MmgtMXogTTY2LDQzaDF2MWgtMXogTTcyLDQzaDF2NWgtMXogTTgxLDQzaDF2MWgtMXogTTIsNDRoMXYxaC0xeiBNNSw0NGg1djFoLTV6IE0xMiw0NGgydjFoLTJ6IE0xNiw0NGgydjFoLTJ6IE0yMCw0NGg0djFoLTR6IE0zMCw0NGgxdjFoLTF6IE0zNiw0NGgxdjFoLTF6IE0zOCw0NGgydjFoLTJ6IE00MSw0NGgxdjFoLTF6IE00NSw0NGgxdjNoLTF6IE00Niw0NGgxdjFoLTF6IE01MSw0NGgxdjJoLTF6IE01Myw0NGgxdjJoLTF6IE01Niw0NGgydjFoLTJ6IE02MSw0NGgxdjFoLTF6IE02Nyw0NGgxdjFoLTF6IE02OSw0NGgxdjJoLTF6IE03NCw0NGgxdjFoLTF6IE03Niw0NGgydjFoLTJ6IE04Myw0NGgxdjFoLTF6IE01LDQ1aDF2MmgtMXogTTcsNDVoMXYxaC0xeiBNOSw0NWgzdjFoLTN6IE0xMyw0NWgxdjNoLTF6IE0xNyw0NWgxdjFoLTF6IE0yMyw0NWgxdjFoLTF6IE0yOSw0NWgxdjJoLTF6IE00MCw0NWgxdjZoLTF6IE00NCw0NWgxdjJoLTF6IE01MCw0NWgxdjFoLTF6IE02OCw0NWgxdjNoLTF6IE03Nyw0NWgydjJoLTJ6IE03OSw0NWgxdjFoLTF6IE04MSw0NWgydjFoLTJ6IE04NCw0NWgxdjFoLTF6IE0xLDQ2aDJ2MWgtMnogTTQsNDZoMXYxaC0xeiBNNiw0NmgxdjFoLTF6IE04LDQ2aDF2MWgtMXogTTEwLDQ2aDN2MWgtM3ogTTE1LDQ2aDF2M2gtMXogTTE2LDQ2aDF2MWgtMXogTTE4LDQ2aDF2M2gtMXogTTI0LDQ2aDF2MmgtMXogTTMwLDQ2aDN2MmgtM3ogTTMzLDQ2aDJ2MWgtMnogTTM4LDQ2aDF2MWgtMXogTTQxLDQ2aDF2MWgtMXogTTQzLDQ2aDF2MmgtMXogTTUyLDQ2aDF2MTdoLTF6IE01OCw0NmgydjJoLTJ6IE02MSw0NmgxdjNoLTF6IE02Myw0NmgydjFoLTJ6IE02Niw0NmgxdjJoLTF6IE03MCw0NmgydjFoLTJ6IE03Myw0NmgxdjJoLTF6IE03NSw0NmgxdjdoLTF6IE03Niw0NmgxdjJoLTF6IE04Miw0NmgxdjNoLTF6IE0yLDQ3aDF2MWgtMXogTTcsNDdoMXYxaC0xeiBNMTEsNDdoMnYxaC0yeiBNMTQsNDdoMXYxaC0xeiBNMjIsNDdoMXY2aC0xeiBNMjYsNDdoMXYyaC0xeiBNMzQsNDdoMnYxaC0yeiBNNDcsNDdoMXYxaC0xeiBNNDksNDdoMXYxaC0xeiBNNTEsNDdoMXYxaC0xeiBNNTUsNDdoMXYxaC0xeiBNNjAsNDdoMXYxaC0xeiBNNjIsNDdoMXYyaC0xeiBNNjcsNDdoMXYxaC0xeiBNNzEsNDdoMXYxaC0xeiBNNzQsNDdoMXYxaC0xeiBNNzcsNDdoMXYxaC0xeiBNNzksNDdoM3YxaC0zeiBNODMsNDdoMXYxaC0xeiBNMCw0OGgydjJoLTJ6IE02LDQ4aDF2MWgtMXogTTgsNDhoMXYyaC0xeiBNMTAsNDhoMXYxaC0xeiBNMTIsNDhoMXYxaC0xeiBNMTYsNDhoMnYxaC0yeiBNMTksNDhoMXY3aC0xeiBNMjAsNDhoMXYyaC0xeiBNMjUsNDhoMXYxaC0xeiBNMjcsNDhoNHYxaC00eiBNMzIsNDhoMXYxaC0xeiBNMzYsNDhoMXYzaC0xeiBNNDEsNDhoMXY5aC0xeiBNNDQsNDhoM3YxaC0zeiBNNTMsNDhoMXYxaC0xeiBNNTcsNDhoMXYxaC0xeiBNNjMsNDhoMnYxaC0yeiBNNjksNDhoMXYxaC0xeiBNNzgsNDhoMnYxaC0yeiBNODQsNDhoMXYyaC0xeiBNMiw0OWgxdjJoLTF6IE01LDQ5aDF2NGgtMXogTTksNDloMXYzaC0xeiBNMTMsNDloMXY1aC0xeiBNMTcsNDloMXYxaC0xeiBNMjMsNDloMnYxaC0yeiBNMjcsNDloMXYyaC0xeiBNMjksNDloMXY0aC0xeiBNMzEsNDloMXYxaC0xeiBNMzMsNDloM3YxaC0zeiBNMzgsNDloMXY0aC0xeiBNNDMsNDloMXYyaC0xeiBNNDUsNDloMnYxaC0yeiBNNDksNDloMnYyaC0yeiBNNTgsNDloMnYzaC0yeiBNNjMsNDloMXYxaC0xeiBNNjUsNDloMXYxaC0xeiBNNzAsNDloMXYxaC0xeiBNNzQsNDloMXYyaC0xeiBNNzcsNDloMXY0aC0xeiBNNzgsNDloMXYxaC0xeiBNODEsNDloMXY0aC0xeiBNMyw1MGgxdjFoLTF6IE02LDUwaDF2MWgtMXogTTEwLDUwaDF2MWgtMXogTTE1LDUwaDJ2MWgtMnogTTIxLDUwaDF2MmgtMXogTTIzLDUwaDF2MWgtMXogTTI1LDUwaDF2MWgtMXogTTI4LDUwaDF2MWgtMXogTTMwLDUwaDF2MWgtMXogTTMyLDUwaDF2OGgtMXogTTMzLDUwaDF2MWgtMXogTTM1LDUwaDF2MWgtMXogTTM3LDUwaDF2MWgtMXogTTQyLDUwaDF2M2gtMXogTTQ3LDUwaDF2NGgtMXogTTU1LDUwaDF2M2gtMXogTTYyLDUwaDF2MWgtMXogTTY0LDUwaDF2M2gtMXogTTY2LDUwaDJ2MmgtMnogTTY5LDUwaDF2MWgtMXogTTcyLDUwaDF2MWgtMXogTTc2LDUwaDF2MWgtMXogTTc5LDUwaDJ2MWgtMnogTTgyLDUwaDF2MWgtMXogTTAsNTFoMnYxaC0yeiBNNCw1MWgxdjhoLTF6IE04LDUxaDF2NmgtMXogTTExLDUxaDF2M2gtMXogTTEyLDUxaDF2MWgtMXogTTE2LDUxaDN2MmgtM3ogTTIwLDUxaDF2MmgtMXogTTI0LDUxaDF2MWgtMXogTTI2LDUxaDF2MmgtMXogTTM0LDUxaDF2MmgtMXogTTM5LDUxaDF2MWgtMXogTTQ0LDUxaDF2MWgtMXogTTQ4LDUxaDF2MmgtMXogTTUxLDUxaDF2MWgtMXogTTU0LDUxaDF2MmgtMXogTTU3LDUxaDF2MmgtMXogTTY1LDUxaDF2MmgtMXogTTY4LDUxaDF2MWgtMXogTTcxLDUxaDF2MmgtMXogTTczLDUxaDF2M2gtMXogTTc4LDUxaDF2MmgtMXogTTg0LDUxaDF2MWgtMXogTTMsNTJoMXYxaC0xeiBNNiw1MmgydjFoLTJ6IE0yMyw1MmgxdjFoLTF6IE0yNyw1MmgydjNoLTJ6IE0zMCw1MmgydjFoLTJ6IE0zMyw1MmgxdjFoLTF6IE0zNSw1MmgxdjNoLTF6IE0zNyw1MmgxdjFoLTF6IE00MCw1MmgxdjRoLTF6IE01MCw1MmgxdjJoLTF6IE01Myw1MmgxdjFoLTF6IE01Niw1MmgxdjZoLTF6IE01OSw1MmgxdjFoLTF6IE02MSw1MmgxdjVoLTF6IE03NCw1MmgxdjFoLTF6IE03Niw1MmgxdjZoLTF6IE03OSw1MmgydjFoLTJ6IE04Miw1MmgxdjFoLTF6IE0wLDUzaDF2MmgtMXogTTIsNTNoMXY1aC0xeiBNMTQsNTNoMXYzaC0xeiBNMTcsNTNoMXYxaC0xeiBNMjEsNTNoMXYzaC0xeiBNMjQsNTNoMXYxaC0xeiBNNDQsNTNoMXYxaC0xeiBNNTEsNTNoMXYxaC0xeiBNNjAsNTNoMXYxaC0xeiBNNjMsNTNoMXYzaC0xeiBNNjYsNTNoMXYxaC0xeiBNNjgsNTNoMXYyaC0xeiBNNzAsNTNoMXYyaC0xeiBNODAsNTNoMXY0aC0xeiBNODQsNTNoMXYzaC0xeiBNNiw1NGgxdjFoLTF6IE0xMCw1NGgxdjVoLTF6IE0xMiw1NGgxdjFoLTF6IE0yMCw1NGgxdjVoLTF6IE0yMyw1NGgxdjFoLTF6IE0yNiw1NGgxdjFoLTF6IE0zMCw1NGgxdjFoLTF6IE0zMyw1NGgxdjFoLTF6IE0zOCw1NGgydjFoLTJ6IE00Myw1NGgxdjJoLTF6IE00Niw1NGgxdjJoLTF6IE00OSw1NGgxdjFoLTF6IE01NCw1NGgxdjFoLTF6IE01OCw1NGgydjJoLTJ6IE02Miw1NGgxdjFoLTF6IE02NCw1NGgxdjNoLTF6IE02OSw1NGgxdjJoLTF6IE03NCw1NGgxdjNoLTF6IE03NSw1NGgxdjFoLTF6IE03OCw1NGgxdjFoLTF6IE04MSw1NGgzdjFoLTN6IE0xMSw1NWgxdjFoLTF6IE0xMyw1NWgxdjFoLTF6IE0yOCw1NWgxdjZoLTF6IE0zNCw1NWgxdjFoLTF6IE0zNiw1NWgxdjFoLTF6IE00Miw1NWgxdjFoLTF6IE00NSw1NWgxdjRoLTF6IE00Nyw1NWgxdjJoLTF6IE01MCw1NWgxdjFoLTF6IE02MCw1NWgxdjFoLTF6IE02NSw1NWgxdjFoLTF6IE03MSw1NWgxdjFoLTF6IE03Myw1NWgxdjJoLTF6IE0wLDU2aDF2MWgtMXogTTUsNTZoM3YxaC0zeiBNOSw1NmgxdjFoLTF6IE0xMiw1NmgxdjJoLTF6IE0xNiw1NmgxdjJoLTF6IE0xOSw1NmgxdjFoLTF6IE0yNiw1NmgxdjNoLTF6IE0yOSw1NmgxdjRoLTF6IE0zMCw1NmgydjFoLTJ6IE0zNSw1NmgxdjFoLTF6IE0zOCw1NmgxdjJoLTF6IE00OSw1NmgxdjFoLTF6IE01Myw1NmgxdjZoLTF6IE01NCw1NmgydjFoLTJ6IE01OCw1NmgxdjFoLTF6IE02Nyw1NmgxdjFoLTF6IE03MCw1NmgxdjFoLTF6IE03Miw1NmgxdjJoLTF6IE03NSw1NmgxdjFoLTF6IE03Nyw1NmgzdjNoLTN6IE04MSw1NmgxdjFoLTF6IE03LDU3aDF2MWgtMXogTTEzLDU3aDF2MmgtMXogTTE3LDU3aDF2NGgtMXogTTE4LDU3aDF2MWgtMXogTTIxLDU3aDJ2MWgtMnogTTI0LDU3aDF2NGgtMXogTTMxLDU3aDF2MmgtMXogTTM0LDU3aDF2MmgtMXogTTQwLDU3aDF2MmgtMXogTTQzLDU3aDJ2MWgtMnogTTQ2LDU3aDF2MmgtMXogTTQ4LDU3aDF2MWgtMXogTTU0LDU3aDF2MWgtMXogTTU3LDU3aDF2MWgtMXogTTU5LDU3aDJ2MWgtMnogTTYyLDU3aDJ2MWgtMnogTTY2LDU3aDF2MmgtMXogTTY5LDU3aDF2MWgtMXogTTgyLDU3aDJ2M2gtMnogTTAsNThoMXYyaC0xeiBNNSw1OGgydjFoLTJ6IE04LDU4aDF2MWgtMXogTTE5LDU4aDF2M2gtMXogTTI3LDU4aDF2MWgtMXogTTM1LDU4aDJ2M2gtMnogTTM3LDU4aDF2MWgtMXogTTM5LDU4aDF2MWgtMXogTTQyLDU4aDF2MmgtMXogTTQ3LDU4aDF2MmgtMXogTTQ5LDU4aDJ2MWgtMnogTTU4LDU4aDF2NGgtMXogTTYwLDU4aDF2NGgtMXogTTYxLDU4aDF2MWgtMXogTTYzLDU4aDN2MWgtM3ogTTY3LDU4aDJ2MWgtMnogTTcwLDU4aDJ2MWgtMnogTTgwLDU4aDF2MWgtMXogTTEsNTloMXYyaC0xeiBNNSw1OWgxdjFoLTF6IE03LDU5aDF2MmgtMXogTTksNTloMXYxaC0xeiBNMTQsNTloMXYxaC0xeiBNMTgsNTloMXYzaC0xeiBNMjIsNTloMXYxaC0xeiBNMzAsNTloMXYyaC0xeiBNMzIsNTloMXYxaC0xeiBNNDEsNTloMXYyaC0xeiBNNDgsNTloMXY2aC0xeiBNNTAsNTloMnYxaC0yeiBNNTQsNTloM3YxaC0zeiBNNjMsNTloMXYyaC0xeiBNNjgsNTloMnYyaC0yeiBNNzIsNTloM3YxaC0zeiBNNzgsNTloMXYzaC0xeiBNODEsNTloMXYxaC0xeiBNODQsNTloMXYxaC0xeiBNMyw2MGgydjFoLTJ6IE02LDYwaDF2MWgtMXogTTExLDYwaDF2MWgtMXogTTE1LDYwaDJ2MWgtMnogTTIwLDYwaDJ2MmgtMnogTTIzLDYwaDF2M2gtMXogTTMxLDYwaDF2MWgtMXogTTM0LDYwaDF2MWgtMXogTTM5LDYwaDF2NGgtMXogTTQzLDYwaDR2MWgtNHogTTUwLDYwaDF2MWgtMXogTTU1LDYwaDN2MWgtM3ogTTYxLDYwaDJ2M2gtMnogTTY2LDYwaDF2NGgtMXogTTc0LDYwaDR2MWgtNHogTTc5LDYwaDF2MmgtMXogTTksNjFoMnYxaC0yeiBNMTMsNjFoMnYxaC0yeiBNMjIsNjFoMXYxaC0xeiBNMjcsNjFoMXY0aC0xeiBNMzIsNjFoMnYyaC0yeiBNMzUsNjFoMXYyaC0xeiBNMzcsNjFoMXYyaC0xeiBNNDIsNjFoMXYyaC0xeiBNNDYsNjFoMnYxaC0yeiBNNDksNjFoMXYxaC0xeiBNNTEsNjFoMXYyaC0xeiBNNjQsNjFoMnYzaC0yeiBNNjcsNjFoMXYxaC0xeiBNNzAsNjFoMnYyaC0yeiBNNzUsNjFoMnYxaC0yeiBNODAsNjFoMXYyaC0xeiBNODQsNjFoMXYxaC0xeiBNMSw2MmgxdjdoLTF6IE0yLDYyaDN2MWgtM3ogTTYsNjJoMnYxaC0yeiBNMTEsNjJoMXYxaC0xeiBNMTQsNjJoMnYxaC0yeiBNMTcsNjJoMXYxaC0xeiBNMjEsNjJoMXYxaC0xeiBNMjUsNjJoMXY2aC0xeiBNMjYsNjJoMXYxaC0xeiBNMjksNjJoMXY1aC0xeiBNNDMsNjJoM3YxaC0zeiBNNDcsNjJoMXYyaC0xeiBNNTAsNjJoMXY0aC0xeiBNNTYsNjJoMXYzaC0xeiBNNjksNjJoMXYxaC0xeiBNNzIsNjJoM3YxaC0zeiBNNzcsNjJoMXYxaC0xeiBNODIsNjJoMXYxaC0xeiBNMCw2M2gxdjFoLTF6IE00LDYzaDF2MWgtMXogTTEyLDYzaDF2M2gtMXogTTE0LDYzaDF2MWgtMXogTTE2LDYzaDF2M2gtMXogTTE4LDYzaDF2MWgtMXogTTIwLDYzaDF2MWgtMXogTTMwLDYzaDF2MmgtMXogTTM0LDYzaDF2MmgtMXogTTQxLDYzaDF2MmgtMXogTTQ1LDYzaDF2MmgtMXogTTUzLDYzaDJ2MWgtMnogTTU3LDYzaDF2MWgtMXogTTYzLDYzaDF2MWgtMXogTTY3LDYzaDJ2MWgtMnogTTcwLDYzaDF2MWgtMXogTTc2LDYzaDF2MmgtMXogTTc4LDYzaDJ2MWgtMnogTTg0LDYzaDF2NWgtMXogTTMsNjRoMXYzaC0xeiBNNSw2NGgxdjNoLTF6IE02LDY0aDF2MWgtMXogTTgsNjRoMXYzaC0xeiBNMTEsNjRoMXYzaC0xeiBNMTMsNjRoMXY0aC0xeiBNMTcsNjRoMXYyaC0xeiBNMjMsNjRoMnYxaC0yeiBNMjYsNjRoMXY0aC0xeiBNMzMsNjRoMXYxaC0xeiBNNDAsNjRoMXYyaC0xeiBNNDYsNjRoMXY0aC0xeiBNNDksNjRoMXYzaC0xeiBNNTEsNjRoMXYzaC0xeiBNNTIsNjRoMXYxaC0xeiBNNTUsNjRoMXYzaC0xeiBNNTgsNjRoM3YxaC0zeiBNNjIsNjRoMXYyaC0xeiBNNjUsNjRoMXYxaC0xeiBNNjgsNjRoMXYxaC0xeiBNNzEsNjRoMXYxaC0xeiBNNzMsNjRoMXYzaC0xeiBNNzksNjRoMXYzaC0xeiBNODEsNjRoMnYyaC0yeiBNMCw2NWgxdjRoLTF6IE03LDY1aDF2MWgtMXogTTksNjVoMnYyaC0yeiBNMTQsNjVoMXYxaC0xeiBNMTgsNjVoMXYyaC0xeiBNMjAsNjVoMXYxaC0xeiBNMjIsNjVoMnYxaC0yeiBNMjgsNjVoMXYxaC0xeiBNMzIsNjVoMXYxaC0xeiBNMzcsNjVoMXYxaC0xeiBNMzksNjVoMXY1aC0xeiBNNDIsNjVoMXYxaC0xeiBNNDQsNjVoMXYyaC0xeiBNNDcsNjVoMXYxaC0xeiBNNTMsNjVoMnYxaC0yeiBNNTksNjVoMXYxaC0xeiBNNjEsNjVoMXYxaC0xeiBNNjMsNjVoMXYxaC0xeiBNNzAsNjVoMXY0aC0xeiBNNzQsNjVoMnYyaC0yeiBNNzgsNjVoMXYxaC0xeiBNNCw2NmgxdjJoLTF6IE02LDY2aDF2MWgtMXogTTE1LDY2aDF2MWgtMXogTTE5LDY2aDF2MmgtMXogTTI0LDY2aDF2MmgtMXogTTMwLDY2aDF2MWgtMXogTTM0LDY2aDN2MWgtM3ogTTQzLDY2aDF2NWgtMXogTTQ1LDY2aDF2MmgtMXogTTU0LDY2aDF2MmgtMXogTTU2LDY2aDN2MWgtM3ogTTY0LDY2aDF2M2gtMXogTTY5LDY2aDF2MmgtMXogTTcyLDY2aDF2MmgtMXogTTc3LDY2aDF2OGgtMXogTTgyLDY2aDJ2MWgtMnogTTIsNjdoMXYzaC0xeiBNNyw2N2gxdjJoLTF6IE0xNiw2N2gxdjFoLTF6IE0yMCw2N2gydjFoLTJ6IE0yMyw2N2gxdjhoLTF6IE0yNyw2N2gxdjNoLTF6IE0zMiw2N2gydjFoLTJ6IE0zNiw2N2gxdjFoLTF6IE00MCw2N2gxdjNoLTF6IE00OCw2N2gxdjFoLTF6IE01Niw2N2gxdjFoLTF6IE02MSw2N2gxdjJoLTF6IE02Myw2N2gxdjFoLTF6IE02Nyw2N2gxdjVoLTF6IE02OCw2N2gxdjFoLTF6IE03MSw2N2gxdjFoLTF6IE03NSw2N2gxdjFoLTF6IE04MCw2N2gydjJoLTJ6IE01LDY4aDJ2MWgtMnogTTgsNjhoMnYxaC0yeiBNMTgsNjhoMXYxaC0xeiBNMjgsNjhoMXYzaC0xeiBNMjksNjhoMXYxaC0xeiBNMzUsNjhoMXYxaC0xeiBNNDEsNjhoMXY0aC0xeiBNNDcsNjhoMXYzaC0xeiBNNDksNjhoMXY0aC0xeiBNNTAsNjhoMnYxaC0yeiBNNTcsNjhoMXY1aC0xeiBNNTgsNjhoMXYxaC0xeiBNNzMsNjhoMnYxaC0yeiBNNzYsNjhoMXY2aC0xeiBNNzksNjhoMXYzaC0xeiBNODMsNjhoMXYyaC0xeiBNMyw2OWgxdjVoLTF6IE00LDY5aDF2MmgtMXogTTksNjloMnY1aC0yeiBNMTQsNjloM3YxaC0zeiBNMTksNjloNHYxaC00eiBNMjQsNjloMXYxaC0xeiBNMzAsNjloMXY0aC0xeiBNMzEsNjloMXYxaC0xeiBNMzQsNjloMXYyaC0xeiBNMzYsNjloM3YxaC0zeiBNNDQsNjloMXYyaC0xeiBNNDgsNjloMXYxaC0xeiBNNTIsNjloMXYyaC0xeiBNNTksNjloMnYyaC0yeiBNNjMsNjloMXYxaC0xeiBNNjksNjloMXYxaC0xeiBNNzEsNjloMnYxaC0yeiBNODEsNjloMnYxaC0yeiBNODQsNjloMXYxaC0xeiBNMCw3MGgxdjFoLTF6IE02LDcwaDF2MWgtMXogTTExLDcwaDJ2MWgtMnogTTE5LDcwaDF2MWgtMXogTTIxLDcwaDJ2MWgtMnogTTI2LDcwaDF2M2gtMXogTTI5LDcwaDF2M2gtMXogTTM1LDcwaDF2MWgtMXogTTM3LDcwaDJ2MWgtMnogTTQyLDcwaDF2M2gtMXogTTQ1LDcwaDF2MmgtMXogTTUwLDcwaDJ2MWgtMnogTTUzLDcwaDJ2MWgtMnogTTYxLDcwaDF2MWgtMXogTTY0LDcwaDN2MWgtM3ogTTcwLDcwaDF2MWgtMXogTTgyLDcwaDF2M2gtMXogTTEsNzFoMXYxaC0xeiBNNyw3MWgxdjRoLTF6IE04LDcxaDF2MWgtMXogTTEyLDcxaDN2MmgtM3ogTTE1LDcxaDJ2MWgtMnogTTIwLDcxaDF2MmgtMXogTTIyLDcxaDF2MWgtMXogTTI3LDcxaDF2MWgtMXogTTMxLDcxaDF2M2gtMXogTTMzLDcxaDF2MWgtMXogTTUwLDcxaDF2MWgtMXogTTU0LDcxaDJ2MWgtMnogTTU4LDcxaDF2MWgtMXogTTYwLDcxaDF2N2gtMXogTTcyLDcxaDF2MWgtMXogTTc0LDcxaDF2MWgtMXogTTgwLDcxaDF2MTNoLTF6IE0wLDcyaDF2MWgtMXogTTYsNzJoMXYxaC0xeiBNMTEsNzJoMXYzaC0xeiBNMTcsNzJoMXYxaC0xeiBNMjQsNzJoMXYxaC0xeiBNMzQsNzJoMXYyaC0xeiBNMzcsNzJoMXYxaC0xeiBNMzksNzJoMnYxaC0yeiBNNDYsNzJoMnYxaC0yeiBNNTMsNzJoMXYxaC0xeiBNNTUsNzJoMnYxaC0yeiBNNjEsNzJoM3YxaC0zeiBNNjUsNzJoMnY0aC0yeiBNNjgsNzJoMXYyaC0xeiBNNzAsNzJoMXYyaC0xeiBNNzgsNzJoMnYxaC0yeiBNODMsNzJoMnYyaC0yeiBNMiw3M2gxdjJoLTF6IE00LDczaDJ2MWgtMnogTTEyLDczaDF2NGgtMXogTTE5LDczaDF2MmgtMXogTTIyLDczaDF2M2gtMXogTTI3LDczaDF2MmgtMXogTTMyLDczaDF2OGgtMXogTTM1LDczaDF2MWgtMXogTTM4LDczaDJ2MWgtMnogTTQ1LDczaDF2NGgtMXogTTQ2LDczaDF2MWgtMXogTTQ5LDczaDF2M2gtMXogTTUwLDczaDF2MWgtMXogTTU2LDczaDF2MmgtMXogTTU4LDczaDF2MWgtMXogTTYyLDczaDJ2MmgtMnogTTcyLDczaDF2M2gtMXogTTc0LDczaDF2NGgtMXogTTc5LDczaDF2MWgtMXogTTgxLDczaDF2MmgtMXogTTAsNzRoMnYxaC0yeiBNNCw3NGgxdjFoLTF6IE02LDc0aDF2MWgtMXogTTgsNzRoMnYxaC0yeiBNMTQsNzRoNHYxaC00eiBNMjEsNzRoMXYxaC0xeiBNMjUsNzRoMXY2aC0xeiBNMjYsNzRoMXYxaC0xeiBNMjgsNzRoMXY3aC0xeiBNMjksNzRoMXYxaC0xeiBNMzMsNzRoMXYxaC0xeiBNMzYsNzRoMnYxaC0yeiBNNDEsNzRoNHYxaC00eiBNNDcsNzRoMXYxaC0xeiBNNTEsNzRoMXY0aC0xeiBNNTcsNzRoMXYyaC0xeiBNNTksNzRoMXY0aC0xeiBNNjcsNzRoMXYyaC0xeiBNNzMsNzRoMXYyaC0xeiBNNzgsNzRoMXYxaC0xeiBNODMsNzRoMXYyaC0xeiBNMSw3NWgxdjFoLTF6IE0zLDc1aDF2MWgtMXogTTUsNzVoMXYxaC0xeiBNOCw3NWgxdjFoLTF6IE0xNCw3NWgxdjNoLTF6IE0xNyw3NWgydjFoLTJ6IE0yMCw3NWgxdjJoLTF6IE0yNCw3NWgxdjFoLTF6IE0zMCw3NWgydjJoLTJ6IE0zNSw3NWgxdjFoLTF6IE0zOCw3NWgydjNoLTJ6IE00Myw3NWgxdjFoLTF6IE02Myw3NWgydjFoLTJ6IE02OCw3NWgydjFoLTJ6IE03MSw3NWgxdjNoLTF6IE03OSw3NWgxdjJoLTF6IE0wLDc2aDF2MWgtMXogTTYsNzZoMXYxaC0xeiBNOSw3NmgydjFoLTJ6IE0xMyw3NmgxdjRoLTF6IE0xNSw3NmgydjFoLTJ6IE0xOCw3NmgxdjFoLTF6IE0yNyw3NmgxdjFoLTF6IE0yOSw3NmgxdjFoLTF6IE0zMyw3NmgydjFoLTJ6IE0zNyw3NmgxdjFoLTF6IE00MCw3NmgzdjFoLTN6IE00NCw3NmgxdjJoLTF6IE00Niw3NmgxdjFoLTF6IE00OCw3NmgxdjFoLTF6IE01MCw3NmgxdjJoLTF6IE01Miw3Nmg1djFoLTV6IE01OCw3NmgxdjFoLTF6IE02NCw3NmgxdjJoLTF6IE02Niw3NmgxdjFoLTF6IE03MCw3NmgxdjloLTF6IE03Niw3NmgxdjZoLTF6IE03Nyw3NmgydjFoLTJ6IE04Miw3NmgxdjNoLTF6IE04NCw3NmgxdjRoLTF6IE04LDc3aDF2MmgtMXogTTEwLDc3aDF2NGgtMXogTTM0LDc3aDJ2MmgtMnogTTQxLDc3aDF2MmgtMXogTTQ3LDc3aDF2NWgtMXogTTUyLDc3aDF2NGgtMXogTTU2LDc3aDF2NGgtMXogTTYxLDc3aDF2MmgtMXogTTYzLDc3aDF2M2gtMXogTTY1LDc3aDF2MWgtMXogTTY4LDc3aDF2MWgtMXogTTcyLDc3aDF2NWgtMXogTTczLDc3aDF2MWgtMXogTTAsNzhoN3YxaC03eiBNMTEsNzhoMXYxaC0xeiBNMTUsNzhoMXYxaC0xeiBNMTcsNzhoMXYyaC0xeiBNMTksNzhoMnYxaC0yeiBNMjIsNzhoMXYyaC0xeiBNMjQsNzhoMXYyaC0xeiBNMjYsNzhoMXYxaC0xeiBNMzAsNzhoMXYxaC0xeiBNMzMsNzhoMXY2aC0xeiBNMzcsNzhoMXYxaC0xeiBNMzksNzhoMnYxaC0yeiBNNDIsNzhoMXYzaC0xeiBNNDMsNzhoMXYxaC0xeiBNNDgsNzhoMnYxaC0yeiBNNTQsNzhoMXYxaC0xeiBNNjIsNzhoMXYyaC0xeiBNNjksNzhoMXYxaC0xeiBNNzQsNzhoMnYxaC0yeiBNNzgsNzhoMXYxaC0xeiBNODMsNzhoMXYxaC0xeiBNMCw3OWgxdjZoLTF6IE02LDc5aDF2NmgtMXogTTksNzloMXYyaC0xeiBNMTgsNzloMXYxaC0xeiBNMjAsNzloMnYxaC0yeiBNMjMsNzloMXY0aC0xeiBNMjcsNzloMXYyaC0xeiBNMzQsNzloMXYxaC0xeiBNMzgsNzloMnYxaC0yeiBNNDQsNzloMXYzaC0xeiBNNDYsNzloMXY1aC0xeiBNNDksNzloMnYyaC0yeiBNNTgsNzloMXYyaC0xeiBNNjQsNzloMnYxaC0yeiBNNzMsNzloMXYyaC0xeiBNNzUsNzloMXYxaC0xeiBNODEsNzloMXY0aC0xeiBNMiw4MGgzdjNoLTN6IE0xMSw4MGgxdjVoLTF6IE0xNSw4MGgxdjFoLTF6IE0xOSw4MGgxdjRoLTF6IE0yMSw4MGgxdjJoLTF6IE0yOSw4MGgzdjFoLTN6IE0zNSw4MGgxdjFoLTF6IE0zNyw4MGgydjFoLTJ6IE00MCw4MGgxdjJoLTF6IE00OCw4MGgxdjFoLTF6IE01MSw4MGgxdjJoLTF6IE01Myw4MGgzdjFoLTN6IE02MCw4MGgydjFoLTJ6IE02NCw4MGgxdjFoLTF6IE02Nyw4MGgydjJoLTJ6IE03NCw4MGgxdjRoLTF6IE03Nyw4MGgydjJoLTJ6IE03OSw4MGgxdjFoLTF6IE04Miw4MGgxdjFoLTF6IE04LDgxaDF2MmgtMXogTTEzLDgxaDJ2MWgtMnogTTE4LDgxaDF2NGgtMXogTTIyLDgxaDF2MmgtMXogTTMwLDgxaDJ2MWgtMnogTTM0LDgxaDF2MmgtMXogTTM2LDgxaDF2MWgtMXogTTM5LDgxaDF2NGgtMXogTTQxLDgxaDF2MWgtMXogTTQzLDgxaDF2NGgtMXogTTQ5LDgxaDF2MWgtMXogTTUzLDgxaDF2MWgtMXogTTU3LDgxaDF2MWgtMXogTTYyLDgxaDJ2MWgtMnogTTY5LDgxaDF2MmgtMXogTTcxLDgxaDF2M2gtMXogTTg0LDgxaDF2MWgtMXogTTEwLDgyaDF2MWgtMXogTTE0LDgyaDR2MWgtNHogTTIwLDgyaDF2MWgtMXogTTI0LDgyaDF2MWgtMXogTTI2LDgyaDF2M2gtMXogTTI4LDgyaDF2MWgtMXogTTM1LDgyaDF2MmgtMXogTTQ4LDgyaDF2MmgtMXogTTUwLDgyaDF2MmgtMXogTTU1LDgyaDF2MWgtMXogTTU4LDgyaDR2MWgtNHogTTY1LDgyaDF2M2gtMXogTTczLDgyaDF2MmgtMXogTTc5LDgyaDF2MWgtMXogTTgzLDgyaDF2MWgtMXogTTksODNoMXYyaC0xeiBNMTMsODNoMXYxaC0xeiBNMTYsODNoMnYxaC0yeiBNMjEsODNoMXYyaC0xeiBNMjUsODNoMXYyaC0xeiBNMzAsODNoM3YxaC0zeiBNMzcsODNoMXYxaC0xeiBNNDIsODNoMXYyaC0xeiBNNDUsODNoMXYyaC0xeiBNNDcsODNoMXYxaC0xeiBNNDksODNoMXYyaC0xeiBNNTEsODNoMXYyaC0xeiBNNTQsODNoMXYyaC0xeiBNNTYsODNoMXYyaC0xeiBNNTksODNoMXYxaC0xeiBNNjEsODNoM3YxaC0zeiBNNjYsODNoMXYxaC0xeiBNNjgsODNoMXYxaC0xeiBNNzIsODNoMXYxaC0xeiBNNzUsODNoMXYyaC0xeiBNNzcsODNoMnYxaC0yeiBNODIsODNoMXYxaC0xeiBNMSw4NGg1djFoLTV6IE0xMiw4NGgxdjFoLTF6IE0xNSw4NGgydjFoLTJ6IE0yMCw4NGgxdjFoLTF6IE0yMyw4NGgxdjFoLTF6IE0yOSw4NGgxdjFoLTF6IE0zMSw4NGgydjFoLTJ6IE0zNCw4NGgxdjFoLTF6IE0zOCw4NGgxdjFoLTF6IE00MCw4NGgydjFoLTJ6IE00NCw4NGgxdjFoLTF6IE02MCw4NGgxdjFoLTF6IE02Myw4NGgydjFoLTJ6IE02OSw4NGgxdjFoLTF6IE04Myw4NGgydjFoLTJ6IiBmaWxsPSIjMDAwMDAwIi8+Cjwvc3ZnPgo="
                QrImage = "data:image/svg+xml;charset=utf-8;base64, PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4KPCFET0NUWVBFIHN2ZyBQVUJMSUMgIi0vL1czQy8vRFREIFNWRyAxLjEvL0VOIiAiaHR0cDovL3d3dy53My5vcmcvR3JhcGhpY3MvU1ZHLzEuMS9EVEQvc3ZnMTEuZHRkIj4KPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZlcnNpb249IjEuMSIgdmlld0JveD0iMCAwIDM3IDM3IiBzdHJva2U9Im5vbmUiPgoJPHJlY3Qgd2lkdGg9IjEwMCUiIGhlaWdodD0iMTAwJSIgZmlsbD0iI2ZmZmZmZiIvPgoJPHBhdGggZD0iTTAsMGg3djFoLTd6IE05LDBoMXYxaC0xeiBNMTEsMGgydjFoLTJ6IE0xNSwwaDF2MWgtMXogTTE3LDBoMnYxaC0yeiBNMjAsMGgxdjFoLTF6IE0yNSwwaDR2MWgtNHogTTMwLDBoN3YxaC03eiBNMCwxaDF2NmgtMXogTTYsMWgxdjZoLTF6IE04LDFoMXYxaC0xeiBNMTEsMWgxdjFoLTF6IE0xNCwxaDF2MWgtMXogTTE2LDFoMXYxaC0xeiBNMTksMWgxdjVoLTF6IE0yMSwxaDJ2MWgtMnogTTI0LDFoMXYxaC0xeiBNMjgsMWgxdjFoLTF6IE0zMCwxaDF2NmgtMXogTTM2LDFoMXY2aC0xeiBNMiwyaDN2M2gtM3ogTTEyLDJoMnYyaC0yeiBNMTUsMmgxdjFoLTF6IE0xOCwyaDF2MmgtMXogTTIwLDJoMXYxaC0xeiBNMjMsMmgxdjFoLTF6IE0yNywyaDF2MWgtMXogTTMyLDJoM3YzaC0zeiBNOSwzaDJ2MWgtMnogTTE0LDNoMXYxaC0xeiBNMjEsM2gydjJoLTJ6IE0yNSwzaDJ2MmgtMnogTTI4LDNoMXYxaC0xeiBNMTAsNGgxdjZoLTF6IE0xMyw0aDF2MWgtMXogTTE1LDRoMXYyaC0xeiBNMTcsNGgxdjFoLTF6IE0yNyw0aDF2MWgtMXogTTgsNWgxdjNoLTF6IE05LDVoMXYxaC0xeiBNMTEsNWgydjFoLTJ6IE0xOCw1aDF2M2gtMXogTTIwLDVoMXY1aC0xeiBNMjIsNWgxdjRoLTF6IE0yMyw1aDF2MWgtMXogTTI1LDVoMXYxaC0xeiBNMjgsNWgxdjJoLTF6IE0xLDZoNXYxaC01eiBNMTIsNmgxdjFoLTF6IE0xNCw2aDF2NGgtMXogTTE2LDZoMXYyaC0xeiBNMjQsNmgxdjFoLTF6IE0yNiw2aDF2MmgtMXogTTMxLDZoNXYxaC01eiBNMTUsN2gxdjJoLTF6IE0xOSw3aDF2MmgtMXogTTIzLDdoMXYxaC0xeiBNNCw4aDR2MWgtNHogTTExLDhoMXY0aC0xeiBNMTIsOGgxdjFoLTF6IE0xNyw4aDF2NGgtMXogTTI1LDhoMXYxaC0xeiBNMjcsOGgxdjFoLTF6IE0zMCw4aDJ2MWgtMnogTTM1LDhoMXYxaC0xeiBNMCw5aDF2MmgtMXogTTQsOWgxdjNoLTF6IE01LDloMXYxaC0xeiBNNyw5aDN2MmgtM3ogTTE2LDloMXYxaC0xeiBNMTgsOWgxdjFoLTF6IE0yMSw5aDF2MWgtMXogTTIzLDloMnYxaC0yeiBNMjYsOWgxdjNoLTF6IE0yOCw5aDF2MWgtMXogTTMwLDloMXYxaC0xeiBNMzIsOWgzdjFoLTN6IE0zLDEwaDF2MmgtMXogTTYsMTBoMXYxaC0xeiBNMTMsMTBoMXYxaC0xeiBNMTUsMTBoMXYxaC0xeiBNMjIsMTBoMXYyaC0xeiBNMjUsMTBoMXYxaC0xeiBNMjcsMTBoMXYxaC0xeiBNMjksMTBoMXY3aC0xeiBNMzEsMTBoMXYzaC0xeiBNMzIsMTBoMXYxaC0xeiBNMzUsMTBoMXYxaC0xeiBNMSwxMWgydjJoLTJ6IE01LDExaDF2MWgtMXogTTcsMTFoMXYxaC0xeiBNOSwxMWgxdjFoLTF6IE0xNCwxMWgxdjNoLTF6IE0xOCwxMWgxdjVoLTF6IE0yMSwxMWgxdjJoLTF6IE0yMywxMWgxdjJoLTF6IE0yOCwxMWgxdjFoLTF6IE0zNCwxMWgxdjFoLTF6IE0zNiwxMWgxdjJoLTF6IE02LDEyaDF2MWgtMXogTTgsMTJoMXY0aC0xeiBNMTMsMTJoMXYxaC0xeiBNMTUsMTJoMnYxaC0yeiBNMTksMTJoMXY2aC0xeiBNMjAsMTJoMXYxaC0xeiBNMzAsMTJoMXYxaC0xeiBNNCwxM2gxdjFoLTF6IE03LDEzaDF2MWgtMXogTTEwLDEzaDF2MWgtMXogTTE2LDEzaDJ2MWgtMnogTTIyLDEzaDF2MWgtMXogTTI1LDEzaDF2MWgtMXogTTI3LDEzaDJ2MWgtMnogTTMyLDEzaDF2MWgtMXogTTM0LDEzaDF2NGgtMXogTTM1LDEzaDF2MWgtMXogTTIsMTRoMXYxaC0xeiBNNiwxNGgxdjFoLTF6IE0xMSwxNGgzdjFoLTN6IE0xNSwxNGgxdjRoLTF6IE0yMSwxNGgxdjJoLTF6IE0yNCwxNGgxdjJoLTF6IE0yNiwxNGgydjFoLTJ6IE0zMSwxNGgxdjFoLTF6IE0zMywxNGgxdjFoLTF6IE0zLDE1aDF2MmgtMXogTTcsMTVoMXYxaC0xeiBNOSwxNWgzdjFoLTN6IE0xMywxNWgxdjFoLTF6IE0yMCwxNWgxdjJoLTF6IE0yMiwxNWgxdjFoLTF6IE0yNiwxNWgxdjFoLTF6IE0zMiwxNWgxdjFoLTF6IE0zNiwxNWgxdjJoLTF6IE0wLDE2aDJ2MWgtMnogTTQsMTZoMXYxaC0xeiBNNiwxNmgxdjFoLTF6IE0xMCwxNmgxdjNoLTF6IE0xMSwxNmgxdjFoLTF6IE0xNiwxNmgydjFoLTJ6IE0yMywxNmgxdjFoLTF6IE0yNywxNmgxdjdoLTF6IE0zMCwxNmgxdjFoLTF6IE0zMywxNmgxdjJoLTF6IE0xLDE3aDF2MWgtMXogTTUsMTdoMXYxaC0xeiBNOSwxN2gxdjNoLTF6IE0xNCwxN2gxdjFoLTF6IE0xNiwxN2gxdjFoLTF6IE0yMSwxN2gxdjNoLTF6IE0yNCwxN2gzdjFoLTN6IE0yOCwxN2gxdjFoLTF6IE0zMiwxN2gxdjFoLTF6IE0zLDE4aDF2MmgtMXogTTYsMThoMXYxaC0xeiBNOCwxOGgxdjFoLTF6IE0xMiwxOGgydjFoLTJ6IE0yMywxOGgxdjFoLTF6IE0yNSwxOGgydjFoLTJ6IE0zMCwxOGgydjJoLTJ6IE0zNCwxOGgxdjRoLTF6IE0wLDE5aDN2MWgtM3ogTTEzLDE5aDF2MWgtMXogTTE1LDE5aDF2MWgtMXogTTE3LDE5aDN2MmgtM3ogTTIwLDE5aDF2MWgtMXogTTI0LDE5aDF2MmgtMXogTTI4LDE5aDJ2MWgtMnogTTM1LDE5aDJ2MWgtMnogTTEsMjBoMnYxaC0yeiBNNCwyMGgxdjFoLTF6IE02LDIwaDN2MWgtM3ogTTEwLDIwaDJ2MWgtMnogTTE0LDIwaDF2MmgtMXogTTE2LDIwaDF2MWgtMXogTTIzLDIwaDF2MWgtMXogTTI2LDIwaDF2NmgtMXogTTI5LDIwaDJ2MWgtMnogTTMzLDIwaDF2M2gtMXogTTAsMjFoMnYxaC0yeiBNMywyMWgxdjJoLTF6IE01LDIxaDF2MWgtMXogTTcsMjFoMXYzaC0xeiBNOSwyMWgxdjNoLTF6IE0xMSwyMWgxdjJoLTF6IE0xNSwyMWgxdjFoLTF6IE0xOCwyMWgxdjFoLTF6IE0zMiwyMWgxdjFoLTF6IE00LDIyaDF2MWgtMXogTTYsMjJoMXYxaC0xeiBNMjAsMjJoMnY0aC0yeiBNMjMsMjJoMXYxaC0xeiBNMjUsMjJoMXYxaC0xeiBNMjgsMjJoMnYyaC0yeiBNMzAsMjJoMXYxaC0xeiBNMzUsMjJoMXYxaC0xeiBNMCwyM2gydjFoLTJ6IE0xMiwyM2gxdjRoLTF6IE0xNCwyM2g2djFoLTZ6IE0yMiwyM2gxdjNoLTF6IE0yNCwyM2gxdjFoLTF6IE0zMSwyM2gxdjFoLTF6IE0zNCwyM2gxdjJoLTF6IE0xLDI0aDJ2MWgtMnogTTQsMjRoMXYxaC0xeiBNNiwyNGgxdjFoLTF6IE0xMCwyNGgxdjNoLTF6IE0xMSwyNGgxdjFoLTF6IE0xNCwyNGgxdjFoLTF6IE0xNiwyNGgxdjFoLTF6IE0xOCwyNGgydjFoLTJ6IE0yMywyNGgxdjFoLTF6IE0yNSwyNGgxdjNoLTF6IE0yNywyNGgxdjFoLTF6IE0yOSwyNGgxdjNoLTF6IE0zMCwyNGgxdjFoLTF6IE0zMywyNGgxdjNoLTF6IE0zNSwyNGgydjFoLTJ6IE0wLDI1aDF2MWgtMXogTTIsMjVoMXYyaC0xeiBNNSwyNWgxdjFoLTF6IE03LDI1aDF2NGgtMXogTTgsMjVoMnYxaC0yeiBNMTcsMjVoMnYxaC0yeiBNMjQsMjVoMXYyaC0xeiBNMjgsMjVoMXYyaC0xeiBNMzIsMjVoMXYyaC0xeiBNMzUsMjVoMXYxaC0xeiBNNiwyNmgxdjFoLTF6IE05LDI2aDF2MWgtMXogTTEzLDI2aDF2MmgtMXogTTE5LDI2aDJ2MWgtMnogTTIzLDI2aDF2MmgtMXogTTI3LDI2aDF2MmgtMXogTTMxLDI2aDF2M2gtMXogTTM0LDI2aDF2MmgtMXogTTMsMjdoMXYxaC0xeiBNNSwyN2gxdjJoLTF6IE0xMSwyN2gxdjVoLTF6IE0xNCwyN2gxdjFoLTF6IE0xNiwyN2gxdjFoLTF6IE0xOCwyN2gydjFoLTJ6IE0yMSwyN2gxdjFoLTF6IE0yNiwyN2gxdjFoLTF6IE0zMCwyN2gxdjJoLTF6IE0zNSwyN2gxdjJoLTF6IE0wLDI4aDJ2MWgtMnogTTQsMjhoMXYxaC0xeiBNNiwyOGgxdjFoLTF6IE05LDI4aDF2MWgtMXogTTEyLDI4aDF2MWgtMXogTTE1LDI4aDF2MmgtMXogTTIyLDI4aDF2MmgtMXogTTI4LDI4aDF2NmgtMXogTTI5LDI4aDF2MWgtMXogTTMyLDI4aDF2NWgtMXogTTM2LDI4aDF2MWgtMXogTTgsMjloMXY0aC0xeiBNMTMsMjloMXYxaC0xeiBNMTYsMjloMnYxaC0yeiBNMTksMjloMnYxaC0yeiBNMjMsMjloMXYyaC0xeiBNMjYsMjloMXYyaC0xeiBNMzMsMjloMXYyaC0xeiBNMCwzMGg3djFoLTd6IE0xMCwzMGgxdjFoLTF6IE0xMiwzMGgxdjFoLTF6IE0xNywzMGgydjFoLTJ6IE0yMCwzMGgxdjFoLTF6IE0yNSwzMGgxdjFoLTF6IE0zMCwzMGgxdjFoLTF6IE0zNCwzMGgxdjNoLTF6IE0wLDMxaDF2NmgtMXogTTYsMzFoMXY2aC0xeiBNMTMsMzFoMnYxaC0yeiBNMTgsMzFoMnYxaC0yeiBNMjEsMzFoMXY2aC0xeiBNMjIsMzFoMXYxaC0xeiBNMiwzMmgzdjNoLTN6IE05LDMyaDF2MmgtMXogTTE0LDMyaDF2MmgtMXogTTE2LDMyaDF2MWgtMXogTTE5LDMyaDJ2MmgtMnogTTI0LDMyaDF2MWgtMXogTTI5LDMyaDN2MWgtM3ogTTMzLDMyaDF2MWgtMXogTTM1LDMyaDJ2MWgtMnogTTExLDMzaDN2MWgtM3ogTTE3LDMzaDF2NGgtMXogTTIyLDMzaDF2MWgtMXogTTI3LDMzaDF2MmgtMXogTTMwLDMzaDJ2MmgtMnogTTEzLDM0aDF2MWgtMXogTTE1LDM0aDJ2MmgtMnogTTE4LDM0aDJ2MWgtMnogTTI1LDM0aDJ2MWgtMnogTTMzLDM0aDF2MWgtMXogTTM1LDM0aDF2M2gtMXogTTksMzVoMXYxaC0xeiBNMTEsMzVoMnYxaC0yeiBNMTQsMzVoMXYxaC0xeiBNMjAsMzVoMXYxaC0xeiBNMjQsMzVoMXYxaC0xeiBNMjksMzVoMXYyaC0xeiBNMzQsMzVoMXYyaC0xeiBNMSwzNmg1djFoLTV6IE0xMiwzNmgydjFoLTJ6IE0xNSwzNmgxdjFoLTF6IE0xOSwzNmgxdjFoLTF6IE0yMywzNmgxdjFoLTF6IE0yNywzNmgxdjFoLTF6IE0zMCwzNmgxdjFoLTF6IE0zMiwzNmgxdjFoLTF6IE0zNiwzNmgxdjFoLTF6IiBmaWxsPSIjMDAwMDAwIi8+Cjwvc3ZnPgo="
            };
        }

        public Task<PaymentOnlineConfigurationModel> OnlinePaymentsConfigurationGetAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PaymentOnlineConfigurationModel()
            {
                Presets = new List<decimal>() { 5, 10, 15, 20, 25, 30, 35 },
                AllowCustomValue = true,
                MinimumAmount = 4
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

        public async Task<bool> TokenIsValidAsync(TokenType tokenType, string token, string confirmationCode, CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            //throw new Exception("Test");

            return confirmationCode == "1" ? false : true;
        }

        public Task<RegistrationVerificationMethod> RegistrationVerificationMethodGetAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(RegistrationVerificationMethod.MobilePhone);
        }

        public Task<UserRecoveryMethod> PasswordRecoveryMethodGetAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(UserRecoveryMethod.Mobile);
        }

        public bool AppCurrentProfilePass(int appId)
        {
            return true;
        }

        public Task<UserRecoveryMethodGetResultModel> UserPasswordRecoveryMethodGetAsync(string userNameEmailOrMobile, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new UserRecoveryMethodGetResultModel());
        }

        public async Task<PasswordRecoveryStartResultModelByEmail> UserPasswordRecoveryByEmailStartAsync(string email, CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            throw new Exception("Test");

            var result = new PasswordRecoveryStartResultModelByEmail()
            {
                Token = "123",
                Email = email,
                CodeLength = 5
            };

            return result;
        }

        public async Task<PasswordRecoveryStartResultModelByMobile> UserPasswordRecoveryByMobileStartAsync(string mobilePhone, Gizmo.ConfirmationCodeDeliveryMethod confirmationCodeDeliveryMethod = Gizmo.ConfirmationCodeDeliveryMethod.Undetermined, CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            throw new Exception("Test");

            var result = new PasswordRecoveryStartResultModelByMobile()
            {
                Token = "123",
                MobilePhone = mobilePhone,
                CodeLength = 5,
                DeliveryMethod = ConfirmationCodeDeliveryMethod.FlashCall
            };

            return result;
        }

        public async Task<PasswordRecoveryCompleteResultCode> UserPasswordRecoveryCompleteAsync(string token, string confirmationCode, string newPassword, CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            throw new Exception("Test");

            return new PasswordRecoveryCompleteResultCode();
        }

        public async Task<AccountCreationResultModelByEmail> UserCreateByEmailStartAsync(string email, CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            throw new Exception("Test");

            var result = new AccountCreationResultModelByEmail()
            {
                Token = "123",
                Email = email,
                CodeLength = 5
            };

            return result;
        }

        public async Task<AccountCreationResultModelByMobilePhone> UserCreateByMobileStartAsync(string mobilePhone, ConfirmationCodeDeliveryMethod confirmationCodeDeliveryMethod = ConfirmationCodeDeliveryMethod.Undetermined, CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            //throw new Exception("Test");

            var result = new AccountCreationResultModelByMobilePhone()
            {
                Token = "123",
                MobilePhone = mobilePhone,
                CodeLength = 5,
                DeliveryMethod = confirmationCodeDeliveryMethod == ConfirmationCodeDeliveryMethod.Undetermined ? ConfirmationCodeDeliveryMethod.FlashCall : ConfirmationCodeDeliveryMethod.SMS
            };

            return result;
        }

        public Task<IEnumerable<PopularApplicationModel>> UserPopularApplicationsGetAsync(UserPopularApplicationsFilter filters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Enumerable.Empty<PopularApplicationModel>());
        }

        public Task<IEnumerable<PopularExecutableModel>> UserPopularExecutablesGetAsync(UserPopularExecutablesFilter filters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Enumerable.Empty<PopularExecutableModel>());
        }

        public Task<IEnumerable<PopularProductModel>> UserPopularProductsGetAsync(UserPopularProductsFilter filters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Enumerable.Empty<PopularProductModel>());
        }

        public Task<HostQRCodeResult> HostQRCodeGeneratAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new HostQRCodeResult() { QRCode = "ICAgIDxzdmcgd2lkdGg9IjE1NCIgaGVpZ2h0PSIxNTQiIHZpZXdCb3g9IjAgMCAxNTQgMTU0IiBmaWxsPSJub25lIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPgogICAgICAgICAgICAgICAgICAgICAgICA8cmVjdCB3aWR0aD0iMTU0IiBoZWlnaHQ9IjE1NCIgcng9IjgiIGZpbGw9IiNGQUZBRkEiIC8+CiAgICAgICAgICAgICAgICAgICAgICAgIDxnIGNsaXAtcGF0aD0idXJsKCNjbGlwMF8zODg4XzgzOTY2KSI+CiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8cGF0aCBmaWxsLXJ1bGU9ImV2ZW5vZGQiIGNsaXAtcnVsZT0iZXZlbm9kZCIgZD0iTTkuMjc1OTIgOS40MTAwN0M4LjE3MzQ2IDEwLjYyODIgOC4wMDE1NCAxMy4wMTE5IDguMDExNTYgMjYuOTQ5MkM4LjAxODUgMzUuODE5NyA4LjI3ODMxIDQzLjQ1OTggOC41ODk3OCA0My45MjYzQzEwLjA4MDggNDYuMTU4OSAxMi45MDMyIDQ2LjU0NzUgMjcuNjQzOCA0Ni41NDc1QzQyLjY2ODEgNDYuNTQ3NSA0Mi43NzQ1IDQ2LjUzNTkgNDQuNjU0OCA0NC42NTQ4QzQ2LjUzNTkgNDIuNzc0NSA0Ni41NDc1IDQyLjY2ODEgNDYuNTQ3NSAyNy42NDM4QzQ2LjU0NzUgMTIuOTAzMiA0Ni4xNTg5IDEwLjA4MDggNDMuOTI2MyA4LjU4OTc4QzQzLjQ1OTggOC4yNzgzMSAzNS43NTk2IDguMDE4NSAyNi44MTUgOC4wMTE1NkMxMi4wMjk4IDguMDAwNzcgMTAuNDM2MiA4LjEyNzk4IDkuMjc1OTIgOS40MTAwN1pNNjMuNTA4NCAxMC43NDY5QzYzLjUwODQgMTMuMzgyIDYzLjYxMDkgMTMuNDg0NSA2Ni4wMTQgMTMuMjUyNUM2OC4yMTczIDEzLjA0MDUgNjguNTQ4OSAxMi43MDkgNjguNzYwOSAxMC41MDU2QzY4Ljk5MjkgOC4xMDI1NCA2OC44OTA0IDggNjYuMjU1MyA4QzYzLjYyMDkgOCA2My41MDg0IDguMTEyNTYgNjMuNTA4NCAxMC43NDY5Wk05MS4yNjI2IDEzLjM5NjZWMTguNzkzM0g4NS40ODA0SDc5LjY5ODNWMjEuNDkxNkM3OS42OTgzIDI0LjA2MTIgNzkuNTY5NiAyNC4xODk5IDc3IDI0LjE4OTlDNzQuNDMwNCAyNC4xODk5IDc0LjMwMTcgMjQuMDYxMiA3NC4zMDE3IDIxLjQ5MTZDNzQuMzAxNyAxOC45MjIgNzQuMTcyOSAxOC43OTMzIDcxLjYwMzQgMTguNzkzM0M2OS4wMzM4IDE4Ljc5MzMgNjguOTA1IDE4LjkyMiA2OC45MDUgMjEuNDkxNkM2OC45MDUgMjQuMDYxMiA2OS4wMzM4IDI0LjE4OTkgNzEuNjAzNCAyNC4xODk5SDc0LjMwMTdWMjkuOTcyMVYzNS43NTQySDY4LjkwNUg2My41MDg0VjQxLjE1MDhWNDYuNTQ3NUg2Ni4yMDY3QzY4Ljc3NjMgNDYuNTQ3NSA2OC45MDUgNDYuNjc2MiA2OC45MDUgNDkuMjQ1OEM2OC45MDUgNTEuODE1NCA2OS4wMzM4IDUxLjk0NDEgNzEuNjAzNCA1MS45NDQxQzc0LjE3MjkgNTEuOTQ0MSA3NC4zMDE3IDUyLjA3MjkgNzQuMzAxNyA1NC42NDI1Qzc0LjMwMTcgNTcuMjEyIDc0LjE3MjkgNTcuMzQwOCA3MS42MDM0IDU3LjM0MDhDNjguOTY5IDU3LjM0MDggNjguOTA1IDU3LjQxNCA2OC45MDUgNjAuNDI0NkM2OC45MDUgNjMuNDM1MSA2OC44NDEgNjMuNTA4NCA2Ni4yMDY3IDYzLjUwODRDNjMuNjM3MSA2My41MDg0IDYzLjUwODQgNjMuNjM3MSA2My41MDg0IDY2LjIwNjdWNjguOTA1SDU1LjAyNzlINDYuNTQ3NVY2Ni4yMDY3QzQ2LjU0NzUgNjMuNjM3MSA0Ni40MTg3IDYzLjUwODQgNDMuODQ5MiA2My41MDg0QzQxLjI3OTYgNjMuNTA4NCA0MS4xNTA4IDYzLjYzNzEgNDEuMTUwOCA2Ni4yMDY3QzQxLjE1MDggNjguODQ3MiA0MS4wODM4IDY4LjkwNSAzOC4wMjA4IDY4LjkwNUgzNC44OUwzNS4xMjk3IDY2LjAxNEMzNS4zNTU2IDYzLjI4MDIgMzUuNTI2IDYzLjEwOTggMzguMjU5OCA2Mi44ODM5QzQwLjk5MzYgNjIuNjU3MyA0MS4xNTA4IDYyLjUgNDEuMTUwOCA1OS45OTI5QzQxLjE1MDggNTcuNDc1NyA0MS4yODgxIDU3LjM0MDggNDMuODQ5MiA1Ny4zNDA4QzQ2LjQxODcgNTcuMzQwOCA0Ni41NDc1IDU3LjQ2OTUgNDYuNTQ3NSA2MC4wMzkxQzQ2LjU0NzUgNjIuNjA4NyA0Ni42NzYyIDYyLjczNzQgNDkuMjQ1OCA2Mi43Mzc0QzUxLjgxNTQgNjIuNzM3NCA1MS45NDQxIDYyLjYwODcgNTEuOTQ0MSA2MC4wMzkxQzUxLjk0NDEgNTcuNDY5NSA1Mi4wNzI5IDU3LjM0MDggNTQuNjQyNSA1Ny4zNDA4QzU3LjIxMiA1Ny4zNDA4IDU3LjM0MDggNTcuNDY5NSA1Ny4zNDA4IDYwLjAzOTFDNTcuMzQwOCA2Mi42MDg3IDU3LjQ2OTUgNjIuNzM3NCA2MC4wMzkxIDYyLjczNzRDNjIuNjA4NyA2Mi43Mzc0IDYyLjczNzQgNjIuNjA4NyA2Mi43Mzc0IDYwLjAzOTFDNjIuNzM3NCA1Ny40Njk1IDYyLjYwODcgNTcuMzQwOCA2MC4wMzkxIDU3LjM0MDhINTcuMzQwOFY0OS4yNDU4VjQxLjE1MDhINTQuNjQyNUg1MS45NDQxVjQ2LjU0NzVWNTEuOTQ0MUg0MC43NjU0SDI5LjU4NjZWNTcuNzI2M1Y2My41MDg0SDI2Ljg4ODNDMjQuMzE4NyA2My41MDg0IDI0LjE4OTkgNjMuNjM3MSAyNC4xODk5IDY2LjIwNjdDMjQuMTg5OSA2OC43NzYzIDI0LjA2MTIgNjguOTA1IDIxLjQ5MTYgNjguOTA1QzE4LjkyMiA2OC45MDUgMTguNzkzMyA2OC43NzYzIDE4Ljc5MzMgNjYuMjA2N0MxOC43OTMzIDYzLjYzNzEgMTguNjY0NSA2My41MDg0IDE2LjA5NSA2My41MDg0QzEzLjUyNTQgNjMuNTA4NCAxMy4zOTY2IDYzLjYzNzEgMTMuMzk2NiA2Ni4yMDY3QzEzLjM5NjYgNjguNzc2MyAxMy4yNjc5IDY4LjkwNSAxMC42OTgzIDY4LjkwNUM4LjEyODc1IDY4LjkwNSA4IDY5LjAzMzggOCA3MS42MDM0QzggNzQuMTcyOSA4LjEyODc1IDc0LjMwMTcgMTAuNjk4MyA3NC4zMDE3QzEzLjI2NzkgNzQuMzAxNyAxMy4zOTY2IDc0LjQzMDQgMTMuMzk2NiA3N1Y3OS42OTgzSDMyLjY3MDRINTEuOTQ0MVY3N1Y3NC4zMDE3SDU3LjcyNjNINjMuNTA4NFY4OC4xNzg4VjEwMi4wNTZINjYuMjA2N0M2OC43NzYzIDEwMi4wNTYgNjguOTA1IDEwMi4xODUgNjguOTA1IDEwNC43NTRDNjguOTA1IDEwNy4zMjQgNjguNzc2MyAxMDcuNDUzIDY2LjIwNjcgMTA3LjQ1M0M2My42MzcxIDEwNy40NTMgNjMuNTA4NCAxMDcuNTgxIDYzLjUwODQgMTEwLjE1MVYxMTIuODQ5SDY4LjkwNUg3NC4zMDE3VjExNS41NDdWMTE4LjI0Nkg3OS42OTgzSDg1LjA5NVYxMTUuNTQ3VjExMi44NDlIOTAuODc3MUg5Ni42NTkyVjExNS45MzNWMTE5LjAxN0g5MC44Mjg1SDg0Ljk5NzFMODUuMjM5MSAxMjEuNTIyQzg1LjQ1OTYgMTIzLjgxIDg1LjczMTggMTI0LjA0OSA4OC4zNzE1IDEyNC4yNjdMOTEuMjYyNiAxMjQuNTA3VjEzMi41NTVWMTQwLjYwM0g5My45NjA5Qzk2LjUzMDUgMTQwLjYwMyA5Ni42NTkyIDE0MC43MzIgOTYuNjU5MiAxNDMuMzAyVjE0NkgxMDIuMDU2SDEwNy40NTNWMTQzLjMwMkMxMDcuNDUzIDE0MC43MzIgMTA3LjMyNCAxNDAuNjAzIDEwNC43NTQgMTQwLjYwM0MxMDIuMTg1IDE0MC42MDMgMTAyLjA1NiAxNDAuNDc1IDEwMi4wNTYgMTM3LjkwNVYxMzUuMjA3SDEwNy40NTNIMTEyLjg0OVYxMzcuOTA1QzExMi44NDkgMTQwLjUzOSAxMTIuOTIyIDE0MC42MDMgMTE1LjkzMyAxNDAuNjAzQzExOC45NDQgMTQwLjYwMyAxMTkuMDE3IDE0MC42NjcgMTE5LjAxNyAxNDMuMzAyQzExOS4wMTcgMTQ1Ljg3MSAxMTkuMTQ2IDE0NiAxMjEuNzE1IDE0NkMxMjQuMjg1IDE0NiAxMjQuNDEzIDE0NS44NzEgMTI0LjQxMyAxNDMuMzAyVjE0MC42MDNIMTI5LjgxSDEzNS4yMDdWMTQzLjMwMlYxNDZIMTQwLjYwM0gxNDZWMTM3LjkwNVYxMjkuODFIMTQzLjMwMkgxNDAuNjAzVjEzNS4yMDdWMTQwLjYwM0gxMzcuOTA1QzEzNS4zMzUgMTQwLjYwMyAxMzUuMjA3IDE0MC40NzUgMTM1LjIwNyAxMzcuOTA1QzEzNS4yMDcgMTM1LjMzNSAxMzUuMDc4IDEzNS4yMDcgMTMyLjUwOCAxMzUuMjA3SDEyOS44MVYxMjkuODFWMTI0LjQxM0gxMzIuNTA4QzEzNS4wNzggMTI0LjQxMyAxMzUuMjA3IDEyNC4yODUgMTM1LjIwNyAxMjEuNzE1QzEzNS4yMDcgMTE5LjE0NiAxMzUuMDc4IDExOS4wMTcgMTMyLjUwOCAxMTkuMDE3QzEyOS45MzkgMTE5LjAxNyAxMjkuODEgMTE5LjE0NiAxMjkuODEgMTIxLjcxNUMxMjkuODEgMTI0LjI4NSAxMjkuNjgxIDEyNC40MTMgMTI3LjExMiAxMjQuNDEzQzEyNC40NzcgMTI0LjQxMyAxMjQuNDEzIDEyNC4zNCAxMjQuNDEzIDEyMS4zM0MxMjQuNDEzIDExOC4zMTkgMTI0LjQ3NyAxMTguMjQ2IDEyNy4xMTIgMTE4LjI0NkgxMjkuODFWMTEyLjg0OVYxMDcuNDUzSDEyNy4xMTJIMTI0LjQxM1YxMDIuMDU2Vjk2LjY1OTJIMTIxLjMzQzExOC4zMTkgOTYuNjU5MiAxMTguMjQ2IDk2LjU5NTIgMTE4LjI0NiA5My45NjA5QzExOC4yNDYgOTEuMzkxMyAxMTguMTE3IDkxLjI2MjYgMTE1LjU0NyA5MS4yNjI2QzExMi45MTMgOTEuMjYyNiAxMTIuODQ5IDkxLjE4OTMgMTEyLjg0OSA4OC4xNzg4Vjg1LjA5NUgxMDcuNDUzSDEwMi4wNTZWODIuMzk2NlY3OS42OTgzSDExMC4xNTFIMTE4LjI0NlY3N0MxMTguMjQ2IDc0LjQzMDQgMTE4LjExNyA3NC4zMDE3IDExNS41NDcgNzQuMzAxN0gxMTIuODQ5VjYzLjEyMjlWNTEuOTQ0MUgxMDcuNDUzSDEwMi4wNTZWNDYuNTQ3NVY0MS4xNTA4SDk5LjM1NzVIOTYuNjU5MlYzNS4zNjg3VjI5LjU4NjZIOTkuMzU3NUMxMDEuOTI3IDI5LjU4NjYgMTAyLjA1NiAyOS40NTc4IDEwMi4wNTYgMjYuODg4M0MxMDIuMDU2IDI0LjMxODcgMTAxLjkyNyAyNC4xODk5IDk5LjM1NzUgMjQuMTg5OUM5Ni43ODggMjQuMTg5OSA5Ni42NTkyIDI0LjA2MTIgOTYuNjU5MiAyMS40OTE2Qzk2LjY1OTIgMTguOTIyIDk2Ljc4OCAxOC43OTMzIDk5LjM1NzUgMTguNzkzM0gxMDIuMDU2VjEzLjM5NjZWOEg5Ni42NTkySDkxLjI2MjZWMTMuMzk2NlpNMTA5LjAwOCAxMC4wODU0QzEwNy40MzYgMTIuMDgyOSAxMDcuMzc3IDEyLjgzNzcgMTA3LjYwMyAyNy44ODY2QzEwNy45MDcgNDguMTg4OCAxMDYuMjQ2IDQ2LjU0NzUgMTI2LjQ3NyA0Ni41NDc1QzE0MS4wOTYgNDYuNTQ3NSAxNDMuOTIxIDQ2LjE1NjYgMTQ1LjQxIDQzLjkyNjNDMTQ1LjcyMiA0My40NTk4IDE0NS45ODEgMzUuNzU5NiAxNDUuOTg4IDI2LjgxNUMxNDUuOTk5IDEyLjAyOTggMTQ1Ljg3MiAxMC40MzYyIDE0NC41OSA5LjI3NTkyQzE0My4zNzIgOC4xNzM0NiAxNDAuOTY4IDggMTI2LjkxNCA4SDExMC42NDhMMTA5LjAwOCAxMC4wODU0Wk0zOS41NTI3IDE1LjQyODFDNDAuOTgyIDE3LjI0NDUgNDEuMTUwOCAxOC41MTA0IDQxLjE1MDggMjcuNDEyNUM0MS4xNTA4IDM2Ljk1MyA0MS4wNzIyIDM3LjQ0NDkgMzkuMjU4MiAzOS4yNTgyQzM3LjQ0NDkgNDEuMDcyMiAzNi45NTMgNDEuMTUwOCAyNy40MTI1IDQxLjE1MDhDMTguNTEwNCA0MS4xNTA4IDE3LjI0NDUgNDAuOTgyIDE1LjQyODEgMzkuNTUyN0MxMy40MDUxIDM3Ljk2MTQgMTMuMzk2NiAzNy45MDk4IDEzLjM5NjYgMjcuMDg1NkMxMy4zOTY2IDEyLjY3NTggMTIuNjgyNyAxMy4zOTY2IDI2Ljk1MTUgMTMuMzk2NkMzNy45MjA2IDEzLjM5NjYgMzcuOTU5OSAxMy40MDI4IDM5LjU1MjcgMTUuNDI4MVpNNTcuMzQwOCAxNi4wOTVDNTcuMzQwOCAxOC42NjQ1IDU3LjIxMiAxOC43OTMzIDU0LjY0MjUgMTguNzkzM0M1Mi4wNzI5IDE4Ljc5MzMgNTEuOTQ0MSAxOC45MjIgNTEuOTQ0MSAyMS40OTE2QzUxLjk0NDEgMjQuMDYxMiA1Mi4wNzI5IDI0LjE4OTkgNTQuNjQyNSAyNC4xODk5SDU3LjM0MDhWMjkuNTg2NlYzNC45ODMySDYwLjAzOTFINjIuNzM3NFYyOS41ODY2VjI0LjE4OTlINjAuMDM5MUM1Ny40Njk1IDI0LjE4OTkgNTcuMzQwOCAyNC4wNjEyIDU3LjM0MDggMjEuNDkxNkM1Ny4zNDA4IDE4LjkyMiA1Ny40Njk1IDE4Ljc5MzMgNjAuMDM5MSAxOC43OTMzQzYyLjYwODcgMTguNzkzMyA2Mi43Mzc0IDE4LjY2NDUgNjIuNzM3NCAxNi4wOTVDNjIuNzM3NCAxMy41MjU0IDYyLjYwODcgMTMuMzk2NiA2MC4wMzkxIDEzLjM5NjZDNTcuNDY5NSAxMy4zOTY2IDU3LjM0MDggMTMuNTI1NCA1Ny4zNDA4IDE2LjA5NVpNMTM5LjE5MyAxNC42NzI2QzE0MC40MzQgMTUuNzk1MSAxNDAuNjAzIDE3LjI3MDcgMTQwLjYwMyAyNi45NTE1QzE0MC42MDMgMzcuOTIwNiAxNDAuNTk3IDM3Ljk1OTkgMTM4LjU3MiAzOS41NTI3QzEzNi43NTggNDAuOTc5NyAxMzUuNDg4IDQxLjE1MDggMTI2LjcwOCA0MS4xNTA4QzExMy4wMzYgNDEuMTUwOCAxMTMuMzEzIDQxLjQxNzYgMTEyLjk4NiAyNy45MDUyQzExMi43NTQgMTguMzMxNSAxMTIuODUzIDE3LjQ1NDIgMTE0LjM5IDE1LjQ5OThMMTE2LjA0NSAxMy4zOTY2SDEyNi45MTRDMTM1Ljk0NiAxMy4zOTY2IDEzOC4wMjIgMTMuNjEyNSAxMzkuMTkzIDE0LjY3MjZaTTE4Ljc5MzMgMjYuODg4M1YzNC45ODMySDI2Ljg4ODNIMzQuOTgzMlYyNi44ODgzVjE4Ljc5MzNIMjYuODg4M0gxOC43OTMzVjI2Ljg4ODNaTTExOS4wMTcgMjYuODg4M1YzNC45ODMySDEyNy4xMTJIMTM1LjIwN1YyNi44ODgzVjE4Ljc5MzNIMTI3LjExMkgxMTkuMDE3VjI2Ljg4ODNaTTg1LjA5NSAzNS4zNjg3VjQ2LjU0NzVIODcuNzkzM0M5MC4zNjI5IDQ2LjU0NzUgOTAuNDkxNiA0Ni40MTg3IDkwLjQ5MTYgNDMuODQ5MkM5MC40OTE2IDQxLjIxNDggOTAuNTY0OSA0MS4xNTA4IDkzLjU3NTQgNDEuMTUwOEg5Ni42NTkyVjQ2LjU0NzVWNTEuOTQ0MUg5OS4zNTc1SDEwMi4wNTZWNTcuNzI2M1Y2My41MDg0SDk5LjM1NzVIOTYuNjU5MlY3MS42MDM0Vjc5LjY5ODNIOTAuODc3MUg4NS4wOTVWNzdDODUuMDk1IDc0LjQzMDQgODQuOTY2MiA3NC4zMDE3IDgyLjM5NjYgNzQuMzAxN0g3OS42OTgzVjgyLjM5NjZWOTAuNDkxNkg4Mi4zOTY2Qzg0Ljk2NjIgOTAuNDkxNiA4NS4wOTUgOTAuMzYyOSA4NS4wOTUgODcuNzkzM1Y4NS4wOTVIOTMuNTc1NEgxMDIuMDU2Vjg3Ljc5MzNDMTAyLjA1NiA5MC4zNjI5IDEwMi4xODUgOTAuNDkxNiAxMDQuNzU0IDkwLjQ5MTZDMTA3LjM4OSA5MC40OTE2IDEwNy40NTMgOTAuNTY0OSAxMDcuNDUzIDkzLjU3NTRDMTA3LjQ1MyA5Ni41ODYgMTA3LjM4OSA5Ni42NTkyIDEwNC43NTQgOTYuNjU5MkMxMDIuMTg1IDk2LjY1OTIgMTAyLjA1NiA5Ni41MzA1IDEwMi4wNTYgOTMuOTYwOUMxMDIuMDU2IDkxLjM5MTMgMTAxLjkyNyA5MS4yNjI2IDk5LjM1NzUgOTEuMjYyNkg5Ni42NTkyVjk5LjM1NzVWMTA3LjQ1M0g5My41NzU0QzkwLjU2NDkgMTA3LjQ1MyA5MC40OTE2IDEwNy4zODkgOTAuNDkxNiAxMDQuNzU0VjEwMi4wNTZIODUuMDk1SDc5LjY5ODNWOTkuMzU3NUM3OS42OTgzIDk2Ljc4OCA3OS41Njk2IDk2LjY1OTIgNzcgOTYuNjU5MkM3NC40MzA0IDk2LjY1OTIgNzQuMzAxNyA5Ni43ODggNzQuMzAxNyA5OS4zNTc1Qzc0LjMwMTcgMTAxLjkyNyA3NC4xNzI5IDEwMi4wNTYgNzEuNjAzNCAxMDIuMDU2QzY5LjAzMzggMTAyLjA1NiA2OC45MDUgMTAxLjkyNyA2OC45MDUgOTkuMzU3NUM2OC45MDUgOTYuNzg4IDY5LjAzMzggOTYuNjU5MiA3MS42MDM0IDk2LjY1OTJDNzQuMTcyOSA5Ni42NTkyIDc0LjMwMTcgOTYuNTMwNSA3NC4zMDE3IDkzLjk2MDlDNzQuMzAxNyA5MS4zOTEzIDc0LjE3MjkgOTEuMjYyNiA3MS42MDM0IDkxLjI2MjZDNjguOTY5IDkxLjI2MjYgNjguOTA1IDkxLjE4OTMgNjguOTA1IDg4LjE3ODhDNjguOTA1IDg1LjE2ODIgNjguOTY5IDg1LjA5NSA3MS42MDM0IDg1LjA5NUg3NC4zMDE3Vjc5LjY5ODNWNzQuMzAxN0g3MS42MDM0QzY5LjAzMzggNzQuMzAxNyA2OC45MDUgNzQuMTcyOSA2OC45MDUgNzEuNjAzNEM2OC45MDUgNjkuMDMzOCA2OS4wMzM4IDY4LjkwNSA3MS42MDM0IDY4LjkwNUM3NC4xNzI5IDY4LjkwNSA3NC4zMDE3IDY5LjAzMzggNzQuMzAxNyA3MS42MDM0Qzc0LjMwMTcgNzQuMTcyOSA3NC40MzA0IDc0LjMwMTcgNzcgNzQuMzAxN0M3OS41Njk2IDc0LjMwMTcgNzkuNjk4MyA3NC4xNzI5IDc5LjY5ODMgNzEuNjAzNEM3OS42OTgzIDY5LjAzMzggNzkuODI3MSA2OC45MDUgODIuMzk2NiA2OC45MDVDODUuMDMxIDY4LjkwNSA4NS4wOTUgNjguODMxOCA4NS4wOTUgNjUuODIxMlY2Mi43Mzc0SDkwLjkyNTdIOTYuNzU3MUw5Ni41MTUxIDYwLjIzMThDOTYuMjk0NiA1Ny45NDQ0IDk2LjAyMjQgNTcuNzA1NCA5My4zODI3IDU3LjQ4NzNDOTAuNjQ4OSA1Ny4yNjA2IDkwLjQ5MTYgNTcuMTAzMyA5MC40OTE2IDU0LjU5NjJDOTAuNDkxNiA1Mi4wNzkxIDkwLjM1NDQgNTEuOTQ0MSA4Ny43OTMzIDUxLjk0NDFDODUuMjIzNyA1MS45NDQxIDg1LjA5NSA1MS44MTU0IDg1LjA5NSA0OS4yNDU4Qzg1LjA5NSA0Ni42NzYyIDg0Ljk2NjIgNDYuNTQ3NSA4Mi4zOTY2IDQ2LjU0NzVDNzkuODI3MSA0Ni41NDc1IDc5LjY5ODMgNDYuNjc2MiA3OS42OTgzIDQ5LjI0NThDNzkuNjk4MyA1MS44MTU0IDc5LjU2OTYgNTEuOTQ0MSA3NyA1MS45NDQxQzc0LjQzMDQgNTEuOTQ0MSA3NC4zMDE3IDUxLjgxNTQgNzQuMzAxNyA0OS4yNDU4Qzc0LjMwMTcgNDYuNjc2MiA3NC4xNzI5IDQ2LjU0NzUgNzEuNjAzNCA0Ni41NDc1QzY5LjAzMzggNDYuNTQ3NSA2OC45MDUgNDYuNDE4NyA2OC45MDUgNDMuODQ5MkM2OC45MDUgNDEuMjc5NiA2OS4wMzM4IDQxLjE1MDggNzEuNjAzNCA0MS4xNTA4Qzc0LjE3MjkgNDEuMTUwOCA3NC4zMDE3IDQxLjI3OTYgNzQuMzAxNyA0My44NDkyQzc0LjMwMTcgNDYuNDE4NyA3NC40MzA0IDQ2LjU0NzUgNzcgNDYuNTQ3NUg3OS42OTgzVjM1LjM2ODdWMjQuMTg5OUg4Mi4zOTY2SDg1LjA5NVYzNS4zNjg3Wk04IDU0LjY0MjVWNTcuMzQwOEgxMy4zOTY2SDE4Ljc5MzNWNjAuMDM5MUMxOC43OTMzIDYyLjYwODcgMTguOTIyIDYyLjczNzQgMjEuNDkxNiA2Mi43Mzc0SDI0LjE4OTlWNTcuMzQwOFY1MS45NDQxSDE2LjA5NUg4VjU0LjY0MjVaTTEyOS44MSA1NC42NDI1QzEyOS44MSA1Ny4yMTIgMTI5LjkzOSA1Ny4zNDA4IDEzMi41MDggNTcuMzQwOEMxMzUuMDc4IDU3LjM0MDggMTM1LjIwNyA1Ny40Njk1IDEzNS4yMDcgNjAuMDM5MUMxMzUuMjA3IDYyLjYwODcgMTM1LjMzNSA2Mi43Mzc0IDEzNy45MDUgNjIuNzM3NEMxNDAuNDc1IDYyLjczNzQgMTQwLjYwMyA2Mi42MDg3IDE0MC42MDMgNjAuMDM5MUMxNDAuNjAzIDU3LjQ2OTUgMTQwLjQ3NSA1Ny4zNDA4IDEzNy45MDUgNTcuMzQwOEMxMzUuMzM1IDU3LjM0MDggMTM1LjIwNyA1Ny4yMTIgMTM1LjIwNyA1NC42NDI1QzEzNS4yMDcgNTIuMDcyOSAxMzUuMDc4IDUxLjk0NDEgMTMyLjUwOCA1MS45NDQxQzEyOS45MzkgNTEuOTQ0MSAxMjkuODEgNTIuMDcyOSAxMjkuODEgNTQuNjQyNVpNMTE5LjAxNyA1OS45OTA1QzExOS4wMTcgNjIuNjI0OSAxMTkuMTI5IDYyLjczNzQgMTIxLjc2NCA2Mi43Mzc0QzEyNC4zOTkgNjIuNzM3NCAxMjQuNTAxIDYyLjYzNDkgMTI0LjI2OSA2MC4yMzE4QzEyNC4wNTcgNTguMDI4NSAxMjMuNzI2IDU3LjY5NyAxMjEuNTIyIDU3LjQ4NDlDMTE5LjExOSA1Ny4yNTI5IDExOS4wMTcgNTcuMzU1NCAxMTkuMDE3IDU5Ljk5MDVaTTEyNC40MTMgNjYuMjA2N0MxMjQuNDEzIDY4Ljc3NjMgMTI0LjU0MiA2OC45MDUgMTI3LjExMiA2OC45MDVDMTI5LjY4MSA2OC45MDUgMTI5LjgxIDY4Ljc3NjMgMTI5LjgxIDY2LjIwNjdDMTI5LjgxIDYzLjYzNzEgMTI5LjY4MSA2My41MDg0IDEyNy4xMTIgNjMuNTA4NEMxMjQuNTQyIDYzLjUwODQgMTI0LjQxMyA2My42MzcxIDEyNC40MTMgNjYuMjA2N1pNMTguNzkzMyA3MS42MDM0QzE4Ljc5MzMgNzQuMTcyOSAxOC42NjQ1IDc0LjMwMTcgMTYuMDk1IDc0LjMwMTdDMTMuNTI1NCA3NC4zMDE3IDEzLjM5NjYgNzQuMTcyOSAxMy4zOTY2IDcxLjYwMzRDMTMuMzk2NiA2OS4wMzM4IDEzLjUyNTQgNjguOTA1IDE2LjA5NSA2OC45MDVDMTguNjY0NSA2OC45MDUgMTguNzkzMyA2OS4wMzM4IDE4Ljc5MzMgNzEuNjAzNFpNMjkuNTg2NiA3MS42MDM0QzI5LjU4NjYgNzQuMTcyOSAyOS40NTc4IDc0LjMwMTcgMjYuODg4MyA3NC4zMDE3QzI0LjMxODcgNzQuMzAxNyAyNC4xODk5IDc0LjE3MjkgMjQuMTg5OSA3MS42MDM0QzI0LjE4OTkgNjkuMDMzOCAyNC4zMTg3IDY4LjkwNSAyNi44ODgzIDY4LjkwNUMyOS40NTc4IDY4LjkwNSAyOS41ODY2IDY5LjAzMzggMjkuNTg2NiA3MS42MDM0Wk00Ni41NDc1IDcxLjYwMzRDNDYuNTQ3NSA3NC4xNzI5IDQ2LjQxODcgNzQuMzAxNyA0My44NDkyIDc0LjMwMTdDNDEuMjc5NiA3NC4zMDE3IDQxLjE1MDggNzQuMTcyOSA0MS4xNTA4IDcxLjYwMzRDNDEuMTUwOCA2OS4wMzM4IDQxLjI3OTYgNjguOTA1IDQzLjg0OTIgNjguOTA1QzQ2LjQxODcgNjguOTA1IDQ2LjU0NzUgNjkuMDMzOCA0Ni41NDc1IDcxLjYwMzRaTTEzNS4yMDcgNzEuNjAzNEMxMzUuMjA3IDc0LjE3MjkgMTM1LjA3OCA3NC4zMDE3IDEzMi41MDggNzQuMzAxN0MxMjkuOTM5IDc0LjMwMTcgMTI5LjgxIDc0LjQzMDQgMTI5LjgxIDc3QzEyOS44MSA3OS41Njk2IDEyOS45MzkgNzkuNjk4MyAxMzIuNTA4IDc5LjY5ODNIMTM1LjIwN1Y4NS4wOTVWOTAuNDkxNkgxNDAuNjAzSDE0NlY4Ny43OTMzQzE0NiA4NS4yMjM3IDE0NS44NzEgODUuMDk1IDE0My4zMDIgODUuMDk1QzE0MC43MzIgODUuMDk1IDE0MC42MDMgODQuOTY2MiAxNDAuNjAzIDgyLjM5NjZDMTQwLjYwMyA3OS44MjcxIDE0MC43MzIgNzkuNjk4MyAxNDMuMzAyIDc5LjY5ODNIMTQ2Vjc0LjMwMTdWNjguOTA1SDE0MC42MDNIMTM1LjIwN1Y3MS42MDM0Wk0xMTkuMDE3IDgyLjM5NjZDMTE5LjAxNyA4NC45NjYyIDExOS4xNDYgODUuMDk1IDEyMS43MTUgODUuMDk1QzEyNC4yODUgODUuMDk1IDEyNC40MTMgODUuMjIzNyAxMjQuNDEzIDg3Ljc5MzNDMTI0LjQxMyA5MC4zNjI5IDEyNC41NDIgOTAuNDkxNiAxMjcuMTEyIDkwLjQ5MTZDMTI5LjY4MSA5MC40OTE2IDEyOS44MSA5MC4zNjI5IDEyOS44MSA4Ny43OTMzQzEyOS44MSA4NS4yMjM3IDEyOS42ODEgODUuMDk1IDEyNy4xMTIgODUuMDk1QzEyNC41NDIgODUuMDk1IDEyNC40MTMgODQuOTY2MiAxMjQuNDEzIDgyLjM5NjZDMTI0LjQxMyA3OS44MjcxIDEyNC4yODUgNzkuNjk4MyAxMjEuNzE1IDc5LjY5ODNDMTE5LjE0NiA3OS42OTgzIDExOS4wMTcgNzkuODI3MSAxMTkuMDE3IDgyLjM5NjZaTTggODcuNzQ0N0M4IDkwLjM3OTEgOC4xMTI1NiA5MC40OTE2IDEwLjc0NjkgOTAuNDkxNkMxMy4zODIgOTAuNDkxNiAxMy40ODQ1IDkwLjM4OTEgMTMuMjUyNSA4Ny45ODZDMTMuMDQwNSA4NS43ODI3IDEyLjcwOSA4NS40NTExIDEwLjUwNTYgODUuMjM5MUM4LjEwMjU0IDg1LjAwNzEgOCA4NS4xMDk2IDggODcuNzQ0N1pNMTguNzkzMyA4OC4xNzg4QzE4Ljc5MzMgOTEuMTg5MyAxOC43MjkzIDkxLjI2MjYgMTYuMDk1IDkxLjI2MjZDMTMuNTI1NCA5MS4yNjI2IDEzLjM5NjYgOTEuMzkxMyAxMy4zOTY2IDkzLjk2MDlDMTMuMzk2NiA5Ni41MzA1IDEzLjI2NzkgOTYuNjU5MiAxMC42OTgzIDk2LjY1OTJDOC4xMjg3NSA5Ni42NTkyIDggOTYuNzg4IDggOTkuMzU3NUM4IDEwMS45MjcgOC4xMjg3NSAxMDIuMDU2IDEwLjY5ODMgMTAyLjA1NkMxMy4yNjc5IDEwMi4wNTYgMTMuMzk2NiAxMDEuOTI3IDEzLjM5NjYgOTkuMzU3NVY5Ni42NTkySDE4Ljc5MzNIMjQuMTg5OVY5OS4zNTc1VjEwMi4wNTZIMjkuNTg2NkgzNC45ODMyVjk5LjM1NzVDMzQuOTgzMiA5Ni43ODggMzQuODU0NSA5Ni42NTkyIDMyLjI4NDkgOTYuNjU5MkMyOS43MTUzIDk2LjY1OTIgMjkuNTg2NiA5Ni41MzA1IDI5LjU4NjYgOTMuOTYwOUMyOS41ODY2IDkxLjM5MTMgMjkuNDU3OCA5MS4yNjI2IDI2Ljg4ODMgOTEuMjYyNkMyNC4yNTM5IDkxLjI2MjYgMjQuMTg5OSA5MS4xODkzIDI0LjE4OTkgODguMTc4OEMyNC4xODk5IDg1LjE2ODIgMjQuMTI2IDg1LjA5NSAyMS40OTE2IDg1LjA5NUMxOC44NTczIDg1LjA5NSAxOC43OTMzIDg1LjE2ODIgMTguNzkzMyA4OC4xNzg4Wk00MS4xNTA4IDg3Ljc5MzNWOTAuNDkxNkg0Ni41NDc1SDUxLjk0NDFWOTMuNTc1NFY5Ni42NTkySDQ2LjU0NzVINDEuMTUwOFY5OS4zNTc1VjEwMi4wNTZINDYuNTQ3NUg1MS45NDQxVjExMy4yMzVWMTI0LjQxM0g1NC42NDI1SDU3LjM0MDhWMTE1LjkzM1YxMDcuNDUzSDYwLjAzOTFDNjIuNjA4NyAxMDcuNDUzIDYyLjczNzQgMTA3LjMyNCA2Mi43Mzc0IDEwNC43NTRWMTAyLjA1Nkg1Ny4zNDA4SDUxLjk0NDFWOTkuMzU3NUM1MS45NDQxIDk2Ljc4OCA1Mi4wNzI5IDk2LjY1OTIgNTQuNjQyNSA5Ni42NTkySDU3LjM0MDhWOTAuODc3MVY4NS4wOTVINDkuMjQ1OEg0MS4xNTA4Vjg3Ljc5MzNaTTEyOS44MSA5OS4zNTc1QzEyOS44MSAxMDEuOTI3IDEyOS45MzkgMTAyLjA1NiAxMzIuNTA4IDEwMi4wNTZIMTM1LjIwN1YxMTAuMTUxVjExOC4yNDZIMTQwLjYwM0gxNDZWMTEyLjg0OVYxMDcuNDUzSDE0My4zMDJDMTQwLjczMiAxMDcuNDUzIDE0MC42MDMgMTA3LjMyNCAxNDAuNjAzIDEwNC43NTRDMTQwLjYwMyAxMDIuMTg1IDE0MC43MzIgMTAyLjA1NiAxNDMuMzAyIDEwMi4wNTZDMTQ1Ljg3MSAxMDIuMDU2IDE0NiAxMDEuOTI3IDE0NiA5OS4zNTc1QzE0NiA5Ni43ODggMTQ1Ljg3MSA5Ni42NTkyIDE0My4zMDIgOTYuNjU5MkMxNDAuNzMyIDk2LjY1OTIgMTQwLjYwMyA5Ni43ODggMTQwLjYwMyA5OS4zNTc1QzE0MC42MDMgMTAxLjkyNyAxNDAuNDc1IDEwMi4wNTYgMTM3LjkwNSAxMDIuMDU2QzEzNS4zMzUgMTAyLjA1NiAxMzUuMjA3IDEwMS45MjcgMTM1LjIwNyA5OS4zNTc1QzEzNS4yMDcgOTYuNzg4IDEzNS4wNzggOTYuNjU5MiAxMzIuNTA4IDk2LjY1OTJDMTI5LjkzOSA5Ni42NTkyIDEyOS44MSA5Ni43ODggMTI5LjgxIDk5LjM1NzVaTTExOC44NSAxMTAuMzQ0TDExOC42MzEgMTE4LjYzMUwxMTAuMzQ0IDExOC44NUwxMDIuMDU2IDExOS4wNjlWMTEwLjU2M1YxMDIuMDU2SDExMC41NjNIMTE5LjA2OUwxMTguODUgMTEwLjM0NFpNMTAuNjk4MyAxMDguMzc0QzkuODUwMjggMTA4Ljg1NSA4LjkwMTI0IDEwOS42MjUgOC41ODk3OCAxMTAuMDg1QzguMjc4MzEgMTEwLjU0NiA4LjAxODUgMTE4LjI0IDguMDExNTYgMTI3LjE4NUM4LjAwMDc3IDE0MS45NyA4LjEyNzk4IDE0My41NjQgOS40MTAwNyAxNDQuNzI0QzEwLjYyODIgMTQ1LjgyNyAxMy4wMTE5IDE0NS45OTggMjYuOTQ5MiAxNDUuOTg4QzM1LjgxOTcgMTQ1Ljk4MSA0My40NTk4IDE0NS43MjIgNDMuOTI2MyAxNDUuNDFDNDYuMTU4OSAxNDMuOTE5IDQ2LjU0NzUgMTQxLjA5NyA0Ni41NDc1IDEyNi4zNTZDNDYuNTQ3NSAxMDYuMDE2IDQ3Ljk4ODQgMTA3LjQ0NSAyNy41MDEyIDEwNy40NzZDMTcuNTg2OCAxMDcuNDkxIDExLjY5OTggMTA3LjgwNiAxMC42OTgzIDEwOC4zNzRaTTg1LjA5NSAxMTAuMTUxQzg1LjA5NSAxMTIuNzIgODQuOTY2MiAxMTIuODQ5IDgyLjM5NjYgMTEyLjg0OUM3OS44MjcxIDExMi44NDkgNzkuNjk4MyAxMTIuNzIgNzkuNjk4MyAxMTAuMTUxQzc5LjY5ODMgMTA3LjU4MSA3OS44MjcxIDEwNy40NTMgODIuMzk2NiAxMDcuNDUzQzg0Ljk2NjIgMTA3LjQ1MyA4NS4wOTUgMTA3LjU4MSA4NS4wOTUgMTEwLjE1MVpNMTA3LjQ1MyAxMTAuMTUxQzEwNy40NTMgMTEyLjcyIDEwNy41ODEgMTEyLjg0OSAxMTAuMTUxIDExMi44NDlDMTEyLjcyIDExMi44NDkgMTEyLjg0OSAxMTIuNzIgMTEyLjg0OSAxMTAuMTUxQzExMi44NDkgMTA3LjU4MSAxMTIuNzIgMTA3LjQ1MyAxMTAuMTUxIDEwNy40NTNDMTA3LjU4MSAxMDcuNDUzIDEwNy40NTMgMTA3LjU4MSAxMDcuNDUzIDExMC4xNTFaTTM5LjM5MDggMTE0LjYwOUM0MC41NDE4IDExNS43NiA0MC44MDYyIDExNy42NDMgNDEuMDE0NCAxMjYuMTlDNDEuMjQ1NyAxMzUuNjY3IDQxLjE0NTQgMTM2LjU0NyAzOS42MDk3IDEzOC41TDM3Ljk1NTMgMTQwLjYwM0gyNi44NzU5QzEyLjc2OTEgMTQwLjYwMyAxMy4zOTY2IDE0MS4yNTUgMTMuMzk2NiAxMjYuNjExVjExNi4wNDVMMTUuNDk5OCAxMTQuMzlDMTcuNDUyNiAxMTIuODU1IDE4LjMzMyAxMTIuNzU0IDI3LjgwOTYgMTEyLjk4NkMzNi4zNTcxIDExMy4xOTQgMzguMjM5NyAxMTMuNDU4IDM5LjM5MDggMTE0LjYwOVpNMTguNzkzMyAxMjcuMTEyVjEzNS4yMDdIMjYuODg4M0gzNC45ODMyVjEyNy4xMTJWMTE5LjAxN0gyNi44ODgzSDE4Ljc5MzNWMTI3LjExMlpNNjMuNTA4NCAxMjEuNzE1QzYzLjUwODQgMTI0LjI4NSA2My42MzcxIDEyNC40MTMgNjYuMjA2NyAxMjQuNDEzQzY4Ljc3NjMgMTI0LjQxMyA2OC45MDUgMTI0LjU0MiA2OC45MDUgMTI3LjExMkM2OC45MDUgMTI5LjY4MSA2OC43NzYzIDEyOS44MSA2Ni4yMDY3IDEyOS44MUM2My42MzcxIDEyOS44MSA2My41MDg0IDEyOS45MzkgNjMuNTA4NCAxMzIuNTA4QzYzLjUwODQgMTM1LjA3OCA2My42MzcxIDEzNS4yMDcgNjYuMjA2NyAxMzUuMjA3QzY4Ljc3NjMgMTM1LjIwNyA2OC45MDUgMTM1LjMzNSA2OC45MDUgMTM3LjkwNUM2OC45MDUgMTQwLjQ3NSA2OC43NzYzIDE0MC42MDMgNjYuMjA2NyAxNDAuNjAzQzYzLjYzNzEgMTQwLjYwMyA2My41MDg0IDE0MC43MzIgNjMuNTA4NCAxNDMuMzAyQzYzLjUwODQgMTQ1Ljg3MSA2My42MzcxIDE0NiA2Ni4yMDY3IDE0NkM2OC43NzYzIDE0NiA2OC45MDUgMTQ1Ljg3MSA2OC45MDUgMTQzLjMwMkM2OC45MDUgMTQwLjczMiA2OS4wMzM4IDE0MC42MDMgNzEuNjAzNCAxNDAuNjAzQzc0LjE3MjkgMTQwLjYwMyA3NC4zMDE3IDE0MC43MzIgNzQuMzAxNyAxNDMuMzAyQzc0LjMwMTcgMTQ1Ljg3MSA3NC40MzA0IDE0NiA3NyAxNDZDNzkuNTY5NiAxNDYgNzkuNjk4MyAxNDUuODcxIDc5LjY5ODMgMTQzLjMwMkM3OS42OTgzIDE0MC43MzIgNzkuNTY5NiAxNDAuNjAzIDc3IDE0MC42MDNDNzQuNDMwNCAxNDAuNjAzIDc0LjMwMTcgMTQwLjQ3NSA3NC4zMDE3IDEzNy45MDVDNzQuMzAxNyAxMzUuMzM1IDc0LjE3MjkgMTM1LjIwNyA3MS42MDM0IDEzNS4yMDdDNjkuMDMzOCAxMzUuMjA3IDY4LjkwNSAxMzUuMDc4IDY4LjkwNSAxMzIuNTA4QzY4LjkwNSAxMjkuOTM5IDY5LjAzMzggMTI5LjgxIDcxLjYwMzQgMTI5LjgxSDc0LjMwMTdWMTI0LjQxM1YxMTkuMDE3SDY4LjkwNUg2My41MDg0VjEyMS43MTVaTTExOS4wMTcgMTI3LjExMkMxMTkuMDE3IDEyOS42ODEgMTE5LjE0NiAxMjkuODEgMTIxLjcxNSAxMjkuODFDMTI0LjI4NSAxMjkuODEgMTI0LjQxMyAxMjkuOTM5IDEyNC40MTMgMTMyLjUwOEMxMjQuNDEzIDEzNS4xNDMgMTI0LjM0IDEzNS4yMDcgMTIxLjMzIDEzNS4yMDdDMTE4LjMxOSAxMzUuMjA3IDExOC4yNDYgMTM1LjE0MyAxMTguMjQ2IDEzMi41MDhWMTI5LjgxSDExMC4xNTFIMTAyLjA1NlYxMjcuMTEyVjEyNC40MTNIMTEwLjUzNkgxMTkuMDE3VjEyNy4xMTJaTTUxLjk0NDEgMTM3LjkwNVYxNDZINTQuNjQyNUM1Ny4yMTIgMTQ2IDU3LjM0MDggMTQ1Ljg3MSA1Ny4zNDA4IDE0My4zMDJDNTcuMzQwOCAxNDAuNzMyIDU3LjQ2OTUgMTQwLjYwMyA2MC4wMzkxIDE0MC42MDNDNjIuNjA4NyAxNDAuNjAzIDYyLjczNzQgMTQwLjQ3NSA2Mi43Mzc0IDEzNy45MDVDNjIuNzM3NCAxMzUuMzM1IDYyLjYwODcgMTM1LjIwNyA2MC4wMzkxIDEzNS4yMDdDNTcuNDY5NSAxMzUuMjA3IDU3LjM0MDggMTM1LjA3OCA1Ny4zNDA4IDEzMi41MDhDNTcuMzQwOCAxMjkuOTM5IDU3LjIxMiAxMjkuODEgNTQuNjQyNSAxMjkuODFINTEuOTQ0MVYxMzcuOTA1Wk03OS42OTgzIDEzMi41MDhDNzkuNjk4MyAxMzUuMDc4IDc5LjgyNzEgMTM1LjIwNyA4Mi4zOTY2IDEzNS4yMDdDODQuOTY2MiAxMzUuMjA3IDg1LjA5NSAxMzUuMDc4IDg1LjA5NSAxMzIuNTA4Qzg1LjA5NSAxMjkuOTM5IDg0Ljk2NjIgMTI5LjgxIDgyLjM5NjYgMTI5LjgxQzc5LjgyNzEgMTI5LjgxIDc5LjY5ODMgMTI5LjkzOSA3OS42OTgzIDEzMi41MDhaIiBmaWxsPSIjMkIyRDMzIiAvPgogICAgICAgICAgICAgICAgICAgICAgICA8L2c+CiAgICAgICAgICAgICAgICAgICAgICAgIDxkZWZzPgogICAgICAgICAgICAgICAgICAgICAgICAgICAgPGNsaXBQYXRoIGlkPSJjbGlwMF8zODg4XzgzOTY2Ij4KICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8cmVjdCB4PSI4IiB5PSI4IiB3aWR0aD0iMTM4IiBoZWlnaHQ9IjEzOCIgcng9IjgiIGZpbGw9IndoaXRlIiAvPgogICAgICAgICAgICAgICAgICAgICAgICAgICAgPC9jbGlwUGF0aD4KICAgICAgICAgICAgICAgICAgICAgICAgPC9kZWZzPgogICAgICAgICAgICAgICAgICAgIDwvc3ZnPg==" });
        }
    }
}
