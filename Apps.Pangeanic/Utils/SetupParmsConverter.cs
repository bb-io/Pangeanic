using Apps.Pangeanic.Models.Responses;
using Newtonsoft.Json;

namespace Apps.Pangeanic.Utils;

public class SetupParmsConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(SetupParmsResponse);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        // Deserialize the nested JSON string.
        var jsonString = (string)reader.Value;
        return JsonConvert.DeserializeObject<SetupParmsResponse>(jsonString);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        // Serialize the object back to a JSON string.
        var jsonString = JsonConvert.SerializeObject(value);
        writer.WriteValue(jsonString);
    }
}
