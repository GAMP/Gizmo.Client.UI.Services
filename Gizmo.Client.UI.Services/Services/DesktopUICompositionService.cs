using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// UI Composition service used in desktop host.
    /// </summary>
    public sealed class DesktopUICompositionService : UICompositionServiceBase
    {
        #region CONSTRUCTOR
        public DesktopUICompositionService(IOptionsMonitor<UICompositionOptions> optionsMonitor,
            UICompositionInMemoryConfiurationSource uiCompositionConfiurationSource,
            UIOptionsInMemoryConfigurationSource uiOptionsConfigurationSource,
            IServiceProvider serviceProvider,
            ILogger<DesktopUICompositionService> logger) : base(optionsMonitor, logger, serviceProvider)
        {
            _uiCompositionConfiurationSource = uiCompositionConfiurationSource;
            _uiOptionsConfigurationSource = uiOptionsConfigurationSource;
        }
        #endregion        

        #region FIELDS

        private string _basePath = Environment.CurrentDirectory;
        private readonly UICompositionInMemoryConfiurationSource _uiCompositionConfiurationSource;
        private readonly UIOptionsInMemoryConfigurationSource _uiOptionsConfigurationSource;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets base path.
        /// </summary>
        public string BasePath
        {
            get { return _basePath; }
        } 

        #endregion

        #region OVERRIDES

        public Task SetConfigurationSourceAsync(string fullPath)
        {
            if(string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentNullException(nameof(fullPath));

            if (!Path.IsPathRooted(fullPath))
                throw new ArgumentOutOfRangeException(nameof(fullPath));

            _basePath = Path.GetDirectoryName(fullPath) ?? Environment.CurrentDirectory;

            using(var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _uiCompositionConfiurationSource.Load(fileStream);
            }

            return Task.CompletedTask;
        }

        public Task SetOptionsConfigurationSourceAsync(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentNullException(nameof(fullPath));

            if (!Path.IsPathRooted(fullPath))
                throw new ArgumentOutOfRangeException(nameof(fullPath));

            _basePath = Path.GetDirectoryName(fullPath) ?? Environment.CurrentDirectory;

            using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _uiOptionsConfigurationSource.Load(fileStream);
            }

            return Task.CompletedTask;
        }

        protected async override void OnCompositionSettingsChanged(UICompositionOptions uICompositionSettings, string setting)
        {
            base.OnCompositionSettingsChanged(uICompositionSettings, setting);

            await InitializeAsync(default);
        }

        protected override Task<Assembly> LoadAssemblyAsync(string assemblyName, CancellationToken ct = default)
        {
            string fullAssemblyPath = !Path.IsPathRooted(assemblyName) ? Path.Combine(_basePath, assemblyName) : assemblyName;

            var assembly = Assembly.LoadFrom(fullAssemblyPath);           

            return Task.FromResult(assembly);
        }

        protected override bool IsAllowedModule(UIPageModuleMetadata metadata)
        {
            return base.IsAllowedModule(metadata);
        }

        #endregion
    }
}
