namespace JbHiFi.Interfaces
{
    public interface IRateLimitTracker
    {
        bool IsLimitExceeded(string apiKey);
        void RegisterCall(string apiKey);
    }
}
