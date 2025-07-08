using JbHiFi.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace JbHiFi.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ApiKeySettings:ValidKeys:0", "test-api-key" }
                });
            });

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IWeatherService));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Mock IWeatherService
                var mock = new Mock<IWeatherService>();
                mock.Setup(s => s.GetWeatherDescriptionAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync("clear sky");

                // Add the mocked service
                services.AddSingleton<IWeatherService>(mock.Object);
            });
        }
    }
}
