🌦️ WeatherForecast Back-End API
    This is the Back-End service for the Weather Forecast App, built using ASP.NET Core (.NET 8).
    It powers real-time weather data by connecting to OpenWeatherMap and includes:
      API key validation
      Rate limiting (5 requests/hour)
      Dockerized setup
      Unit + integration testing
      Swagger API documentation
      Features
      API Key validation via custom middleware
      Per-key rate limiting (5 requests/hour)
      Fetch weather descriptions from OpenWeatherMap
      Unit + integration tests using xUnit
      Dockerized for local development
      Swagger support for API docs



🛠️ Getting Started (Local Development)
    Clone the repository
    Ensure Docker is installed
    Run the backend: docker-compose up --build
    Access the API at: http://localhost:5000/api/weather/description?city=sydney&country=aus



🧪 Running Tests
    Run all unit and integration tests with:
    dotnet test


⚙️ Environment Variables
    This app requires:
    🔑 ValidKeys- Used to authenticate incoming requests
        Defined in:
          Development: appsettings.Development.json
          Production: AWS SSM Parameter Store

   🌐 OPENWEATHER_API_KEY- Used to call OpenWeatherMap API
       Stored securely in AWS SSM (For local testing- can be overriden using appSettings.Development.json file)

   📄 Swagger - API Documentation
        Swagger UI is enabled for exploring and testing APIs.
        Visit: http://localhost:5000/swagger



Production Deployment:
  Hosted on AWS App Runner
  Docker image from Amazon ECR
  CI/CD via GitHub Actions
  Secrets via AWS Parameter Store
  Custom domain: https://api.weatherreportinfo.com
  API Key & Rate Limiting
  Every request Header must include: X-API-KEY: 'key-value-here'
  Each key is limited to 5 requests/hour
  Exceeding limit returns: 429 Too Many Requests
