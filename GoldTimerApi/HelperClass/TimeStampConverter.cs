using System.Text.Json.Serialization;
using System.Text.Json;

namespace GoldFetchTimer.HelperClass
{
    public class TimestampConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt64().ToString();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            throw new JsonException("Unexpected token type for Timestamp.");
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
