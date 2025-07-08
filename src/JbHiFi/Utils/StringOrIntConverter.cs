using System.Text.Json.Serialization;
using System.Text.Json;

namespace JbHiFi.Utils
{
    public class StringOrIntConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Number => reader.GetInt32().ToString(),
                _ => throw new JsonException("Unsupported token type for 'cod'")
            };
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (int.TryParse(value, out var intValue))
                writer.WriteNumberValue(intValue);
            else
                writer.WriteStringValue(value);
        }
    }
}
