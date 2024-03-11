using Apps.Pangeanic.Models.Responses;
using Apps.Pangeanic.Models.Responses.Api;
using Newtonsoft.Json;

namespace Apps.Pangeanic.Utils;

public class RunParmsConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(RunParmsResponse);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        // Deserialize the nested JSON string.
        var jsonString = (string)reader.Value;
        return JsonConvert.DeserializeObject<RunParmsResponse>(jsonString);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        // Serialize the object back to a JSON string.
        var jsonString = JsonConvert.SerializeObject(value);
        writer.WriteValue(jsonString);
    }
}
