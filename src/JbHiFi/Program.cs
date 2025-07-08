using JbHiFi.Interfaces;
using JbHiFi.Settings;
using JbHiFi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddSingleton<IRateLimitTracker, InMemoryRateLimitTracker>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JB Hi-Fi Weather API", Version = "v1" });

    // Register the custom header filter
    c.OperationFilter<AddApiKeyHeaderOperationFilter>();
});

builder.Services.Configure<ApiKeySettings>(
    builder.Configuration.GetSection("ApiKeySettings"));

builder.Services.Configure<OpenWeatherMapSettings>(
    builder.Configuration.GetSection("OpenWeatherMap"));


// Allow CORS
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


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<JbHiFi.Middlewares.ApiKeyRateLimitingMiddleware>();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }
