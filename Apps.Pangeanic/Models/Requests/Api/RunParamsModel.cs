using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Requests.Api;

public class RunParamsModel
{
    [JsonProperty("Sensitivity")]
    public string Sensitivity { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Tags")]
    public List<string> Tags { get; set; }
}