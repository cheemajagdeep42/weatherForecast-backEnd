using System.Text.Json;
using JbHiFi.Exceptions;
using JbHiFi.Interfaces;
using JbHiFi.Models;
using JbHiFi.Settings;
using Microsoft.Extensions.Options;

namespace JbHiFi.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenWeatherKeySettings _owmSettings;
        private readonly ILogger<WeatherService> _logger;
        private readonly ISecretKeyProvider _secretKeyProvider;

        public WeatherService(
            HttpClient httpClient,
            IOptions<OpenWeatherKeySettings> options,
            ILogger<WeatherService> logger,
            ISecretKeyProvider secretKeyProvider)
        {
            _httpClient = httpClient;
            _owmSettings = options.Value;
            _logger = logger;
            _secretKeyProvider = secretKeyProvider;
        }

        public async Task<string> GetWeatherDescriptionAsync(string city, string country)
        {
            try
            {
                var apiKey = await _secretKeyProvider.GetOpenWeatherApiKeyAsync();
                var url = $"{_owmSettings.BaseUrl}/weather?q={city},{country}&appid={apiKey}";

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<WeatherResponseModel>(content, options);

                if (data?.Cod == "404")
                {
                    _logger.LogWarning("Weather location not found: {City}, {Country}", city, country);
                    throw new LocationNotFoundException(data.Message ?? "Location not found");
                }

                var description = data?.Weather?.FirstOrDefault()?.Description;

                if (string.IsNullOrWhiteSpace(description))
                {
                    _logger.LogError("Weather description is missing or invalid for {City}, {Country}", city, country);
                    throw new Exception("Weather description is missing or invalid.");
                }

                return description;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to OpenWeatherMap failed for {City}, {Country}", city, country);
                throw;
            }
        }
    }
}
