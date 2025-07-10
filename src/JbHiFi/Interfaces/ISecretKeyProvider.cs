namespace JbHiFi.Interfaces
{
    public interface ISecretKeyProvider
    {
        Task<string> GetOpenWeatherApiKeyAsync();
        Task<HashSet<string>> GetClientApiKeysAsync();
    }

}
