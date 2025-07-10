using JbHiFi.Interfaces;
using JbHiFi.Settings;
using Microsoft.Extensions.Options;

namespace JbHiFi.Services
{
    public class SecretKeyProvider: ISecretKeyProvider
    {
        private readonly IWebHostEnvironment _env;
        private readonly Lazy<AwsParameterService> _aws;
        private readonly OpenWeatherKeySettings _owmSettings;
        private readonly ApiKeySettings _apiKeySettings;

        private const string OpenWeatherApiKeyPath = "/weatherapp/apiKeys/openWeatherApi";
        private const string ClientApiKeysPath = "/weatherapp/clientApiKeys/validKeys";

        public SecretKeyProvider(
            IWebHostEnvironment env,
            Lazy<AwsParameterService> aws,
            IOptions<OpenWeatherKeySettings> owmOptions,
            IOptions<ApiKeySettings> apiKeyOptions)
        {
            _env = env;
            _aws = aws;
            _owmSettings = owmOptions.Value;
            _apiKeySettings = apiKeyOptions.Value;
        }

        /// <summary>
        /// Returns the OpenWeather API key.
        /// Uses local config in Dev/Test and AWS Parameter Store otherwise.
        /// Supports multiple comma-separated keys but uses only the first one.
        /// </summary>
        public async Task<string> GetOpenWeatherApiKeyAsync()
        {
            if (_env.IsDevelopment() || _env.IsEnvironment("Test"))
            {
                return _owmSettings.ApiKey;
            }

            var keys = await _aws.Value.GetArrayParameterAsync(OpenWeatherApiKeyPath);
            var openWeatherApiKey = keys.FirstOrDefault();

            return string.IsNullOrWhiteSpace(openWeatherApiKey)
                ? throw new InvalidOperationException($"Missing OpenWeather API key at {OpenWeatherApiKeyPath}")
                : openWeatherApiKey;
        }

        /// <summary>
        /// Returns the list of valid client API keys.
        /// Uses local config in Dev/Test and AWS Parameter Store otherwise.
        /// </summary>
        public async Task<HashSet<string>> GetClientApiKeysAsync()
        {
            if (_env.IsDevelopment() || _env.IsEnvironment("Test"))
            {
                return _apiKeySettings.ValidKeys?.ToHashSet() ?? new HashSet<string>();
            }

            var keys = await _aws.Value.GetArrayParameterAsync(ClientApiKeysPath);
            return keys.ToHashSet();
        }
    }
}
