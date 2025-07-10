using JbHiFi.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace JbHiFi.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // Replace IWeatherService with mock
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IWeatherService));
                if (descriptor != null)
                    services.Remove(descriptor);

                var mockWeatherService = new Mock<IWeatherService>();
                mockWeatherService
                    .Setup(s => s.GetWeatherDescriptionAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync("clear sky");

                services.AddSingleton<IWeatherService>(mockWeatherService.Object);
            });
        }
    }
}
