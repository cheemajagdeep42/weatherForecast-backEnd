using System.Net;
using JbHiFi.Exceptions;
using JbHiFi.Interfaces;
using JbHiFi.Services;
using JbHiFi.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace JbHiFi.Tests.Services
{
    public class WeatherServiceTests
    {
        private WeatherService CreateService(string jsonResponse, HttpStatusCode statusCode)
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://api.openweathermap.org")
            };

            var options = Options.Create(new OpenWeatherKeySettings
            {
                BaseUrl = "https://api.openweathermap.org/data/2.5"
            });

            var loggerMock = new Mock<ILogger<WeatherService>>();

            // ✅ Now mock the interface
            var secretKeyProviderMock = new Mock<ISecretKeyProvider>();
            secretKeyProviderMock
                .Setup(p => p.GetOpenWeatherApiKeyAsync())
                .ReturnsAsync("test-api-key");

            return new WeatherService(httpClient, options, loggerMock.Object, secretKeyProviderMock.Object);
        }

        [Fact]
        public async Task GetWeatherDescriptionAsync_ReturnsDescription_WhenValidResponse()
        {
            var json = @"{
                ""weather"": [ { ""description"": ""clear sky"" } ],
                ""cod"": ""200""
            }";

            var service = CreateService(json, HttpStatusCode.OK);
            var result = await service.GetWeatherDescriptionAsync("sydney", "au");

            Assert.Equal("clear sky", result);
        }

        [Fact]
        public async Task GetWeatherDescriptionAsync_ThrowsException_WhenDescriptionMissing()
        {
            var json = @"{ ""cod"": ""200"" }";
            var service = CreateService(json, HttpStatusCode.OK);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.GetWeatherDescriptionAsync("sydney", "au"));
            Assert.Contains("Weather description is missing", ex.Message);
        }

        [Fact]
        public async Task GetWeatherDescriptionAsync_ThrowsLocationNotFoundException_OnApiError()
        {
            var json = @"{ ""cod"": ""404"", ""message"": ""city not found"" }";
            var service = CreateService(json, HttpStatusCode.OK);

            var ex = await Assert.ThrowsAsync<LocationNotFoundException>(() =>
                service.GetWeatherDescriptionAsync("sydney", "au"));
            Assert.Contains("city not found", ex.Message);
        }
    }
}
