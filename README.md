# 🌦️ WeatherForecast Back-End API

This is the Back-End service for the Weather Forecast App, built using **ASP.NET Core (.NET 8)**.  
It powers real-time weather data by connecting to **OpenWeatherMap** and includes:
- 🔐 API key validation  
- 📉 Rate limiting (5 requests/hour)  
- 🐳 Dockerized setup  
- ✅ Unit + integration testing  
- 📄 Swagger API documentation  


### Features
- API Key validation via custom middleware  
- Per-key rate limiting (5 requests/hour)  
- Fetch weather descriptions from OpenWeatherMap  
- Unit + integration tests using xUnit  
- Dockerized for local development  
- Swagger support for API docs  

---

## 🛠️ Getting Started (Local Development)
1. Clone the repository  
2. Ensure Docker is installed  
3. Run the backend:
   Go to this older under Project- src/JbHiFi and run below Command in bash
     docker-compose up --build
   API will be ready on this url now - http://localhost:5000/api/weather/description?city=sydney&country=aus


🧪 Running Tests
    Run all unit and integration tests:
    dotnet test

⚙️ Environment Variables
    This app requires:
   🔑  ValidKeys – Used to authenticate incoming requests
        Development: appsettings.Development.json
        Production: AWS SSM Parameter Store

🌐 OPENWEATHER_API_KEY – Used to call OpenWeatherMap API
    Stored securely in AWS SSM
    For local testing, can be overridden in appSettings.Development.json

📄 Swagger – API Documentation
     Swagger UI is enabled for exploring and testing APIs.
     Visit: http://localhost:5000/swagger

🔐 API Key & Rate Limiting
     Every request header must include:
     X-API-KEY: your-key-value
     Each key is limited to 5 requests/hour
     If limit is exceeded, response will be: 429 Too Many Requests

🚀 Production Deployment
    Hosted on AWS App Runner
    Docker image from Amazon ECR
    CI/CD via GitHub Actions
    Secrets managed via AWS Parameter Store
    Custom domain: https://api.weatherreportinfo.com



