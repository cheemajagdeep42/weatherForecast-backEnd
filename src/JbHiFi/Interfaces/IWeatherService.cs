namespace JbHiFi.Interfaces
{
    public interface IWeatherService
    {
        Task<string> GetWeatherDescriptionAsync(string city, string country);
    }
}
