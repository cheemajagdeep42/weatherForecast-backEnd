using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace JbHiFi.Tests.Middlewares
{
    public class ApiKeyRateLimitingMiddlewareTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private const string ValidApiKey = "test-api-key";

        public ApiKeyRateLimitingMiddlewareTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Returns400_WhenApiKeyIsMissing()
        {
            var response = await _client.GetAsync("/weather?q=sydney,au");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Returns401_WhenApiKeyIsInvalid()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/weather?q=sydney,au");
            request.Headers.Add("X-API-KEY", "invalid-key");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Returns429_WhenApiKeyExceedsLimit()
        {
            for (int i = 0; i < 5; i++)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "api/weather/description?city=sydney&country=au");
                request.Headers.Add("X-API-KEY", ValidApiKey);


                var response = await _client.SendAsync(request);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            // Sixth request should be rejected
            var finalRequest = new HttpRequestMessage(HttpMethod.Get, "/weather?q=sydney,au");
            finalRequest.Headers.Add("X-API-KEY", ValidApiKey);

            var finalResponse = await _client.SendAsync(finalRequest);
            Assert.Equal(HttpStatusCode.TooManyRequests, finalResponse.StatusCode);
        }
    }
}
