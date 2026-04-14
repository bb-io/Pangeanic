using Apps.Pangeanic.Models.Responses.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Pangeanic.Utils;

public class SetupParmsConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(SetupParmsResponse);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return new SetupParmsResponse();

        if (reader.TokenType == JsonToken.String)
        {
            var jsonString = reader.Value?.ToString();

            if (string.IsNullOrWhiteSpace(jsonString))
                return new SetupParmsResponse();

            return JsonConvert.DeserializeObject<SetupParmsResponse>(jsonString) ?? new SetupParmsResponse();
        }

        if (reader.TokenType == JsonToken.StartObject)
        {
            var token = JToken.Load(reader);
            return token.ToObject<SetupParmsResponse>(serializer) ?? new SetupParmsResponse();
        }

        throw new JsonSerializationException(
            $"Unexpected token {reader.TokenType} when parsing setup parameters.");
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
