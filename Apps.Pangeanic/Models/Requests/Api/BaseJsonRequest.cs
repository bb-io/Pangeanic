using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Requests.Api;

public class BaseJsonRequest
{
    [JsonProperty("apikey")]
    public string ApiKey { get; set; }
}