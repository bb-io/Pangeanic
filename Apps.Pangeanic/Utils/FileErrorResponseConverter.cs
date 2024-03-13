using Apps.Pangeanic.Models.Responses.Api;
using Newtonsoft.Json;

namespace Apps.Pangeanic.Utils;

public class FileErrorResponseConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var jsonString = JsonConvert.SerializeObject(value);
        writer.WriteValue(jsonString);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var jsonString = (string)reader.Value;
        return JsonConvert.DeserializeObject<FileErrorResponse>(jsonString);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(FileErrorResponse);
    }
}