using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Requests;

public class BaseJsonRequest
{
    [JsonProperty("apikey")]
    public string ApiKey { get; set; }
}