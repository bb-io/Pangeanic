using Newtonsoft.Json;

namespace Apps.Pangeanic.Models.Responses.Api;

public class RunParmsResponse
{
    [JsonProperty("type")]
    public TypeSelectionResponse Type { get; set; }
    
    [JsonProperty("tags")]
    public TagsSelectionResponse Tags { get; set; }
}