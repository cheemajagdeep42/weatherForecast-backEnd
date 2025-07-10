using System.Text.Json.Serialization;
using JbHiFi.Utils;

namespace JbHiFi.Models
{
    public class WeatherResponseModel
    {
        [JsonConverter(typeof(StringOrIntConverter))]
        public string? Cod { get; set; }
        public string? Message { get; set; }
        public List<WeatherItem>? Weather { get; set; }
    }

    public class WeatherItem
    {
        public int Id { get; set; }

        public string? Main { get; set; }

        public string? Description { get; set; }

        public string? Icon { get; set; }
    }
}
