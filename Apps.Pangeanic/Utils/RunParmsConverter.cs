using Apps.Pangeanic.Models.Responses.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Pangeanic.Utils;

public class RunParmsConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(RunParmsResponse);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return new RunParmsResponse();

        if (reader.TokenType == JsonToken.String)
        {
            var jsonString = reader.Value?.ToString();

            if (string.IsNullOrWhiteSpace(jsonString))
                return new RunParmsResponse();

            return JsonConvert.DeserializeObject<RunParmsResponse>(jsonString) ?? new RunParmsResponse();
        }

        if (reader.TokenType == JsonToken.StartObject)
        {
            var token = JToken.Load(reader);
            return token.ToObject<RunParmsResponse>(serializer) ?? new RunParmsResponse();
        }

        throw new JsonSerializationException(
            $"Unexpected token {reader.TokenType} when parsing run parameters.");
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var jsonString = JsonConvert.SerializeObject(value);
        writer.WriteValue(jsonString);
    }
}
