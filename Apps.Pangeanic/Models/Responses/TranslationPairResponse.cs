using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Responses;

public class TranslationPairResponse
{
    [JsonProperty("src"), Display("Source text")]
    public string Source { get; set; }

    [JsonProperty("tgt"), Display("Target text")]
    public string Target { get; set; }
}