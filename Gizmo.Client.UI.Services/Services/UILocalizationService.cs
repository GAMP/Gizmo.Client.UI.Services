using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using Gizmo.UI.Services;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Client localization service.
    /// </summary>
    public class UILocalizationService : LocalizationServiceBase
    {
        #region CONSTRUCTOR
        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="localizer">Localizer.</param>
        public UILocalizationService(ILogger<UILocalizationService> logger, IStringLocalizer localizer) : base(logger, localizer)
        {
            var prop = localizer.GetType().GetField("_localizer", BindingFlags.NonPublic | BindingFlags.Instance);
            _resourceManagerStringLocalizer = (ResourceManagerStringLocalizer)prop.GetValue(localizer);
            prop = _resourceManagerStringLocalizer.GetType().GetField("_resourceManager", BindingFlags.NonPublic | BindingFlags.Instance);
            _resourceManager = (ResourceManager)prop.GetValue(_resourceManagerStringLocalizer);
        }
        #endregion

        private static readonly bool _isWebBrowser = RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser"));
        private readonly ResourceManagerStringLocalizer? _resourceManagerStringLocalizer;
        private readonly ResourceManager? _resourceManager;

        public override IEnumerable<CultureInfo> SupportedCultures
        {
            get
            {
                if(_resourceManager==null || _isWebBrowser)
                    return base.SupportedCultures;

                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
                var supportedCultures =  cultures.Where(culture =>
                {
                    try
                    {
                        ResourceSet? resourceSet = _resourceManager?.GetResourceSet(culture, true, false);
                        if (resourceSet != null)
                            return true;
                        return false;
                    }
                    catch (CultureNotFoundException ex)
                    {
                        Logger.LogError(ex, "Could not obtain resource set for {culture}.", culture);
                        return false;
                    }
                }).ToList();

                //replace invariant culture with default english
                if(supportedCultures.Contains(CultureInfo.InvariantCulture))
                {
                    supportedCultures.Remove(CultureInfo.InvariantCulture);
                    supportedCultures.Insert(0,CultureInfo.GetCultureInfo("en-us"));
                }

                return supportedCultures;
            }
        }
    }
}
