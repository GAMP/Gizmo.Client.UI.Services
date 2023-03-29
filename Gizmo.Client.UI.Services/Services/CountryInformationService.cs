using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Generic helper service used for obtaining country releated information.
    /// </summary>
    public sealed class CountryInformationService
    {
        public const string HTTP_CLIENT_NAME_REST_COUNTRIES = "HttpClientRestCountries";
        public const string HTTP_CLIENT_NAME_GEO_PLUGIN = "HttpClientGeoPlugin";

        private const string COUNTRIES_RESOURCE_NAME = "Gizmo.Client.UI.Services.Resources.countries.json";
        private static readonly Assembly EXECUTING_ASSEMBLY = Assembly.GetExecutingAssembly();

        public CountryInformationService(IHttpClientFactory httpClientFactory, ILogger<CountryInformationService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        private ILogger Logger => _logger;

        /// <summary>
        /// Gets countries information.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Countries information.</returns>
        /// <remarks>
        /// This function will not throw any exceptions and return empty list in case of error.
        /// </remarks>
        public async Task<IEnumerable<CountryInfo>> GetCountryInfoAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<Country>? countries = null;

            try
            {
                using (var httpClient = _httpClientFactory.CreateClient(HTTP_CLIENT_NAME_REST_COUNTRIES))
                {
                    countries = await httpClient.GetFromJsonAsync<IEnumerable<Country>>("all", cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to obtain countries information form the api.");
            }

            if (countries == null)
            {
                try
                {
                    using (Stream? stream = EXECUTING_ASSEMBLY.GetManifestResourceStream(COUNTRIES_RESOURCE_NAME))
                    {
                        if (stream == null)
                            throw new ArgumentException("Countries resource stream could not be opened.");
                        countries = await JsonSerializer.DeserializeAsync<IEnumerable<Country>>(stream, cancellationToken: cancellationToken);
                    }

                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to obtain countries information form local embeded file cache.");
                }
            }

            return Parse(countries ?? Enumerable.Empty<Country>());
        }

        /// <summary>
        /// Gets country information.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result.</returns>
        /// <remarks>
        /// <see cref="GetCountryCodeResult.IsSuccess"/> needs to be checked before using the <see cref="GetCountryCodeResult.CountryCode"/> can be used.<br></br>
        /// This function will not throw any exception and will have <see cref="GetCountryCodeResult.IsSuccess"/> set to false in case of error.
        /// </remarks>
        public async Task<GetCountryCodeResult> GetCountryCodeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient(HTTP_CLIENT_NAME_GEO_PLUGIN))
                {
                    var response = await httpClient.GetAsync($"json.gp", cancellationToken).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var geoResponse = await response.Content.ReadFromJsonAsync<GeoPluginResponse>(cancellationToken: cancellationToken);
                        if (geoResponse != null)
                        {
                            if (geoResponse.Status == 200 || geoResponse.Status == 206)
                                return new GetCountryCodeResult() { TwoLetterCountryCode = geoResponse.CountryCode, IsSuccess = true };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Country detection failed.");
            }
            return GetCountryCodeResult.Failure;
        }

        /// <summary>
        /// Gets currently detected country info.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Country info, null in case of error or failed country detection.</returns>
        public async Task<CountryInfo?> GetCurrentCountryInfoAsync(CancellationToken cancellationToken = default)
        { 
            var countryCodeResult = await GetCountryCodeAsync(cancellationToken).ConfigureAwait(false);
            if(countryCodeResult.IsSuccess)
            {
                var countries = await GetCountryInfoAsync(cancellationToken).ConfigureAwait(false);
                return countries.Where(country=> country.TwoLetterCountryCode == countryCodeResult.TwoLetterCountryCode).FirstOrDefault();
            }

            return default;
        }

        private static IEnumerable<CountryInfo> Parse(IEnumerable<Country> countries)
        {
            return countries.Select(country => new CountryInfo()
            {
                Name = country.Name.Common,
                NativeName = country.Name.NativeName.Values.FirstOrDefault()?.Common ?? string.Empty,
                FlagSvg = country.Flags.Svg,
                FlagPng = country.Flags.Png,
                CallingCode = string.Format("{0}{1}", country.Idd.Root, country.Idd.Suffixes.FirstOrDefault()),
                TwoLetterCountryCode = country.CCA2,
            }).ToList();
        }
    }

    /// <summary>
    /// Get country code result.
    /// </summary>
    public sealed class GetCountryCodeResult
    {
        public static readonly GetCountryCodeResult Failure = new GetCountryCodeResult() { IsSuccess = false };

        /// <summary>
        /// Indicates success.
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// Two letter country number (cca2) <a href="https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2"/>.
        /// </summary>
        public string TwoLetterCountryCode { get; init; } = string.Empty;
    }

    /// <summary>
    /// Provides generic country information.
    /// </summary>
    public sealed class CountryInfo
    {
        /// <summary>
        /// Gets country name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets country native name.
        /// </summary>
        public string NativeName { get; init; } = string.Empty;

        /// <summary>
        /// Gets svg flag url.
        /// </summary>
        public string FlagSvg { get; init; } = string.Empty;

        /// <summary>
        /// Gets png flag url.
        /// </summary>
        public string FlagPng { get; init; } = string.Empty;

        /// <summary>
        /// Gets full calling code.
        /// </summary>
        public string CallingCode { get; init; } = string.Empty;

        /// <summary>
        /// Two letter country number (cca2) <a href="https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2"/>.
        /// </summary>
        public string TwoLetterCountryCode { get;init; } = string.Empty;
    }


    #region API MODELS

    class GeoPluginResponse
    {

        [JsonPropertyName("geoplugin_status")]
        public int Status { get; init; }

        [JsonPropertyName("geoplugin_countryCode")]
        public string CountryCode { get; init; } = string.Empty;

        [JsonPropertyName("geoplugin_countryName")]
        public string CountryName { get; init; } = string.Empty;
    }

    class Country
    {
        [JsonPropertyName("name")]
        public CountryName Name
        {
            get;
            init;
        } = CountryName.Empty;

        [JsonPropertyName("cca2")]
        public string CCA2
        {
            get;init;
        } = string.Empty;

        [JsonPropertyName("flags")]
        public CountryFlag Flags
        {
            get; init;
        } = CountryFlag.Empty;

        [JsonPropertyName("idd")]
        public CountryIdd Idd
        {
            get;
            init;
        } = CountryIdd.Empty;
    }

    class CountryFlag
    {
        public static readonly CountryFlag Empty = new CountryFlag();

        [JsonPropertyName("png")]
        public string Png
        {
            get; init;
        } = string.Empty;

        [JsonPropertyName("svg")]
        public string Svg
        {
            get; init;
        } = string.Empty;

        [JsonPropertyName("alt")]
        public string Alt
        {
            get; init;
        } = string.Empty;
    }

    class CountryIdd
    {
        public static readonly CountryIdd Empty = new CountryIdd();

        [JsonPropertyName("root")]
        public string Root
        {
            get; init;
        } = string.Empty;

        [JsonPropertyName("suffixes")]
        public IEnumerable<string> Suffixes
        {
            get; init;
        } = Enumerable.Empty<string>();
    }

    class CountryName
    {
        public static readonly CountryName Empty = new CountryName();

        [JsonPropertyName("common")]
        public string Common
        {
            get; init;
        } = string.Empty;

        [JsonPropertyName("official")]
        public string Official
        {
            get; init;
        } = string.Empty;

        [JsonPropertyName("nativeName")]
        public Dictionary<string, CountryNativeName> NativeName
        {
            get; init;
        } = new Dictionary<string, CountryNativeName>();
    }

    class CountryNativeName
    {
        [JsonPropertyName("common")]
        public string Common
        {
            get; init;
        } = string.Empty;

        [JsonPropertyName("official")]
        public string Official
        {
            get; init;
        } = string.Empty;
    }

    #endregion
}
