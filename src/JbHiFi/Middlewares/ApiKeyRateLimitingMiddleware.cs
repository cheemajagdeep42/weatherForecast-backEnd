using JbHiFi.Interfaces;
using JbHiFi.Settings;
using Microsoft.Extensions.Options;

namespace JbHiFi.Middlewares
{
    public class ApiKeyRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyRateLimitingMiddleware> _logger;
        private readonly HashSet<string> _validKeys;
        private readonly IRateLimitTracker _rateLimiter;

        public ApiKeyRateLimitingMiddleware(
            RequestDelegate next,
            ILogger<ApiKeyRateLimitingMiddleware> logger,
            IOptions<ApiKeySettings> options,
            IRateLimitTracker rateLimiter)
        {
            _next = next;
            _logger = logger;
            _validKeys = options.Value.ValidKeys.ToHashSet();
            _rateLimiter = rateLimiter;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var apiKey = context.Request.Headers["X-API-KEY"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing API Key");
                return;
            }

            if (!_validKeys.Contains(apiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            if (_rateLimiter.IsLimitExceeded(apiKey))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("API key has exceeded its hourly limit (5 requests/hour). Try again later.");
                return;
            }

            _rateLimiter.RegisterCall(apiKey);

            _logger.LogInformation("API Key {ApiKey} used.", apiKey);

            await _next(context);
        }
    }
}
