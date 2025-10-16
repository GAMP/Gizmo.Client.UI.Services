#nullable enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Assembly resource localization service.
    /// </summary>
    [Register(typeof(IAssemblyResourcesLocalizationService))]
    public sealed class AssemblyResourcesLocalizationService : IAssemblyResourcesLocalizationService
    {
        #region CONSTRUCTOR
        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="stringLocalizerFactory">String localizer factory.</param>
        public AssemblyResourcesLocalizationService(IStringLocalizerFactory stringLocalizerFactory)
        {
            _stringLocalizerFactory = stringLocalizerFactory;
        }
        #endregion

        #region CONSTATNTS
        /// <summary>
        /// Default name for the resources. (Resources.resx)
        /// </summary>
        private const string DEFAULT_RESOURCE_NAME = "Resources";
        /// <summary>
        /// Executing assembly.
        /// </summary>
        private readonly Assembly EXECUTING_ASSEMBLY = Assembly.GetExecutingAssembly();
        #endregion

        #region FIELDS

        /// <summary>
        /// Localizer cache. This will allow faster access to the target localizers.
        /// </summary>
        private readonly ConcurrentDictionary<Assembly, IStringLocalizer> _localizerCache = new();

        /// <summary>
        /// String localizer factory.
        /// </summary>
        private readonly IStringLocalizerFactory _stringLocalizerFactory;

        #endregion

        #region PUBLIC FUNCTIONS

        public string GetLocalizedStringValue(Type type, ExtendedDescriptionAttribute? descriptionAttribute)
        {
            if (descriptionAttribute == null)
                return string.Empty;

            var localizedString = GetLocalizedString(type, descriptionAttribute);

            return localizedString.ResourceNotFound ? descriptionAttribute.Description ?? string.Empty : localizedString.Value;
        }

        public string GetLocalizedStringValue(Type type, NameAttribute? nameAttribute)
        {
            if (nameAttribute == null)
                return string.Empty;

            var localizedString = GetLocalizedString(type, nameAttribute);

            return localizedString.ResourceNotFound ? nameAttribute.Name ?? string.Empty : localizedString.Value;
        }

        public string GetLocalizedStringValue(Enum enumValue)
        {
            var nameAttribute = enumValue.GetAttribute<NameAttribute>();
            if (nameAttribute == null)
                return string.Empty;

            return GetLocalizedString(enumValue.GetType(), nameAttribute);
        }


        /// <inheritdoc/>
        public string GetLocalizedStringValue(Type type, string resourceKey)
        {
            var localizer = GetLocalizer(type.Assembly);
            return localizer.GetString(resourceKey);
        }

        #endregion

        #region PRIVATE FUNCTIONS

        private LocalizedString GetLocalizedString(Type type, LocalizedAttribute localizedAttribute)
        {
            var localizer = GetLocalizer(type.Assembly);
            return localizer.GetString(localizedAttribute.ResourceKey);
        }

        /// <summary>
        /// Gets default resource string localizer for specified assembly.
        /// </summary>
        /// <param name="assembly">Assembly instance.</param>
        /// <returns>Default resource string localizer.</returns>
        /// <exception cref="ArgumentException">thrown in case assembly name cannot be extracted from specified assembly.</exception>
        private IStringLocalizer GetLocalizer(Assembly assembly)
        {
            string? assemblyName = assembly.FullName;
            if (string.IsNullOrEmpty(assemblyName))
                throw new ArgumentException();

            return _localizerCache.GetOrAdd(assembly, (a) =>
            {
                return _stringLocalizerFactory.Create(DEFAULT_RESOURCE_NAME, assemblyName);
            });
        }

        #endregion
    }
}
