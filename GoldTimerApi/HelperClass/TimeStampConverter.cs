using System.Text.Json.Serialization;
using System.Text.Json;

namespace GoldFetchTimer.HelperClass
{
    public class TimestampConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the value is a long (Unix timestamp)
            if (reader.TokenType == JsonTokenType.Number)
            {
                // Convert the long to string and return
                return reader.GetInt64().ToString();
            }

            // If it's already a string, just return it as is
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            // Handle other cases (like null or invalid)
            throw new JsonException("Unexpected token type for Timestamp.");
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            // Convert string back to number for serialization, if needed (optional)
            writer.WriteStringValue(value);
        }
    }
}
