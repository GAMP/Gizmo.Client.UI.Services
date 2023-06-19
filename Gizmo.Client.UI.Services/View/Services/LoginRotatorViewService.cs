using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.LoginRoute)]
    public sealed class LoginRotatorViewService : ViewStateServiceBase<LoginRotatorViewState>
    {
        #region CONSTRUCTOR
        public LoginRotatorViewService(LoginRotatorViewState viewState,
            ILogger<LoginRotatorViewService> logger,
            IServiceProvider serviceProvider,
            IOptions<LoginRotatorOptions> loginRotatorOptions) : base(viewState, logger, serviceProvider)
        {
            _loginRotatorOptions = loginRotatorOptions;
        }
        #endregion

        #region STATIC FIELDS

        private static readonly string[] IMAGE_EXTENSIONS = new string[]
        {
            ".JPG",
            ".PNG",
            ".BMP"
        };

        private static readonly string[] VIDEO_EXTENSIONS = new string[]
        {
            ".M4V",
            ".MP4",
            ".WMV",
            ".AVI",
        };

        private static readonly string[] ALL_EXTENSIONS = IMAGE_EXTENSIONS.Union(VIDEO_EXTENSIONS).ToArray();

        #endregion

        #region FIELDS
        private readonly IOptions<LoginRotatorOptions> _loginRotatorOptions;
        private Timer? _rotatateTimer;
        private List<LoginRotatorItemViewState> _items = new();
        private int _index = 0;
        #endregion

        #region FUNCTIONS

        private int GetRotateMills()
        {
            if (_loginRotatorOptions.Value.RotateEvery <= 0)
                return 6000; //just in case check that correct value is set and provide default value if not

            return (int)TimeSpan.FromSeconds(_loginRotatorOptions.Value.RotateEvery).TotalMilliseconds;
        }

        #endregion

        private void OnTimerCallback(object? state)
        {
            _index += 1;

            if (_index == _items.Count)
                _index = 0;

            ViewState.CurrentItem = _items[_index];

            if (ViewState.CurrentItem?.IsVideo == true)
            {
                _rotatateTimer?.Dispose();
            }

            DebounceViewStateChanged();
        }

        public Task PlayNext()
        {
            _index += 1;

            if (_index == _items.Count)
                _index = 0;

            ViewState.CurrentItem = _items[_index];

            if (ViewState.CurrentItem?.IsVideo == false)
            {
                _rotatateTimer?.Dispose();
                _rotatateTimer = new Timer(OnTimerCallback, null, GetRotateMills(), GetRotateMills());
            }

            DebounceViewStateChanged();

            return Task.CompletedTask;
        }

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            try
            {
                if (_loginRotatorOptions.Value.IsEnabled)
                {
                    var ROTATE_FOLDER = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot\\rotator"); //TODO: AAA
                    ROTATE_FOLDER = _loginRotatorOptions.Value.Path;
                    if (!string.IsNullOrEmpty(ROTATE_FOLDER))
                    {
                        var MEDIA_FILES = Directory.GetFiles(ROTATE_FOLDER, "*.*", SearchOption.AllDirectories)
                                                   .Where(FILE => ALL_EXTENSIONS.Any(EXTESNSION => FILE.EndsWith(EXTESNSION, StringComparison.InvariantCultureIgnoreCase)));

                        _items = MEDIA_FILES.Select(FILE => new LoginRotatorItemViewState()
                        {
                            MediaPath = FILE.Replace(ROTATE_FOLDER, "_content/Gizmo.Client.UI/img/rotator/"),
                            IsVideo = VIDEO_EXTENSIONS.Any(EXTENSION => FILE.EndsWith(EXTENSION, StringComparison.InvariantCultureIgnoreCase))
                        }).ToList();

                        ViewState.IsEnabled = _loginRotatorOptions.Value.IsEnabled;
                        ViewState.CurrentItem = _items[_index];

                        if (ViewState.CurrentItem?.IsVideo == false)
                        {
                            _rotatateTimer?.Dispose();
                            _rotatateTimer = new Timer(OnTimerCallback, null, GetRotateMills(), GetRotateMills());
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Task.CompletedTask;
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _rotatateTimer?.Dispose();
            base.OnDisposing(isDisposing);
        }

        #endregion
    }
}
