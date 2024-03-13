using Newtonsoft.Json;

namespace Apps.Pangeanic.Webhooks.Payload;

public class TranslationStatusUpdatedPayload
{
    [JsonProperty("fileid")]
    public string FileId { get; set; }

    [JsonProperty("data")]
    public TranslationUpdatedDataPayload Data { get; set; }
}