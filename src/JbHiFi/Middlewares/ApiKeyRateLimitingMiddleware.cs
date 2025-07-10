using JbHiFi.Interfaces;
using JbHiFi.Services;

namespace JbHiFi.Middlewares
{
    public class ApiKeyRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyRateLimitingMiddleware> _logger;
        private readonly IRateLimitTracker _rateLimiter;
        private readonly ISecretKeyProvider _secretKeyProvider;
        private readonly HashSet<string> _validKeys = new();

        public ApiKeyRateLimitingMiddleware(
            RequestDelegate next,
            ILogger<ApiKeyRateLimitingMiddleware> logger,
            IRateLimitTracker rateLimiter,
            ISecretKeyProvider secretKeyProvider)
        {
            _next = next;
            _logger = logger;
            _rateLimiter = rateLimiter;
            _secretKeyProvider = secretKeyProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            if (!context.Request.Path.StartsWithSegments("/api/weather/description", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var apiKey = context.Request.Headers["X-API-KEY"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing API Key");
                return;
            }

            // Load valid keys
            if (_validKeys.Count == 0)
            {
                var keys = await _secretKeyProvider.GetClientApiKeysAsync();
                foreach (var key in keys)
                {
                    _validKeys.Add(key);
                }
            }

            if (!_validKeys.Contains(apiKey))
            {
                _logger.LogWarning("Invalid API Key attempted: {ApiKey}", apiKey);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            if (_rateLimiter.IsLimitExceeded(apiKey))
            {
                _logger.LogWarning("Rate limit exceeded for API Key: {ApiKey}", apiKey);
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers["Retry-After"] = "3600";
                await context.Response.WriteAsync("API key has exceeded its hourly limit (5 requests/hour). Try again later.");
                return;
            }

            _rateLimiter.RegisterCall(apiKey);
            _logger.LogInformation("API Key {ApiKey} used.", apiKey);

            await _next(context);
        }
    }
}
