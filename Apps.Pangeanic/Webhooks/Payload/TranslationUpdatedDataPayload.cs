using Newtonsoft.Json;

namespace Apps.Pangeanic.Webhooks.Payload;

public class TranslationUpdatedDataPayload
{
    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}
