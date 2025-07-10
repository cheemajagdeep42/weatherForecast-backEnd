using System.Net;
using JbHiFi.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace JbHiFi.Tests.Controllers
{
    public class WeatherControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly Mock<IWeatherService> _weatherServiceMock;
        private const string ValidApiKey = "test-key-1";
        private const string City = "sydney";
        private const string Country = "au";

        public WeatherControllerTests(CustomWebApplicationFactory factory)
        {
            _weatherServiceMock = new Mock<IWeatherService>();

            var clientFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace WeatherService with mock
                    var weatherServiceDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IWeatherService));
                    if (weatherServiceDescriptor != null)
                        services.Remove(weatherServiceDescriptor);

                    services.AddSingleton<IWeatherService>(_weatherServiceMock.Object);

                    // Disable rate limiting in tests
                    var rateLimitDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IRateLimitTracker));
                    if (rateLimitDescriptor != null)
                        services.Remove(rateLimitDescriptor);

                    var rateLimiterMock = new Mock<IRateLimitTracker>();
                    rateLimiterMock.Setup(x => x.IsLimitExceeded(It.IsAny<string>())).Returns(false);
                    rateLimiterMock.Setup(x => x.RegisterCall(It.IsAny<string>()));
                    services.AddSingleton<IRateLimitTracker>(rateLimiterMock.Object);
                });
            });

            _client = clientFactory.CreateClient();
        }

        [Fact]
        public async Task Returns200_WhenCityAndCountryAreValid()
        {
            _weatherServiceMock
                .Setup(s => s.GetWeatherDescriptionAsync(City, Country))
                .ReturnsAsync("clear sky");

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"/api/weather/description?city={City}&country={Country}");
            request.Headers.Add("X-API-KEY", ValidApiKey);

            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("clear sky", content);
        }

        [Theory]
        [InlineData("", Country)]
        [InlineData(City, "")]
        [InlineData("", "")]
        public async Task Returns400_WhenCityOrCountryMissing(string city, string country)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/weather/description?city={city}&country={country}");
            request.Headers.Add("X-API-KEY", ValidApiKey);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Returns400_WhenLocationNotFound()
        {
            _weatherServiceMock
                .Setup(s => s.GetWeatherDescriptionAsync("nowhere", "xx"))
                .ThrowsAsync(new JbHiFi.Exceptions.LocationNotFoundException("City not found"));

            var request = new HttpRequestMessage(HttpMethod.Get,
                "/api/weather/description?city=nowhere&country=xx");
            request.Headers.Add("X-API-KEY", ValidApiKey);

            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("City not found", content);
        }

        [Fact]
        public async Task Returns503_WhenHttpRequestFails()
        {
            _weatherServiceMock
                .Setup(s => s.GetWeatherDescriptionAsync(City, Country))
                .ThrowsAsync(new HttpRequestException("Network issue"));

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"/api/weather/description?city={City}&country={Country}");
            request.Headers.Add("X-API-KEY", ValidApiKey);

            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            Assert.Contains("Weather service is unavailable", content);
        }

        [Fact]
        public async Task Returns500_WhenUnexpectedExceptionOccurs()
        {
            _weatherServiceMock
                .Setup(s => s.GetWeatherDescriptionAsync(City, Country))
                .ThrowsAsync(new Exception("Boom"));

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"/api/weather/description?city={City}&country={Country}");
            request.Headers.Add("X-API-KEY", ValidApiKey);

            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Contains("An unexpected error occurred", content);
        }
    }
}
