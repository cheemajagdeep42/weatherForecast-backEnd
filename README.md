# WeatherForecast Back-End API

This is the **back-end service** for the Weather Forecast application, implemented using **ASP.NET Core (.NET 8)**.  
It provides weather data by calling the OpenWeatherMap API, with features like **rate limiting**, **API key validation**, and **unit/integration tests**.

---

## Features

- API Key validation using middleware
- Rate limiting (per API key)
- Fetch weather data from OpenWeatherMap API
- Unit and integration tests using xUnit
- Dockerized setup for deployment
- Swagger/OpenAPI support

---

## Testing

- Run all tests:
  ```bash
  dotnet test
  ```

---

## ⚙️ Configuration

### Required Environment Variables (can be set in AWS / CI / secrets)

| Name                  | Description                          |
|-----------------------|--------------------------------------|
| `OPENWEATHER_API_KEY` | Your OpenWeatherMap API key          |

> You can add these in your `.env` (for local) or in Vercel/AWS as environment variables.

---

## 🐳 Docker Usage

```bash
docker-compose up --build
The API will be available at: http://localhost:5000/api/weather
```

---

## Deployment

This back-end can be deployed via:
- AWS EC2 / ECS
- GitHub Actions CI/CD
- Connected to Vercel front-end via env var `NEXT_PUBLIC_API_BASE_URL`

---

## Notes

- This repo is for educational/demo use
- To test rate limiting, avoid caching on frontend

---
