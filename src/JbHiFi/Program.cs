using JbHiFi.Interfaces;
using JbHiFi.Settings;
using JbHiFi.Services;
using Microsoft.OpenApi.Models;
using Amazon.SimpleSystemsManagement;
using JbHiFi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddSingleton<IRateLimitTracker, InMemoryRateLimitTracker>();
builder.Services.Configure<ApiKeySettings>(builder.Configuration.GetSection("ApiKeySettings"));
builder.Services.Configure<OpenWeatherKeySettings>(builder.Configuration.GetSection("OpenWeatherApi"));

// Add AWS services
builder.Services.AddAWSService<IAmazonSimpleSystemsManagement>();

// Register AwsParameterService + Lazy wrapper
builder.Services.AddSingleton<AwsParameterService>();
builder.Services.AddSingleton(provider => new Lazy<AwsParameterService>(
    () => provider.GetRequiredService<AwsParameterService>()
));

// Register the SecretKeyProvider
builder.Services.AddSingleton<ISecretKeyProvider, SecretKeyProvider>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JB Hi-Fi Weather API", Version = "v1" });
    c.OperationFilter<AddApiKeyHeaderOperationFilter>();
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

app.UseRouting();

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply API Key Middleware
app.UseMiddleware<ApiKeyRateLimitingMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.Run();

// Allow WebApplicationFactory to access Program in integration tests
public partial class Program { }
