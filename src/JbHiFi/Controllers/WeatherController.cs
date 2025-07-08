using Microsoft.AspNetCore.Mvc;
using JbHiFi.Interfaces;
using JbHiFi.Exceptions;

namespace JbHiFi.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        /// <summary>
        /// Returns the weather description for a given city and country.
        /// </summary>
        /// <param name="city">City name (e.g., London)</param>
        /// <param name="country">Country code (e.g., uk, au)</param>
        /// <returns>Weather description as JSON</returns>
        [HttpGet("description")]
        public async Task<IActionResult> GetWeather([FromQuery] string city, [FromQuery] string country)
        {
            if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(country))
            {
                return BadRequest("Both city and country are required.");
            }


            try
            {
                var description = await _weatherService.GetWeatherDescriptionAsync(city, country);
                return Ok(new { description });
            }
            catch (LocationNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, "Weather service is unavailable.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
